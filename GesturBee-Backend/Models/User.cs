using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

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
    }
}
