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

        public string ExerciseTitle {  get; set; }
        public string ExerciseDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public Teacher? Teacher { get; set; }
        public List<ExerciseItem> ExerciseItems { get; set; } = new List<ExerciseItem>();
    }
}
