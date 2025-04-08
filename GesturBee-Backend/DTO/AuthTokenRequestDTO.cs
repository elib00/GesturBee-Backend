namespace GesturBee_Backend.DTO
{
    public class AuthTokenRequestDTO
    {
        public string? Email { get; set; }
        public List<string> Roles { get; set; }
        public string? Type { get; set; }
    }
}
