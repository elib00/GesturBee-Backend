using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class ExerciseItem
    {
        [Key]
        public int Id {  get; set; }

        [ForeignKey("ExerciseId")]
        public int ExerciseId { get; set; }

        public int ItemNumber { get; set; }
        public Video Video { get; set; }
        public string Question { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; }

        [JsonIgnore]
        public Exercise Exercise { get; set; }
    }
}
