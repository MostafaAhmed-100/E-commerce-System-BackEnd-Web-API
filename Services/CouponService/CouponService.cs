using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Repository.SpecificRepository.CouponRepository;

namespace WebApplication1.Services.CouponService
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IMapper mapper)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        public async Task<ApiResponseDto<CouponResponseDto>> CreateCouponAsync(CreateCouponRequestDto createCouponRequestDto, int sellerId)
        {
            if (createCouponRequestDto.StartDate >= createCouponRequestDto.EndDate)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_DATES",
                    Data = null,
                    Message = "End date must be after the start date."
                };
            }

            var existingCoupon = await _couponRepository.GetCouponByCodeAsync(createCouponRequestDto.CouponCode);
            if (existingCoupon != null)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 409,
                    ErrorCode = "COUPON_CODE_EXISTS",
                    Data = null,
                    Message = "A coupon with this code already exists."
                };
            }

            var coupon = _mapper.Map<Coupon>(createCouponRequestDto);
            coupon.UsedCount = 0;
            coupon.SellerId = sellerId;

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();

            return new ApiResponseDto<CouponResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Coupon created successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }

        public async Task<ApiResponseDto<CouponResponseDto>> UpdateCouponAsync(UpdateCouponRequestDto updateCouponRequestDto, int couponId, int sellerId)
        {
            if (updateCouponRequestDto.StartDate >= updateCouponRequestDto.EndDate)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_DATES",
                    Data = null,
                    Message = "End date must be after the start date."
                };
            }

            var coupon = await _couponRepository.GetByIdAsync(couponId);

            if (coupon == null)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "COUPON_NOT_FOUND",
                    Data = null,
                    Message = "The coupon does not exist."
                };
            }

            if (coupon.SellerId != sellerId)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to update this coupon."
                };
            }
            _mapper.Map(updateCouponRequestDto, coupon);

            _couponRepository.Update(coupon);
            await _couponRepository.SaveChangesAsync();

            return new ApiResponseDto<CouponResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Coupon updated successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }

        public async Task<ApiResponseDto<string>> DeleteCouponAsync(int couponId, int sellerId)
        {
            var coupon = await _couponRepository.GetByIdAsync(couponId);

            if (coupon == null)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "COUPON_NOT_FOUND",
                    Data = null,
                    Message = "The coupon does not exist."
                };
            }

            if (coupon.SellerId != sellerId)
            {
                return new ApiResponseDto<string>
                {
                    IsSuccess = false,
                    StatusCode = 403,
                    ErrorCode = "UNAUTHORIZED_ACTION",
                    Data = null,
                    Message = "You do not have permission to delete this coupon."
                };
            }

            _couponRepository.Delete(coupon);
            await _couponRepository.SaveChangesAsync();

            return new ApiResponseDto<string>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Coupon deleted successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<CouponResponseDto>> GetCouponByCodeAsync(string couponCode)
        {
            var coupon = await _couponRepository.GetCouponByCodeAsync(couponCode);

            if (coupon == null)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    ErrorCode = "COUPON_NOT_FOUND",
                    Data = null,
                    Message = "Invalid coupon code."
                };
            }

            var currentTime = DateTime.UtcNow;

            if (currentTime < coupon.StartDate)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "COUPON_NOT_STARTED",
                    Data = null,
                    Message = "This coupon is not valid yet."
                };
            }

            if (currentTime > coupon.EndDate)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "COUPON_EXPIRED",
                    Data = null,
                    Message = "This coupon has expired."
                };
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return new ApiResponseDto<CouponResponseDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    ErrorCode = "COUPON_LIMIT_REACHED",
                    Data = null,
                    Message = "This coupon has reached its maximum usage limit."
                };
            }

            return new ApiResponseDto<CouponResponseDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                ErrorCode = "",
                Message = "Coupon is valid and retrieved successfully.",
                Data = _mapper.Map<CouponResponseDto>(coupon)
            };
        }
    }
}