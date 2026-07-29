using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.ReviewDtos.ResponseDto;
using WebApplication1.DTOS.ReviewRequestDto;
using WebApplication1.DTOS.Shared.RequestDto;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<ReviewResponseDto>> AddReviewAsync(CreateReviewRequestDto requestDto, int buyerId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null || buyer.BuyerId != buyerId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to add a review for unauthorized BuyerId {BuyerId}.", userId, buyerId);
                    throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user.");
                }

                var existingReview = await _unitOfWork.ReviewRepository.GetReviewByBuyerAndVariantAsync(buyerId, requestDto.ProductVariantId);
                if (existingReview != null)
                {
                    _logger.LogWarning("Buyer {BuyerId} attempted to review ProductVariant {VariantId} more than once.", buyerId, requestDto.ProductVariantId);
                    throw new ConflictException("You have already reviewed this product variant.");
                }

                var review = _mapper.Map<Review>(requestDto);
                review.BuyerId = buyerId;
                review.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.ReviewRepository.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Buyer {BuyerId} successfully added a review for ProductVariant {VariantId}.", buyerId, requestDto.ProductVariantId);

                return new ApiResponseDto<ReviewResponseDto>
                {
                    Message = "Review added successfully.",
                    Data = _mapper.Map<ReviewResponseDto>(review)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while adding review for ProductVariant {VariantId} by Buyer {BuyerId}", requestDto.ProductVariantId, buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<ReviewResponseDto>>> GetVariantReviewsAsync(int productVariantId, PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var result = await _unitOfWork.ReviewRepository.GetReviewsByVariantIdAsync(
                    productVariantId,
                    paginationRequestDto.PageNumber,
                    paginationRequestDto.PageSize
                );

                if (result == null)
                {
                    return new ApiResponseDto<PaginatedResponseDto<ReviewResponseDto>>
                    {
                        Message = "No reviews found for this product variant.",
                        Data = new PaginatedResponseDto<ReviewResponseDto>
                        {
                            CurrentPage = paginationRequestDto.PageNumber,
                            PageSize = paginationRequestDto.PageSize,
                            TotalCount = 0,
                            TotalPages = 0,
                            Data = new List<ReviewResponseDto>()
                        }
                    };
                }

                var (reviews, totalCount) = result.Value;
                int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequestDto.PageSize);
                var mappedReviews = _mapper.Map<List<ReviewResponseDto>>(reviews);

                return new ApiResponseDto<PaginatedResponseDto<ReviewResponseDto>>
                {
                    Message = "Product variant reviews retrieved successfully.",
                    Data = new PaginatedResponseDto<ReviewResponseDto>
                    {
                        CurrentPage = paginationRequestDto.PageNumber,
                        PageSize = paginationRequestDto.PageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        Data = mappedReviews
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving reviews for ProductVariant {VariantId}", productVariantId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteReviewAsync(int reviewId, int buyerId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(userId);
                if (buyer == null || buyer.BuyerId != buyerId)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to delete a review for unauthorized BuyerId {BuyerId}.", userId, buyerId);
                    throw new UnauthorizedException("The provided Buyer ID does not belong to the authenticated user.");
                }

                var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId);
                if (review == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent ReviewId {ReviewId}.", reviewId);
                    throw new NotFoundException("The review does not exist.");
                }

                if (review.BuyerId != buyerId)
                {
                    _logger.LogWarning("Security Warning: Buyer {BuyerId} attempted to delete ReviewId {ReviewId} belonging to another buyer.", buyerId, reviewId);
                    throw new UnauthorizedException("You do not have permission to delete this review.");
                }

                _unitOfWork.ReviewRepository.Delete(review);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Buyer {BuyerId} successfully deleted ReviewId {ReviewId}.", buyerId, reviewId);

                return new ApiResponseDto<string>
                {
                    Message = "Review deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting ReviewId {ReviewId}", reviewId);
                throw;
            }
        }
    }
}