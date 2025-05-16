using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class CreateClassDTO
    {   
        [Required(ErrorMessage = "Teacher Id is required.")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        public string? ClassName { get; set; }
        public string? ClassDescription { get; set; }
    }
}
