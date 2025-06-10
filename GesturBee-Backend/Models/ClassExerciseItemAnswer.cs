using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class ClassExerciseItemAnswer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ClassExerciseId")]
        public int ClassExerciseId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public int ItemNumber { get; set; }
        public string Answer { get; set; } = string.Empty;

        [JsonIgnore]
        public ClassExercise? ClassExercise { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

    }
}