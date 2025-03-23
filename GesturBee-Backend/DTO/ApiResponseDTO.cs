using GesturBee_Backend.Enums;

namespace GesturBee_Backend.DTO
{
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public ResponseType? ResponseType { get; set; }
        public T? Data { get; set; }
    }
}
