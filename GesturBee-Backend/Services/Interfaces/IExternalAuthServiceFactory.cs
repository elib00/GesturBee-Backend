using GesturBee_Backend.Enums;

namespace GesturBee_Backend.Services.Interfaces
{
    public interface IExternalAuthServiceFactory
    {
        IExternalAuthService GetAuthService(AuthType authType);
    }
}
