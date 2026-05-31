using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.ProductRepository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {

        public ProductRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Product?> GetProductWithCategoryAsync(int ProductId)
        {
            var productWithCategory = await _AppDbcontext.Products
                .Where(p => p.ProductId == ProductId)
                .Include(x => x.Category)
                .FirstOrDefaultAsync();
            return productWithCategory;
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var ProductsByCategory = await _AppDbcontext.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
            return ProductsByCategory;
        }
        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            var searchResults = await  _AppDbcontext.Products
                .Where(p => p.ProductName.Contains(searchTerm) 
                || p.ProductDescription.Contains(searchTerm))
                .ToListAsync();
            return searchResults;
        }
        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync()
        {
            var outOfStockProducts = await _AppDbcontext.Products
                .Include(p => p.ProductVariants)
                .Where(p => p.ProductVariants.All(v => v.QuantityInStock == 0))
                .ToListAsync();
            return outOfStockProducts;
        }


    }
}
