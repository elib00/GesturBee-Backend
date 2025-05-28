namespace GesturBee_Backend.DTO
{
    public class CreateExerciseContentDTO
    {
        public string ContentS3Key { get; set; } = string.Empty; //file key in S3 bucket
        public string ContentType { get; set; } = string.Empty;
        public string BatchId { get; set; } = string.Empty;
        public int ItemNumber { get; set; }
    }
}
