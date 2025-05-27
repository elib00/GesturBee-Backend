namespace GesturBee_Backend.Services.Interfaces
{
    public interface IS3Service
    {
        string GeneratePreSignedClassVideoUploadUrl(string fileName, string contentType);
    }
}
