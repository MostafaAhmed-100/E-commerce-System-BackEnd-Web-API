using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.ProductRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithCategoryAsync(int id);

        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);

        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);

        Task<Product?> GetProductWithVariantsAsync(int ProductId);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync();
        Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsPagedAsync(int? categoryId, int pageNumber, int pageSize);
    }
}
