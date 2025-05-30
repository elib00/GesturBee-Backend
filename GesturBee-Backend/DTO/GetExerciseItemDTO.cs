namespace GesturBee_Backend.DTO
{
    public class GetExerciseItemDTO
    {
        public int ItemNumber { get; set; }
        public string Question { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string PresignedURL { get; set; } = string.Empty; // file key in S3 bucket
        public string ChoiceA { get; set; } = string.Empty;
        public string ChoiceB { get; set; } = string.Empty;
        public string ChoiceC { get; set; } = string.Empty;
        public string ChoiceD { get; set; } = string.Empty;
    }
}
