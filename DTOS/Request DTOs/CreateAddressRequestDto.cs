namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateAddressRequestDto
    {
        public required string City { get; set; }
       
        public required string State { get; set; }
       
        public required string HomeAddress { get; set; }
        
        public required string ZipCode { get; set; }
    }
}
