namespace WebApplication1.DTOS.Request_DTOs
{
    public class AddSavedCardRequestDto
    {
        public required string CardBrand { get; set; }
        public required string MaskedNumber { get; set; }
        public required string CardToken { get; set; }
    }
}