using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class LoginRequestDto
    {
        public required string Email{ get; set; }
        public required string Password { get; set; }
    }
}
