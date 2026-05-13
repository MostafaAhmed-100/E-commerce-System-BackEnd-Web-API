using WebApplication1.DTOS;
using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface IProductService
    {
        public Task<ProductModel?> GetByProductId(int id);
        public Task<List<ProductModel>> GetAllProducts();
        public Task<ProductModel> CreateProduct(ProductDTO productDTO );
        public Task<ProductDTO?> UpdateProduct(int Id ,Product productD);
        public Task<bool?> DeleteByProductId(int id);
    }
}
