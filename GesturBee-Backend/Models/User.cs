using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required] //para needed sha for creation
        public required UserAccount Account { get; set; }

        [Required]
        public required UserProfile Profile { get; set; }

        public DateTime? LastLogin { get; set; }

        [JsonIgnore]
        public ICollection<Class> TaughtClasses { get; set; } = new List<Class>();
        [JsonIgnore]
        public ICollection<EnrollmentRequest> EnrollmentRequests { get; set; } = new List<EnrollmentRequest>();
        [JsonIgnore]
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

    }
}
