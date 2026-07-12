using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.BuyerRepository
{
    public interface IBuyerRepository : IGenericRepository<Buyer>
    {
        Task<Buyer?> GetBuyerByUserId(int UserId);
        
        Task<Buyer?> GetBuyerWithAddressesById(int BuyerId);
    }
}
