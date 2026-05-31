using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.CartRepository
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithItemsAsync(int buyerId);
    }
}
