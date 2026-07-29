using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.RefreshTokenRepository;

namespace WebApplication1.Repository.SpecificRepository.ReviewRepository
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Review?> GetReviewByBuyerAndVariantAsync(int buyerId, int productVariantId)
        {
            var review = await _AppDbcontext.reviews
                .AsNoTracking()
                .Include(x => x.Buyer)
                .ThenInclude(x => x.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId && x.ProductVariantId == productVariantId);
            return review;
        }

        public async Task<(List<Review>, int TotalCount)?> GetReviewsByVariantIdAsync(int productVariantId, int PageNumber, int PageSize)
        {
            var query = _AppDbcontext.reviews.Where(x => x.ProductVariantId == productVariantId)
                .AsNoTracking()
                .Include(x => x.Buyer)
                .ThenInclude(x => x.User);

            var TotalCount = await query.CountAsync();
            if (TotalCount == 0)
                return null;
            var Items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip(PageSize * (PageNumber - 1))
                .Take(PageSize)
                .ToListAsync();
            return (Items , TotalCount);
        }
    }
}
