using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.Models
{
    public class EnrollmentRequest
    {
        [Key]
        public int Id { get; set; }
        public DateTime RequestedAt { get; set; }

        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        //nav prop
        public User Student { get; set; }
        public Class Class { get; set; }
    }
}
