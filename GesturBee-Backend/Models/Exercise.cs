using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GesturBee_Backend.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TeacherId")]
        public int TeacherId { get; set; }

        public string ExerciseTitle { get; set; } = string.Empty;
        public string ExerciseDescription { get; set; } = string.Empty;
        public string BatchId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public User? Teacher { get; set; }
        public ICollection<ExerciseItem> ExerciseItems { get; set; } = [];
    }
}
