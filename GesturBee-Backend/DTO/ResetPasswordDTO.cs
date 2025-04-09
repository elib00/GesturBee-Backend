namespace GesturBee_Backend.DTO
{
    public class ResetPasswordDTO
    {
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
