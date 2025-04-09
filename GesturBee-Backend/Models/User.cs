using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GesturBee_Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [NotMapped] //i store ra nako diri later on ang mga roless
        public List<string> Roles { get; set; } = new List<string>(["User"]);

        [Required] //para needed sha for creation
        public required UserAccount Account { get; set; }

        [Required]
        public required UserProfile Profile { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}
