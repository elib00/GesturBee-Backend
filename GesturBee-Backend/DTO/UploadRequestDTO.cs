namespace GesturBee_Backend.DTO
{
    public class UploadRequestDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string BatchId { get; set; } = string.Empty; // Unique identifier for the batch
        public int ItemNumber { get; set; }
    }
}
