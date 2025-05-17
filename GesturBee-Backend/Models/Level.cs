using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class Level
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StageId")]
        public int StageId { get; set; }

        public string LevelName { get; set; }
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public Stage Stage { get; set; }

    }
}
