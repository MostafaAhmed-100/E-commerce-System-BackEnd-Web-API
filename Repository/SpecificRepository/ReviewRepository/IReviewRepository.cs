using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.ReviewRepository
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<Review?> GetReviewByBuyerAndVariantAsync(int buyerId, int productVariantId);
        Task<(List<Review?>, int TotalCount)?> GetReviewsByVariantIdAsync(int productVariantId, int PageNumber , int PageSize);
    }
}