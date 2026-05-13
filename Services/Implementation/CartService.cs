using WebApplication1.DTOS;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        public CartService(ICartRepository CartRepository)
        {
            _repository = CartRepository;
        }

        public async Task<List<CartModel>> GetAllAsync()
        {
            var GetAllCategorys = await _repository.GetAll();
            return GetAllCategorys.Select(x => new CartModel
            {
                CartId =x.CartId,
                Quantity =x.Quantity,
                Size =x.Size,
            }).ToList();
        }

        public async Task<CartModel?> GetByIdAsync(int id)
        {
            var Result = await _repository.GetById(id);
            if (Result == null) { return null; }
            return new CartModel
            {
                CartId = Result.CartId,
                Quantity = Result.Quantity,
                Size = Result.Size,
            };
        }
        public async Task<CartModel> CreateAsync(CartDTO cartDTO)
        {
            var cart = new Cart
            {
                Quantity = cartDTO.Quantity,
                Size = cartDTO.Size,
            };

            var created = await _repository.Create(cart);

            return new CartModel
            {
                CartId = created.CartId,
                Quantity = created.Quantity,
                Size = created.Size,
            };
        }

        public async Task<CartModel?> UpdateByIdAsync(int cartId, CartDTO cartDTO)
        {
            var cart = new Cart
            {
                CartId = cartId,
                Quantity = cartDTO.Quantity,
                Size = cartDTO.Size,
            };

            var result = await _repository.UpdateById(cartId, cart);
            if (result == null) return null;

            return new CartModel
            {
                CartId = result.CartId,
                Quantity = result.Quantity,
                Size = result.Size,
            };
        }

        public async Task<bool?> DeleteByIdAsync(int id)
        {
            return await _repository.DeleteById(id);
        }

    }
}
