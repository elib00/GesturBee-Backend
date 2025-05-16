using System.ComponentModel.DataAnnotations.Schema;

namespace GesturBee_Backend.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        //nav prop
        public User? User { get; set; }

        public List<Class> Classes { get; set; }
    }
}
