using GesturBee_Backend;
using GesturBee_Backend.Repository;
using GesturBee_Backend.Repository.Interfaces;
using GesturBee_Backend.Services;
using GesturBee_Backend.Services.Factory;
using GesturBee_Backend.Services.Interfaces;
using GesturBee_Backend.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BackendDbContext>(
    db => db.UseSqlServer(builder.Configuration.GetConnectionString("BackendDbConnectionString")), ServiceLifetime.Scoped
);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JSON serialization settings for enum conversion to string
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpClient();

// Add dependencies
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEClassroomService, EClassroomService>();
builder.Services.AddScoped<IEClassroomRepository, EClassroomRepository>();
builder.Services.AddScoped<IRoadmapRepository, RoadmapRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<FacebookAuthService>();
builder.Services.AddScoped<IExternalAuthServiceFactory, ExternalAuthServiceFactory>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<GoogleTokenValidator>();
builder.Services.AddScoped<FacebookTokenValidator>();


// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Change for other providers
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax; // Or SameSiteMode.None for cross-origin
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Only secure if HTTPS
    options.Cookie.HttpOnly = true; // Make the cookie inaccessible to JavaScript
    //options.LoginPath = "/login";
}) // Required for OAuth callbacks
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    //googleOptions.SignInScheme = "External";
    //googleOptions.CallbackPath = "/api/auth/signin-google/";
    googleOptions.CallbackPath = "/signin-google";
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    facebookOptions.CallbackPath = "/signin-facebook";

    facebookOptions.Scope.Add("email");          // Access to the user's email address
    facebookOptions.Scope.Add("public_profile"); // Access to basic profile info (name, profile picture, etc.)
    facebookOptions.Scope.Add("user_birthday");  // Access to the user's birthdate
    facebookOptions.Scope.Add("user_gender");    // Access to the user's gender
});

//cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
     policy.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod());

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//rate limiter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
       RateLimitPartition.GetFixedWindowLimiter(
           partitionKey: httpContext.User.Identity?.Name
               ?? httpContext.Connection.RemoteIpAddress?.ToString()
               ?? "unknown",
           factory: _ => new FixedWindowRateLimiterOptions
           {
               PermitLimit = 5,  // Allow 20 requests per user per minute per ip
               Window = TimeSpan.FromMinutes(1),
               QueueLimit = 0
           }));

    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5; // Allow 5 requests
        limiterOptions.Window = TimeSpan.FromSeconds(10); // Every 10 seconds
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2; // Allow 2 extra requests in queue
    });

    //options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var response = new
        {
            message = "Too many requests. Please try again later.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? retryAfter.TotalSeconds
                : (double?) null
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
    };
});



// Add authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//use cors
app.UseCors("DevelopmentPolicy");

// Use authentication
app.UseAuthentication();

//authorization
app.UseAuthorization();

//rate limiter
app.UseRateLimiter();

app.UseHttpsRedirection();



app.MapControllers();

app.Run();

