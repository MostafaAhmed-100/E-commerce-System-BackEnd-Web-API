using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Request_DTOs.Category
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        public string CategoryName { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
