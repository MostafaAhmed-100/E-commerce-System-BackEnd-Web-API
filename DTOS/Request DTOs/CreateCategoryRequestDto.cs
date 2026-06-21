using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs
{
    public class CreateCategoryRequestDto
    {
        public required string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
