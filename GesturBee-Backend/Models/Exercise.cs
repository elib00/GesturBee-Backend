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
        public int TeacherId;

        public string? ExerciseTitle {  get; set; }

        [JsonIgnore]
        public Teacher? Teacher { get; set }
    }
}
