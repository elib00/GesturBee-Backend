using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class ClassAdmissionDTO
    {
        [Required]
        public int? StudentId { get; set; }
        [Required]
        public int? ClassId { get; set; }
        public string ClassCode { get; set; } = String.Empty;

        [Required]
        public bool Accept { get; set; }
    }
}
