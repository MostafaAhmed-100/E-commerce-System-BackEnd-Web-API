
using Microsoft.EntityFrameworkCore;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Implementation
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Cart>> GetAll()
        {
            var AllCarts = await _context.Carts
                .Include(p => p.Products).ToListAsync();
            return AllCarts;
        }
        public async Task<Cart?> GetById(int id)
        {
            var Result = await _context.Carts.Include(p => p.Products).FirstOrDefaultAsync(c => c.CartId == id);
            return Result;
        }

        public async Task<Cart> Create(Cart cart)
        {
            var Create = await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return Create.Entity;
        }
        public async Task<Cart?> UpdateById(int Id, Cart cart)
        {
             var Result = await _context.Carts.Include(p => p.Products).FirstOrDefaultAsync(c => c.CartId == Id);
            if (Result == null) return null;
            Result.Quantity = cart.Quantity;
            Result.Size = cart.Size;
            await _context.SaveChangesAsync();
            return Result;
        }

        public async Task<bool?> DeleteById(int Id)
        {
            var Result = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == Id);
            if (Result == null) return false;
            _context.Carts.Remove(Result);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
