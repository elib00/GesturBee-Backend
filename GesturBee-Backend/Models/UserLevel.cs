using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class UserLevel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("LevelId")]
        public int LevelId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string Status { get; set; }

        [JsonIgnore]
        public Level Level { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
