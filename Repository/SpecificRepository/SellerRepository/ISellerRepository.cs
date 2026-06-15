using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.SellerRepository
{
    public interface ISellerRepository : IGenericRepository<Seller>
    {
        Task<Seller?> GetSellerIdByUserId(int UserId);
    }
}
