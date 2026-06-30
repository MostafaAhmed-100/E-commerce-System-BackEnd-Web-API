using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entitys
{
    public class User : IdentityUser<int>
    {
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public Buyer? Buyer { get; set; }
        public Seller? Seller { get; set; }

    }
}
