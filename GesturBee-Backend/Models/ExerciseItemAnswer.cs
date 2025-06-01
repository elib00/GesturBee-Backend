namespace GesturBee_Backend.Models
{
    public class ExerciseItemAnswer
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public int ItemNumber { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}