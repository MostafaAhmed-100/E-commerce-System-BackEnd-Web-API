using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Entitys
{
    public class Role : IdentityRole<int>
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
