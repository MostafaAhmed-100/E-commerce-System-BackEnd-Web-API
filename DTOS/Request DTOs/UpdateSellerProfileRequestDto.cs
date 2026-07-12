namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateSellerProfileRequestDto
    {
        public required string SellerStoreName { get; set; }
        public required string SellerPhoneNumber { get; set; }
        public required string SellerBankName { get; set; }
        public required string SellerBankAccountNumber { get; set; }
    }
}
