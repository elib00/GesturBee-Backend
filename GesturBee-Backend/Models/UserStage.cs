using MimeKit.Tnef;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GesturBee_Backend.Models
{
    public class UserStage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StageId")]
        public int StageId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string Status { get; set; }

        public int CurrentStars { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public Stage Stage { get; set; }
    }
}
