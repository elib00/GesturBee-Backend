using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class ClassExercise
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        [JsonIgnore]
        public Class Class { get; set; }
    }
}
