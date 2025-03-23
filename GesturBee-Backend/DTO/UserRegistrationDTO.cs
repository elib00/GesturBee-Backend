using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class UserRegistrationDTO
    {
        //TODO: Implement Role

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string? Password { get; set; }



        //for the profile
        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        public string? ContactNumber { get; set; }

        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
