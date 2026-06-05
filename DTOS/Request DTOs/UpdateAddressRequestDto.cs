using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class UpdateAddressRequestDto
    {
        [Required]
        public required string City { get; set; }

        [Required]
        public required string State { get; set; }

        [Required]
        public required string HomeAddress { get; set; }

        [Required]
        public required string ZipCode { get; set; }
    }
}
