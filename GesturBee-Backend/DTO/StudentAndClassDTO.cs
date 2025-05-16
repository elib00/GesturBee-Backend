using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    //DTO for endpoints that need class id and student id
    public class StudentAndClassDTO
    {
        [Required]
        public int? StudentId { get; set; }
        [Required]
        public int? ClassId { get; set; }
    }
}
