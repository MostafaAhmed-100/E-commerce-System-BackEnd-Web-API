using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Shared.RequestDto
{
    public class PaginationRequestDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
