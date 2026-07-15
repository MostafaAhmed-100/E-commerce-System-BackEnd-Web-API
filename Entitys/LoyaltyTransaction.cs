namespace WebApplication1.Entitys
{
    public class LoyaltyTransaction
    {
        public int LoyaltyTransactionId { get; set; }

        public int BuyerId { get; set; }

        public int? OrderId { get; set; }

        public int Points { get; set; }

        public string TransactionType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Buyer Buyer { get; set; }

        public Order Order { get; set; }

    }
}
