using WebApplication1.DTOS;
using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface ICartService
    {
        public Task<List<CartModel>> GetAllAsync();
        public Task<CartModel?> GetByIdAsync(int id);
        public Task<CartModel> CreateAsync(CartDTO cart);
        public Task<CartModel?> UpdateByIdAsync(int cartId, CartDTO cartDTO);
        public Task<bool?> DeleteByIdAsync(int id);
    }
}
