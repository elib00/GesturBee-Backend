using Google.Apis.Auth;

namespace GesturBee_Backend.Validators
{
    public class GoogleTokenValidator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleTokenValidator> _logger;

        public GoogleTokenValidator(IConfiguration configuration, ILogger<GoogleTokenValidator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
        {
            try
            {
                var clientId = _configuration["Authentication:Google:ClientId"];

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Google token validation");
                return null;    
            }

        }
    }
}
