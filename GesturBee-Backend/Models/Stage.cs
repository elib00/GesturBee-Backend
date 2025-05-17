using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GesturBee_Backend.Models
{
    public class Stage
    {
        [Key]
        public int Id { get; set; }

        public string StageName { get; set; }
        public int RequiredStars { get; set; }
    }
}
