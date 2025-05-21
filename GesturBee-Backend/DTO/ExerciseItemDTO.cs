using GesturBee_Backend.Models;
using System.ComponentModel.DataAnnotations;

namespace GesturBee_Backend.DTO
{
    public class ExerciseItemDTO
    {
        [Required]
        public int ExerciseItemId { get; set; }
        public int? ItemNumber { get; set; }
        public string? Question { get; set; }
        public string? ChoiceA { get; set; }
        public string? ChoiceB { get; set; }
        public string? ChoiceC { get; set; }
        public string? ChoiceD { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
