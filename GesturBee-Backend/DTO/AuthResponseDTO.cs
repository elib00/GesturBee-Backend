namespace GesturBee_Backend.DTO
{
    public class AuthResponseDTO
    {
        public string? Token { get; set; }
        public DateTime Expiry { get; set; }
        public string? RefreshToken { get; set; } 
        //public UserDTO User { get; set; }

    }
}
