using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Address
    {
        public int AddressId { get; set; }
        public required int UserId { get; set; }
        public required string City { get; set; }
        public required string HomeAddress { get; set; }
        public required string State { get; set; }
        public required string Zip_Code { get; set; }
        public User User { get; set; }
    }
}
