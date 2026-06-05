using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.CouponRepository
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon?> GetCouponByCodeAsync(string couponCode);
    }
}