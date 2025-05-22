namespace GesturBee_Backend.Services.Interfaces
{
    public interface IS3Service
    {
        string GeneratePreSignedUploadUrl(string fileName, string contentType);
    }
}
