using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterAdminRequestDto
    {
        public string AdminEmail { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string AdminSecretCode { get; set; }

    }
}
