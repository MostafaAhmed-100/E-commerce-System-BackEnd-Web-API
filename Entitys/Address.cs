using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Required]
        public required int UserId { get; set; }
        [Required]
        public required string City { get; set; }

        [Required]
        public required string HomeAddress { get; set; }
        [Required]
        public required string State { get; set; }
        [Required]
        public required string Zip_Code { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
