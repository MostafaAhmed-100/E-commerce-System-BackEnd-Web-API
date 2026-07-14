using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.WishlistItemRepository
{
    public interface IWishlistItemRepository : IGenericRepository<WishlistItem>
    {
        Task<bool> CheckVariantExistsInWishlistAsync(int wishlistId, int variantId);

        Task<(IEnumerable<WishlistItem> Items, int TotalCount)> GetWishlistItemsPaginatedAsync(int wishlistId, int pageNumber, int pageSize);
    }
}
