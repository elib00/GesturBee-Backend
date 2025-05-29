namespace GesturBee_Backend.DTO
{
    public class CreateExerciseItemDTO
    {
        public int ItemNumber { get; set; }
        public string Question { get; set; } = "";
        public string ChoiceA { get; set; } = "";
        public string ChoiceB { get; set; } = "";
        public string ChoiceC { get; set; } = "";
        public string ChoiceD { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
    }
}
