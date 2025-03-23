using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace GesturBee_Backend.Models
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lacking credentials. Email must be provided.")]
        [EmailAddress(ErrorMessage = "Invalid email format. Please try again.")]
        public required string Email { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public int UserId { get; set; }  // Foreign key for User

        [JsonIgnore]
        public User? User { get; set; }  // Navigation property to access the associated User

    }
}
