namespace GesturBee_Backend.Services.Interfaces
{
    public interface IS3Service
    {
        string GeneratePresignedClassVideoUploadUrl(string fileName, string contentType);
        string GeneratePresignedFetchExerciseContentUrl(string key);
    }
}
