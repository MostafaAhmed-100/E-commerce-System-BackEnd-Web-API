namespace WebApplication1.Entitys
{
    public class Buyer
    {
        public int BuyerId { get; set; }
        public int UserId { get; set; }
        public int? LoyaltyPoints { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string? PaymentGatewayCustomerId { get; set; }
        public required User User { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
