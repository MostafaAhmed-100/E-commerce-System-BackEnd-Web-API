using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOS.Shared.RequestDto
{
    public class PaginationRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
