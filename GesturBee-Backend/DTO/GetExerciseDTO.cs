namespace GesturBee_Backend.DTO
{
    public class GetExerciseDTO
    {
        public string ExerciseTitle { get; set; } = string.Empty;
        public string ExerciseDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<GetExerciseItemDTO> ExerciseItems { get; set; } = [];
    }
}
