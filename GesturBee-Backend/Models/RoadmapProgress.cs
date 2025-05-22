using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class RoadmapProgress
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public int Stage { get; set; } = 1;
        public int Level { get; set; } = 1;

        [JsonIgnore]
        public User? User { get; set; }
    }
}
