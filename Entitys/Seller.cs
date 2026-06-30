using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class Seller
    {
        public int SellerId { get; set; }
        public required string TaxNumber { get; set; }
        public required string BankAccountNumber { get; set; }
        public required string BankName { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public  DateTime CreatedAt { get; set; } = DateTime.Now;
        public  bool IsDeleted { get; set; } = false;
        public required int UserId { get; set; }
        public required User User { get; set; }
        public ICollection<Product> products { get; set; } = new List<Product>();
    }
}
