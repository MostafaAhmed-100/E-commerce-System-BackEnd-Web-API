using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOS;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProductRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Product>> GetAll()
        {
            
           var AllProdct = await _appDbContext.Products
                    .Include(c => c.Category).ToListAsync();
            return AllProdct;
        }
        public async Task<Product?> GetById(int id)
        {
            var ProdctGetId = await _appDbContext.Products.Include(c => c.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            return ProdctGetId;
        }
        
        
        public async Task<Product> Create(Product product)
        {
          var entry=  await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task<Product?> UpdateById(int ID, Product product)
        {
            var UpdateProduct = await _appDbContext.Products.Include(c => c.Category).FirstOrDefaultAsync(p => p.ProductId == ID);
            if (UpdateProduct == null) { return null; }
            UpdateProduct.ProductName = product.ProductName;
            UpdateProduct.ProductDescription = product.ProductDescription;
            UpdateProduct.CategoryId = product.CategoryId;
            UpdateProduct.Category.CategoryName = product.Category.CategoryName;
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                UpdateProduct.ImagePath = product.ImagePath;
            }
            await _appDbContext.SaveChangesAsync();
            return UpdateProduct;
        }

        public async Task<bool?> DeleteById(int id)
        {
            var ProdctDeleteById = await _appDbContext.Products.Include(c => c.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            if (ProdctDeleteById == null) { return false; }
            _appDbContext.Remove(ProdctDeleteById);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

    }
}
