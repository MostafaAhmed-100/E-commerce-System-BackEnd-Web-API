using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterSellerRequestDto
    {
        [Required , EmailAddress]
        public string SellerEmail { get; set; }
        [Required , MinLength(7)]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required, MaxLength(90)]
        public string BankAccountNumber { get; set; }
        [Required]
        public string StoreName { get; set; }
        [Required , Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string TaxNumber { get; set; }

    }
}
