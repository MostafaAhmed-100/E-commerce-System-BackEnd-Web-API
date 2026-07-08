using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class ForgotPasswordRequestDto
    {
        [EmailAddress]
        public required string Email { get; set; }
    }
}
