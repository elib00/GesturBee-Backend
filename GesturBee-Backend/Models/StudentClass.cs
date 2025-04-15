using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class StudentClass
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        [JsonIgnore]
        public Student Student { get; set; }

        [JsonIgnore]
        public Class Class { get; set; }
    }
}
