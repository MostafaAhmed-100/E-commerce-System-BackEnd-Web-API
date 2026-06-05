using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.AddressRepository
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(int UserId)
        {
            var Addresses = await _AppDbcontext.Addresses
                .Where(p => p.UserId == UserId)
                .ToListAsync();
            return Addresses;
        }
    }
}
