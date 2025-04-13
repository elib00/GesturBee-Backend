using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class UserRegistrationDTO
    {
        //TODO: Implement Role

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        //[Required(ErrorMessage = "Password is required.")]
        //not required for people who logged in using third-party services
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? ContactNumber { get; set; }

        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
