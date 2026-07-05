using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.CouponRepository;

namespace WebApplication1.Services.CouponService
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponService> _logger;

        public CouponService(
            ICouponRepository couponRepository,
            IMapper mapper,
            ILogger<CouponService> logger)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CouponResponseDto>> CreateCouponAsync(CreateCouponRequestDto createCouponRequestDto, int sellerId)
        {
            if (createCouponRequestDto.StartDate >= createCouponRequestDto.EndDate)
            {
                _logger.LogWarning("Seller {SellerId} attempted to create a coupon with invalid dates " +
                    "(StartDate: {StartDate}, EndDate: {EndDate}).", sellerId, createCouponRequestDto.StartDate, createCouponRequestDto.EndDate);
                throw new BadRequestException("End date must be after the start date.");
            }

            var existingCoupon = await _couponRepository.GetCouponByCodeAsync(createCouponRequestDto.CouponCode);
            if (existingCoupon != null)
            {
                _logger.LogWarning("Seller {SellerId} attempted to create a coupon with " +
                    "an already existing code: {CouponCode}.", sellerId, createCouponRequestDto.CouponCode);
                throw new ConflictException("A coupon with this code already exists.");
            }

            var coupon = _mapper.Map<Coupon>(createCouponRequestDto);
            coupon.UsedCount = 0;
            coupon.SellerId = sellerId;

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully created a new coupon {CouponCode}" +
                " (CouponId: {CouponId}).", sellerId, coupon.CouponCode, coupon.CouponId);

            return new ApiResponseDto<CouponResponseDto>
            {
                Message = "Coupon created successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }

        public async Task<ApiResponseDto<CouponResponseDto>> UpdateCouponAsync(UpdateCouponRequestDto updateCouponRequestDto, int couponId, int sellerId)
        {
            if (updateCouponRequestDto.StartDate >= updateCouponRequestDto.EndDate)
            {
                _logger.LogWarning("Seller {SellerId} attempted to update CouponId {CouponId} with invalid dates.", sellerId, couponId);
                throw new BadRequestException("End date must be after the start date.");
            }

            var coupon = await _couponRepository.GetByIdAsync(couponId);

            if (coupon == null)
            {
                _logger.LogWarning("Seller {SellerId} attempted to update non-existent CouponId {CouponId}.", sellerId, couponId);
                throw new NotFoundException("The coupon does not exist.");
            }

            if (coupon.SellerId != sellerId)
            {
                _logger.LogWarning("Security Warning: Seller {SellerId} attempted to update CouponId {CouponId} belonging to another seller.", sellerId, couponId);
                throw new UnauthorizedException("You do not have permission to update this coupon.");
            }

            _mapper.Map(updateCouponRequestDto, coupon);

            _couponRepository.Update(coupon);
            await _couponRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully updated CouponId {CouponId}.", sellerId, couponId);

            return new ApiResponseDto<CouponResponseDto>
            {
                Message = "Coupon updated successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }

        public async Task<ApiResponseDto<string>> DeleteCouponAsync(int couponId, int sellerId)
        {
            var coupon = await _couponRepository.GetByIdAsync(couponId);

            if (coupon == null)
            {
                _logger.LogWarning("Seller {SellerId} attempted to delete non-existent CouponId {CouponId}.", sellerId, couponId);
                throw new NotFoundException("The coupon does not exist.");
            }

            if (coupon.SellerId != sellerId)
            {
                _logger.LogWarning("Security Warning: Seller {SellerId} attempted to delete CouponId {CouponId} belonging to another seller.", sellerId, couponId);
                throw new UnauthorizedException("You do not have permission to delete this coupon.");
            }

            _couponRepository.Delete(coupon);
            await _couponRepository.SaveChangesAsync();

            _logger.LogInformation("Seller {SellerId} successfully deleted CouponId {CouponId}.", sellerId, couponId);

            return new ApiResponseDto<string>
            {
                Message = "Coupon deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<CouponResponseDto>> GetCouponByCodeAsync(string couponCode)
        {
            var coupon = await _couponRepository.GetCouponByCodeAsync(couponCode);

            if (coupon == null)
            {
                _logger.LogWarning("Attempted to retrieve or use non-existent or invalid coupon code: {CouponCode}.", couponCode);
                throw new NotFoundException("Invalid coupon code.");
            }

            var currentTime = DateTime.UtcNow;

            if (currentTime < coupon.StartDate)
            {
                _logger.LogWarning("Attempted to use coupon {CouponCode} before its start date. (StartDate: {StartDate}).", couponCode, coupon.StartDate);
                throw new BadRequestException("This coupon is not valid yet.");
            }

            if (currentTime > coupon.EndDate)
            {
                _logger.LogWarning("Attempted to use expired coupon {CouponCode}. (EndDate: {EndDate}).", couponCode, coupon.EndDate);
                throw new BadRequestException("This coupon has expired.");
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                _logger.LogWarning("Attempted to use coupon {CouponCode}" +
                    " which has reached its maximum usage limit ({UsedCount}/{UsageLimit}).", couponCode, coupon.UsedCount, coupon.UsageLimit.Value);
                throw new BadRequestException("This coupon has reached its maximum usage limit.");
            }

            return new ApiResponseDto<CouponResponseDto>
            {
                Message = "Coupon is valid and retrieved successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }
    }
}