using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services
{
    public class SavedCardService : ISavedCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SavedCardService> _logger;

        public SavedCardService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SavedCardService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<string>> AddCardAsync(AddSavedCardRequestDto dto, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var card = _mapper.Map<SavedCard>(dto);
                card.UserId = userId;

                await _unitOfWork.SavedCardRepository.AddAsync(card);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} successfully added a new saved card.", userId);

                return new ApiResponseDto<string>
                {
                    Message = "Card added successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error: Failed to save new card for User {UserId}.", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<IEnumerable<SavedCardResponseDto>>> GetMyCardsAsync(int userId)
        {
            try
            {
                var cards = await _unitOfWork.SavedCardRepository.GetSavedCardByUserIdAsync(userId);

                if (cards == null || !cards.Any())
                {
                    return new ApiResponseDto<IEnumerable<SavedCardResponseDto>>
                    {
                        Message = "No saved cards found for this user.",
                        Data = new List<SavedCardResponseDto>()
                    };
                }

                var mappedCards = _mapper.Map<IEnumerable<SavedCardResponseDto>>(cards);

                return new ApiResponseDto<IEnumerable<SavedCardResponseDto>>
                {
                    Message = "Saved cards retrieved successfully.",
                    Data = mappedCards
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cards for User {UserId}.", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteCardAsync(int cardId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var card = await _unitOfWork.SavedCardRepository.GetByIdAsync(cardId);

                if (card == null || card.UserId != userId || !card.IsActive)
                {
                    _logger.LogWarning("Security/Validation Warning: User {UserId} attempted to delete CardId {CardId} which is non-existent, unauthorized, or already inactive.", userId, cardId);
                    throw new NotFoundException("الكارت غير موجود أو لا تملك صلاحية حذفه.");
                }

                card.IsActive = false;
                _unitOfWork.SavedCardRepository.Update(card);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} successfully soft-deleted CardId {CardId}.", userId, cardId);

                return new ApiResponseDto<string>
                {
                    Message = "Card deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error: Failed to soft-delete CardId {CardId} for User {UserId}.", cardId, userId);
                throw;
            }
        }
    }
}