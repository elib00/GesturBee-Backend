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

        public IExternalAuthService GetAuthService(string provider)
        {
            return provider switch
            {
                "Google" => _serviceProvider.GetRequiredService<GoogleAuthService>(),
                "Facebook" => _serviceProvider.GetRequiredService<FacebookAuthService>(),
                _ => throw new ArgumentException($"No service found for {provider}")
            };
        }
    }
}
