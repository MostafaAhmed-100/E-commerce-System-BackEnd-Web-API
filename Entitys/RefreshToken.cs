using Newtonsoft.Json.Linq;

namespace WebApplication1.Entitys
{
    public class RefreshToken
    {
        public int TokenId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsActive => RevokedOn == null && ExpiresOn > DateTime.UtcNow;

    }
}
