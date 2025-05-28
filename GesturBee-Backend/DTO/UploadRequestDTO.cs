namespace GesturBee_Backend.DTO
{
    public class UploadRequestDTO
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string BatchId { get; set; } = string.Empty; // Unique identifier for the batch
        public int ItemNumber { get; set; }
    }
}
