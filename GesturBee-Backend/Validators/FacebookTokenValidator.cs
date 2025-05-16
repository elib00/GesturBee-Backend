using System.Text.Json;
using GesturBee_Backend.DTO;

namespace GesturBee_Backend.Validators
{
    public class FacebookTokenValidator
    {
        private readonly HttpClient _httpClient;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly ILogger<FacebookTokenValidator> _logger;

        public FacebookTokenValidator(IConfiguration config, IHttpClientFactory clientFactory, ILogger<FacebookTokenValidator> logger)
        {
            _httpClient = clientFactory.CreateClient();
            _appId = config["Authentication:Facebook:AppId"];
            _appSecret = config["Authentication:Facebook:AppSecret"];
            _logger = logger;
        }

        public async Task<FacebookUserInfoDTO> ValidateTokenAsync(string userAccessToken)
        {
            var appAccessToken = $"{_appId}|{_appSecret}";
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={userAccessToken}&access_token={appAccessToken}";

            var debugResponse = await _httpClient.GetAsync(debugUrl);
            var debugContent = await debugResponse.Content.ReadAsStringAsync();

            if (!debugResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook token debug failed: {Content}", debugContent);
                return null;
            }

            var debugJson = JsonDocument.Parse(debugContent);
            var isValid = debugJson.RootElement.GetProperty("data").GetProperty("is_valid").GetBoolean();
            if (!isValid) return null;

            // Fetch user info
            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={userAccessToken}";
            var userResponse = await _httpClient.GetAsync(userInfoUrl);
            var userContent = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook user fetch failed: {Content}", userContent);
                return null;
            }

            return JsonSerializer.Deserialize<FacebookUserInfoDTO>(userContent);
        }
    }
}
