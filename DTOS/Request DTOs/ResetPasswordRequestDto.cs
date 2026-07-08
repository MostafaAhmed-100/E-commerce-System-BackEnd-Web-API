namespace WebApplication1.DTOS.Request_DTOs
{
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
