using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class ExerciseContent
    {
        public int Id { get; set; }
        public string ContentS3Key { get; set; } = string.Empty; //file key in S3 bucket
        public string ContentType { get; set; } = string.Empty;
        public string BatchId { get; set; } = string.Empty;
        public int ExerciseId { get; set; }
        public int ItemNumber { get; set; }


        [JsonIgnore]
        public Exercise? Exercise { get; set; }

    }
}
