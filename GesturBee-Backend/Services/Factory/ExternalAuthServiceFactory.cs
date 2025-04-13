using GesturBee_Backend.Enums;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Services.Factory
{
    public class ExternalAuthServiceFactory : IExternalAuthServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ExternalAuthServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IExternalAuthService GetAuthService(AuthType providerType)
        {
            return providerType switch
            {
                AuthType.GoogleAuth => _serviceProvider.GetRequiredService<GoogleAuthService>(),
                AuthType.FacebookAuth => _serviceProvider.GetRequiredService<FacebookAuthService>(),
                _ => throw new ArgumentException($"No service found for {providerType}")
            };
        }
    }
}
