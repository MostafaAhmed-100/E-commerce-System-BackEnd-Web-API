using AutoMapper;
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

        public SavedCardService(ISavedCardRepository savedCardRepository, IMapper mapper)
        {
            _savedCardRepository = savedCardRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<string>> AddCardAsync(AddSavedCardRequestDto dto, int userId)
        {
            var card = _mapper.Map<SavedCard>(dto);
            card.UserId = userId;

            await _savedCardRepository.AddAsync(card);

            var isSaved = await _savedCardRepository.SaveChangesAsync();
            if (!isSaved)
            {
                throw new BadRequestException("حدث خطأ أثناء حفظ الكارت في قاعدة البيانات.");
            }

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
                throw new NotFoundException("الكارت غير موجود أو لا تملك صلاحية حذفه.");
            }

            card.IsActive = false;
            _savedCardRepository.Update(card);

            var isUpdated = await _savedCardRepository.SaveChangesAsync();
            if (!isUpdated)
            {
                throw new BadRequestException("حدث خطأ أثناء محاولة حذف الكارت من قاعدة البيانات.");
            }

            return new ApiResponseDto<string>
            {
                Message = "Card deleted successfully.",
                Data = null
            };
        }
    }
}