using WebApplication1.Entitys;
using WebApplication1.Repository.GenericRepository;

namespace WebApplication1.Repository.SpecificRepository.ProductVariantRepository
{
    public interface IProductVariantRepository : IGenericRepository<ProductVariant>
    {
    }
}
