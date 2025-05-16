using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class AddStudentToClassDTO
    {
        [Required]
        public int? StudentId { get; set; }
        [Required]
        public int? ClassId { get; set; }
    }
}
