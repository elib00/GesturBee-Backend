using MimeKit.Tnef;
using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class RoadmapProgressDTO
    {
        [Required]
        public int Stage { get; set; }
        public int Level { get; set; }
    }
}
