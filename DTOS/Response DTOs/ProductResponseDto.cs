using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Response_DTOs
{
    public class ProductResponseDto
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required string CategoryName { get; set; }
        public required string SellerStoreName { get; set; }
        public required List<ProductVariantResponseDto> Variants { get; set; }
    }
}
