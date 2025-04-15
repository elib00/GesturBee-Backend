using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GesturBee_Backend.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("LevelId")]
        public int LevelId { get; set; }

        public string LessonName { get; set; }

        [JsonIgnore]
        public Level Level { get; set; }
    }
}
