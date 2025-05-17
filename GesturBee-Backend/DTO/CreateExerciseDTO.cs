using GesturBee_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class CreateExerciseDTO
    {
        [Required]
        public int TeacherId { get; set; }
        public string ExerciseTitle { get; set; }
        public string ExerciseDescription { get; set; }
        public List<ExerciseItem> ExerciseItems { get; set; } = new List<ExerciseItem>();
    }
}
