using System.ComponentModel.DataAnnotations.Schema;

namespace GesturBee_Backend.Models
{
    public class Student
    {
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        //nav prop
        public User? User { get; set; }

        public ICollection<ClassInvitation> ClassInvitations { get; set; }
        public ICollection<EnrollmentRequest> EnrollmentRequests { get; set; }
        public ICollection<StudentClass> StudentClasses { get; set; } 
    }
}
