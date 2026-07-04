namespace WebApplication1.DTOS.Response_DTOs
{
    public class SavedCardResponseDto
    {
        public int CardId { get; set; }
        public required string CardBrand { get; set; }
        public required string MaskedNumber { get; set; }
    }
}