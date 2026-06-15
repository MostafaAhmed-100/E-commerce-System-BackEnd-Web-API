using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.CouponRepository
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string couponCode)
        {
            return await _AppDbcontext.Set<Coupon>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CouponCode == couponCode);
        }
    }
}