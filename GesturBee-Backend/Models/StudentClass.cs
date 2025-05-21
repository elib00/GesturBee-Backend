using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GesturBee_Backend.Models
{
    public class StudentClass
    {
        //MANY-TO-MANY RELATIONSHIP BETWEEN STUDENT AND CLASS
        [Key]
        public int Id { get; set; }

        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        //nav props
        public User Student { get; set; }
        public Class Class { get; set; }
    }
}
