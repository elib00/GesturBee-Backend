using Amazon.S3;
using Amazon.S3.Model;
using GesturBee_Backend.Services.Interfaces;

namespace GesturBee_Backend.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"];
        }

        public string GeneratePreSignedClassVideoUploadUrl(string fileName, string contentType)
        {
            string key = $"class_materials/{fileName}"; // teacher/id/uniqueId/video.mp4

            GetPreSignedUrlRequest request = new()
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = contentType
            };

            string url = _s3Client.GetPreSignedURL(request);
            return url;
        }
    }

}
