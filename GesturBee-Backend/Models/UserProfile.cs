using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GesturBee_Backend.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lacking credentials. First name must be provided.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Lacking credentials. Last name must be provided.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Lacking credentials. Contact number must be provided.")]
        public string? ContactNumber { get; set; }

        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public int UserId { get; set; }  // Foreign key for User

        // Navigation property to access the associated User
        [JsonIgnore]
        public User? User { get; set; }
    }
}
