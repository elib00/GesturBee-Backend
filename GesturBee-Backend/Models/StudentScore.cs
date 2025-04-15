using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class StudentScore
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        [ForeignKey("ExerciseId")]
        public int ExerciseId { get; set; }

        public int Score { get; set; }

        [JsonIgnore]
        public Student Student { get; set; }

        [JsonIgnore]
        public Exercise Exercise { get; set; }

    }
}
