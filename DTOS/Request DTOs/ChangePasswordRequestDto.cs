namespace WebApplication1.DTOS.Request_DTOs
{
    public class ChangePasswordRequestDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }

    }
}
