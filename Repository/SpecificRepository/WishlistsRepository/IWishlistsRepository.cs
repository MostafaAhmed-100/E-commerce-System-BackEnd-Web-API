using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.WishlistsRepository
{
    public interface IWishlistsRepository :IGenericRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetBuyerWishlistsAsync(int buyerId)
    }
}
