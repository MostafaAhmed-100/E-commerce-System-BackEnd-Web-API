using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.DTOS.Shared.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.CouponService
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponService> _logger;

        public CouponService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CouponService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<CouponResponseDto>> CreateCouponAsync(CreateCouponRequestDto createCouponRequestDto, int sellerId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (createCouponRequestDto.StartDate >= createCouponRequestDto.EndDate)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to create a coupon with invalid dates (StartDate: {StartDate}, EndDate: {EndDate}).", sellerId, createCouponRequestDto.StartDate, createCouponRequestDto.EndDate);
                    throw new BadRequestException("End date must be after the start date.");
                }

                var existingCoupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(createCouponRequestDto.CouponCode);
                if (existingCoupon != null)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to create a coupon with an already existing code: {CouponCode}.", sellerId, createCouponRequestDto.CouponCode);
                    throw new ConflictException("A coupon with this code already exists.");
                }

                var coupon = _mapper.Map<Coupon>(createCouponRequestDto);
                coupon.UsedCount = 0;
                coupon.SellerId = sellerId;

                await _unitOfWork.CouponRepository.AddAsync(coupon);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully created a new coupon {CouponCode} (CouponId: {CouponId}).", sellerId, coupon.CouponCode, coupon.CouponId);

                return new ApiResponseDto<CouponResponseDto>
                {
                    Message = "Coupon created successfully.",
                    Data = _mapper.Map<CouponResponseDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating coupon for Seller {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<CouponResponseDto>> UpdateCouponAsync(UpdateCouponRequestDto updateCouponRequestDto, int couponId, int sellerId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (updateCouponRequestDto.StartDate >= updateCouponRequestDto.EndDate)
                {
                    _logger.LogWarning("Seller {SellerId} attempted to update CouponId {CouponId} with invalid dates.", sellerId, couponId);
                    throw new BadRequestException("End date must be after the start date.");
                }

                var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(couponId);

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

                _unitOfWork.CouponRepository.Update(coupon);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully updated CouponId {CouponId}.", sellerId, couponId);

                return new ApiResponseDto<CouponResponseDto>
                {
                    Message = "Coupon updated successfully.",
                    Data = _mapper.Map<CouponResponseDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating CouponId {CouponId} for Seller {SellerId}", couponId, sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteCouponAsync(int couponId, int sellerId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var coupon = await _unitOfWork.CouponRepository.GetByIdAsync(couponId);

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

                _unitOfWork.CouponRepository.Delete(coupon);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} successfully deleted CouponId {CouponId}.", sellerId, couponId);

                return new ApiResponseDto<string>
                {
                    Message = "Coupon deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting CouponId {CouponId} for Seller {SellerId}", couponId, sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<CouponResponseDto>> GetCouponByCodeAsync(string couponCode)
        {
            try
            {
                var coupon = await _unitOfWork.CouponRepository.GetCouponByCodeAsync(couponCode);

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
                    _logger.LogWarning("Attempted to use coupon {CouponCode} which has reached its maximum usage limit ({UsedCount}/{UsageLimit}).", couponCode, coupon.UsedCount, coupon.UsageLimit.Value);
                    throw new BadRequestException("This coupon has reached its maximum usage limit.");
                }

                return new ApiResponseDto<CouponResponseDto>
                {
                    Message = "Coupon is valid and retrieved successfully.",
                    Data = _mapper.Map<CouponResponseDto>(coupon)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving coupon by code {CouponCode}", couponCode);
                throw;
            }
        }
    }
}