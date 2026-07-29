using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.CartRepository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Cart?> GetCartWithItemsAsync(int buyerId)
        {
            var cart = await _AppDbcontext.Carts
                .Include(I => I.Items)
                .ThenInclude(pv => pv.ProductVariant)
                .ThenInclude(p => p.Product)
                .AsSplitQuery()
                .FirstOrDefaultAsync(B => B.BuyerId == buyerId);
            return cart;
        }
    }
}