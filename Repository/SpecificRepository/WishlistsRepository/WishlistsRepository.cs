using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.CategoryRepository.Interface;

namespace WebApplication1.Repository.SpecificRepository.WishlistsRepository
{
    public class WishlistsRepository : GenericRepository<Wishlist>, IWishlistsRepository
    {
        public WishlistsRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {

        }
        public async Task<IEnumerable<Wishlist>> GetBuyerWishlistsAsync(int buyerId)
        {
            var wishlists = await _AppDbcontext.wishlists
                .AsNoTracking()
                .Where(x => x.BuyerId == buyerId)
                .ToListAsync();

            return wishlists;
        }
    }
}
