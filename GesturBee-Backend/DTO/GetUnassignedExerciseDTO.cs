namespace GesturBee_Backend.DTO
{
    public class GetUnassignedExerciseDTO
    {
        public int Id { get; set; }
        public string ExerciseTitle { get; set; } = string.Empty;
        public string ExerciseDescription { get; set; } = string.Empty;
    }
}
