using Microsoft.AspNetCore.Server.HttpSys;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        [Required,EmailAddress]
        public string? Email { get; set; }
        [Required, MinLength(6)]
        public string? Password { get; set; }
        [Required, Compare("Password")]
        public string? ConfirmPassword { get; set; }


    }
}
