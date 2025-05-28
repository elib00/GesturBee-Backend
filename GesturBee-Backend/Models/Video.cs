using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ExerciseItemId")]
        public int ExerciseItemId { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public ExerciseItem ExerciseItem { get; set; }
    }
}