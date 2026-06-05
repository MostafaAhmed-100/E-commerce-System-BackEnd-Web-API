using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;

namespace WebApplication1.Services.CouponService
{
    public interface ICouponService
    {
        Task<ApiResponseDto<CouponResponseDto>> CreateCouponAsync(CreateCouponRequestDto createCouponRequestDto, int sellerId);
        Task<ApiResponseDto<CouponResponseDto>> UpdateCouponAsync(UpdateCouponRequestDto updateCouponRequestDto, int couponId, int sellerId);
        Task<ApiResponseDto<string>> DeleteCouponAsync(int couponId, int sellerId);
        Task<ApiResponseDto<CouponResponseDto>> GetCouponByCodeAsync(string couponCode);
    }
}