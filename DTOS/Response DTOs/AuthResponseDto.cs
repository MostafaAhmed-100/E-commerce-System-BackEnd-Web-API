namespace WebApplication1.DTOS.Response_DTOs
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}
