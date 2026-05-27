using Microsoft.AspNetCore.Server.HttpSys;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterRequestDto
    {
        [Required,EmailAddress]
        public required string Email { get; set; }
        [Required, MinLength(6)]
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string ConfirmPassword { get; set; }
        [Required]
        public required string Role { get; set; }

    }
}
