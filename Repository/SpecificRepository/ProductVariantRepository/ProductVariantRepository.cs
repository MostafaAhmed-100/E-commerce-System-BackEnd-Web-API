using WebApplication1.Data;
using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;
using WebApplication1.Repository.SpecificRepository.ProductRepository;

namespace WebApplication1.Repository.SpecificRepository.ProductVariantRepository
{
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }
    }
}
