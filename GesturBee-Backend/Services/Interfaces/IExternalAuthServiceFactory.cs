namespace GesturBee_Backend.Services.Interfaces
{
    public interface IExternalAuthServiceFactory
    {
        IExternalAuthService GetAuthService(string provider);
    }
}
