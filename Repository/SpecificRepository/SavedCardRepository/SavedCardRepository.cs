using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.SavedCardRepository
{
    public class SavedCardRepository : GenericRepository<SavedCard>, ISavedCardRepository
    {
        public SavedCardRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<IEnumerable<SavedCard>> GetSavedCardByUserIdAsync(int userId)
        {
            return await _AppDbcontext.SavedCards
                .Where(c => c.UserId == userId && c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> CheckCardOwnershipAsync(int cardId, int userId)
        {
            return await _AppDbcontext.SavedCards
                .AnyAsync(c => c.CardId == cardId && c.UserId == userId && c.IsActive);
        }
    }
}