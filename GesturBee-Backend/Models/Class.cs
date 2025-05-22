using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class Class
    {
        [Key]
        public int Id {  get; set; }

        public string? ClassName { get; set; }
        public string? ClassDescription { get; set; }

        [JsonIgnore] //hide this because class code is a secret hehez
        public string? ClassCode { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("TeacherId")]
        public int TeacherId { get; set; }

        //[JsonIgnore]
        public User? Teacher { get; set; }

        [JsonIgnore]
        public ICollection<EnrollmentRequest> EnrollmentRequests { get; set; }

        [JsonIgnore]
        public ICollection<StudentClass> StudentClasses { get; set; }
    }
}
