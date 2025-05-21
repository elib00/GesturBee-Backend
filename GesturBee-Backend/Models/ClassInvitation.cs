using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GesturBee_Backend.Models
{
    public class ClassInvitation
    {
        [Key]
        public int Id { get; set; }
        public DateTime InvitedAt { get; set; }

        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        //nav prop
        public User Student { get; set; }  
        public Class Class { get; set; }    
    }
}
