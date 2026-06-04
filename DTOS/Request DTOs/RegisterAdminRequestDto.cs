using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterAdminRequestDto
    {
        [Required, EmailAddress]
        public string AdminEmail { get; set; }
        [Required, MinLength(7)]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string AdminSecretCode { get; set; }

    }
}
