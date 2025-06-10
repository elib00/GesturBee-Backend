namespace GesturBee_Backend.DTO
{
    public class ClassExerciseDTO
    {
        public int ClassExerciseId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseTitle { get; set; } = string.Empty;
        public string ExerciseDescription { get; set; } = string.Empty;
    }
}
