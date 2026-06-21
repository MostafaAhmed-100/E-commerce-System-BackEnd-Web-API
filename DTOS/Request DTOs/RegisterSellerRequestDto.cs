using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class RegisterSellerRequestDto
    {
        public string SellerEmail { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxNumber { get; set; }

    }
}
