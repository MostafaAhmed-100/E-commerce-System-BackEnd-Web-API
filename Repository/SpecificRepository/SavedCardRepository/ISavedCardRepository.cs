using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.SavedCardRepository
{
    public interface ISavedCardRepository : IGenericRepository<SavedCard>
    {
        Task<IEnumerable<SavedCard>> GetSavedCardByUserIdAsync(int userId);
        Task<bool> CheckCardOwnershipAsync(int cardId, int userId);
    }
}