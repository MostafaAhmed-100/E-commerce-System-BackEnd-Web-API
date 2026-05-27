using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Seller
    {
        [Key]
        public int SellerId { get; set; }
        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public required User User { get; set; }
        public string TaxNumber { get; set; }
        [Required,MaxLength(90)]
        public required string BankAccountNumber { get; set; }
        [Required,MaxLength(100)]
        public required string BankName { get; set; }
        [Required]
        public  DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public  bool IsDeleted { get; set; } = false;

        public ICollection<Product> products { get; set; } = new List<Product>();


    }
}
