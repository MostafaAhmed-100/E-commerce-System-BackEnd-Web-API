using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public required string Token { get; set; }
    }
}
