using Microsoft.AspNetCore.Server.HttpSys;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public  string PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }


    }
}
