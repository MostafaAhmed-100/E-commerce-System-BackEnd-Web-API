using WebApplication1.DTOS;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productService)
        {
            _productRepository = productService;
        }

        public async Task<List<ProductModel>> GetAllProducts()
        {
            var Products = await _productRepository.GetAll();
            return Products.Select(x => new ProductModel
            {
               
                ProductId = x.ProductId,
                ProductName = x.ProductName,

            }).ToList();
        }
        public async Task<ProductModel?> GetByProductId(int id)
        {
            var Result = await _productRepository.GetById(id);
            if (Result == null) { return null; }
            return new ProductModel
            {

                ProductId = Result.ProductId,
                ProductName = Result.ProductName,

            };
        }
        public async Task<ProductDTO?>? UpdateProduct(int Id  ,Product product)
        {
            var Result = await _productRepository.UpdateById(Id,product);
            if (Result == null) { return null; }
            return new ProductDTO
            {
                
                CategoryId = Result.CategoryId,
                ProductName = Result.ProductName,
                ProductDescription = Result.ProductDescription,
            

        };


        }
        public async Task<ProductModel> CreateProduct(ProductDTO product)
        {
            var entity = new Product
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ImagePath = product.ImagePath,
                CategoryId = product.CategoryId
            };

            var createdEntity = await _productRepository.Create(entity);

            return new ProductModel
            {
                ProductId = createdEntity.ProductId,
                ProductName = createdEntity.ProductName,
            };
        }

        public async Task<bool?> DeleteByProductId(int id)
        {
            var Result = await _productRepository.DeleteById(id);
            return Result;
           
        }



    }
}
