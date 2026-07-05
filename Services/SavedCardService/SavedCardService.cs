using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.SavedCardRepository;

namespace WebApplication1.Services
{
    public class SavedCardService : ISavedCardService
    {
        private readonly ISavedCardRepository _savedCardRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SavedCardService> _logger;

        public SavedCardService(
            ISavedCardRepository savedCardRepository,
            IMapper mapper,
            ILogger<SavedCardService> logger)
        {
            _savedCardRepository = savedCardRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponseDto<string>> AddCardAsync(AddSavedCardRequestDto dto, int userId)
        {
            var card = _mapper.Map<SavedCard>(dto);
            card.UserId = userId;

            await _savedCardRepository.AddAsync(card);

            var isSaved = await _savedCardRepository.SaveChangesAsync();
            if (!isSaved)
            {
                _logger.LogError("Database error: Failed to save new card for User {UserId}.", userId);
                throw new BadRequestException("حدث خطأ أثناء حفظ الكارت في قاعدة البيانات.");
            }

            _logger.LogInformation("User {UserId} successfully added a new saved card.", userId);

            return new ApiResponseDto<string>
            {
                Message = "Card added successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<IEnumerable<SavedCardResponseDto>>> GetMyCardsAsync(int userId)
        {
            var cards = await _savedCardRepository.GetSavedCardByUserIdAsync(userId);

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

        public async Task<ApiResponseDto<string>> DeleteCardAsync(int cardId, int userId)
        {
            var card = await _savedCardRepository.GetByIdAsync(cardId);

            if (card == null || card.UserId != userId || !card.IsActive)
            {
                _logger.LogWarning("Security/Validation Warning: User {UserId} attempted to delete CardId {CardId} which is non-existent, unauthorized, or already inactive.", userId, cardId);
                throw new NotFoundException("الكارت غير موجود أو لا تملك صلاحية حذفه.");
            }

            card.IsActive = false;
            _savedCardRepository.Update(card);

            var isUpdated = await _savedCardRepository.SaveChangesAsync();
            if (!isUpdated)
            {
                _logger.LogError("Database error: Failed to soft-delete CardId {CardId} for User {UserId}.", cardId, userId);
                throw new BadRequestException("حدث خطأ أثناء محاولة حذف الكارت من قاعدة البيانات.");
            }

            _logger.LogInformation("User {UserId} successfully soft-deleted CardId {CardId}.", userId, cardId);

            return new ApiResponseDto<string>
            {
                Message = "Card deleted successfully.",
                Data = null
            };
        }
    }
}