using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.ReviewDtos.ResponseDto;
using WebApplication1.DTOS.ReviewRequestDto;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Services.ReviewService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<ActionResult<ApiResponseDto<ReviewResponseDto>>> AddReview([FromBody] CreateReviewRequestDto requestDto)
        {
            int buyerId = int.Parse(User.FindFirstValue("ProfileId")!);
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _reviewService.AddReviewAsync(requestDto, buyerId, userId);
            return Ok(result);
        }

        [HttpGet("Variant/{variantId}")]
        public async Task<ActionResult<ApiResponseDto<PaginatedResponseDto<ReviewResponseDto>>>> GetVariantReviews(int variantId, [FromQuery] PaginationRequestDto paginationRequestDto)
        {
            var result = await _reviewService.GetVariantReviewsAsync(variantId, paginationRequestDto);
            return Ok(result);
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = AppRoles.Buyer)]
        public async Task<ActionResult<ApiResponseDto<string>>> DeleteReview(int reviewId)
        {
            int buyerId = int.Parse(User.FindFirstValue("ProfileId")!);
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _reviewService.DeleteReviewAsync(reviewId, buyerId, userId);
            return Ok(result);
        }
    }
}