using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.WishlistItemRepository
{
    public class WishlistItemRepository : GenericRepository<WishlistItem>, IWishlistItemRepository
    {
        public WishlistItemRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
        public async Task<(IEnumerable<WishlistItem> Items, int TotalCount)> GetWishlistItemsPaginatedAsync(int wishlistId, int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.wishlistItems
                .Where(w => w.WishlistId == wishlistId)
                .Include(w => w.productVariant)   
                    .ThenInclude(v => v.Product)
                .AsSplitQuery()
                .AsNoTracking();
            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }

        public async Task<bool> CheckVariantExistsInWishlistAsync(int wishlistId, int variantId)
        {
            return await _AppDbcontext.wishlistItems
                .AsNoTracking()
                .AnyAsync(c => c.WishlistId == wishlistId && c.productVariantId == variantId);
        }
    }
}
