namespace WebApplication1.DTOS.Response_DTOs
{
    public class AddressResponseDto
    {
        public required int AddressId { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string HomeAddress { get; set; }
        public required string ZipCode { get; set; }
    }
}
