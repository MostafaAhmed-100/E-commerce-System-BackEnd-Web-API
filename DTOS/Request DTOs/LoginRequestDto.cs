using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class LoginRequestDto
    {
        [Required, EmailAddress]
        public required string Email{ get; set; }
        [Required,MinLength(6)]
        public required string Password { get; set; }
    }
}
