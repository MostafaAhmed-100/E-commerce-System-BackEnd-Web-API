namespace WebApplication1.DTOS.Shared
{
    public class Addressdto
    {
        public int AddressId { get; set; }
        public required string City { get; set; }
        public required string HomeAddress { get; set; }
        public required string State { get; set; }
        public required string Zip_Code { get; set; }
    }
}
