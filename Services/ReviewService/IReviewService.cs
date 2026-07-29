using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.ReviewDtos.ResponseDto;
using WebApplication1.DTOS.ReviewRequestDto;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.ReviewService
{
    public interface IReviewService
    {
        Task<ApiResponseDto<ReviewResponseDto>> AddReviewAsync(CreateReviewRequestDto requestDto, int buyerId, int userId);
        Task<ApiResponseDto<PaginatedResponseDto<ReviewResponseDto>>> GetVariantReviewsAsync(int productVariantId, PaginationRequestDto paginationRequestDto);
        Task<ApiResponseDto<string>> DeleteReviewAsync(int reviewId, int buyerId, int userId);
    }
}
