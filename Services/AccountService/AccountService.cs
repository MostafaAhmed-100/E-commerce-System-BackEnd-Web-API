using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Migrations;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;

namespace WebApplication1.Services.AccountService
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<User> _userManager;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        public AccountService
        (
            ILogger<AccountService> logger,
            UserManager<User> userManager,
            IBuyerRepository buyerRepository,
            ISellerRepository sellerRepository,
            IMapper mapper
        )
        {
            _logger = logger;
            _userManager = userManager;
            _buyerRepository = buyerRepository;
            _sellerRepository = sellerRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<string>> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {UserId} account Not Found ", userId);
                throw new NotFoundException("User not found");
            }
            var changedPassword = await _userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword , changePasswordRequestDto.NewPassword);
            if (changedPassword.Succeeded == false)
            {
                var errors = string.Join(", ", changedPassword.Errors.Select(e => e.Description));
                _logger.LogWarning("User {UserId} failed to change password due to An Error {errors}.", userId, errors);
                throw new BadRequestException($"There is an Error {errors}");
            }
            else
            {
                _logger.LogInformation("User {UserId} changed their password successfully.", userId);
                return new ApiResponseDto<string>
                {
                    Message = "Your PassWord Has Been Changed successfully",
                    Data = null
                };
            }
        }

        public async Task<ApiResponseDto<string>> DeleteAccountAsync(int userId)
        {
            var User = await _userManager.FindByIdAsync(userId.ToString());
            if (User == null)
            {
                _logger.LogWarning("User {UserId} account Not Found ", userId);
                throw new NotFoundException("User not found");
            }
            await _userManager.SetLockoutEnabledAsync(User, enabled: true);
            await _userManager.SetLockoutEndDateAsync(User, DateTimeOffset.MaxValue);
            bool isBuyer = await _userManager.IsInRoleAsync(User, "Buyer");
            bool IsSeller = await _userManager.IsInRoleAsync(User, "Seller");
            if (isBuyer)
            {
                var Buyer = await _buyerRepository.GetBuyerByUserId(userId);
                Buyer.IsDeleted = true;
                await _buyerRepository.SaveChangesAsync();
            }
            if (IsSeller)
            {
                var Seller = await _sellerRepository.GetSellerIdByUserId(userId);
                Seller.IsDeleted = true;
                await _sellerRepository.SaveChangesAsync();
            }
            _logger.LogInformation("User {UserId} account locked and profile soft-deleted. IsBuyer: {IsBuyer}, IsSeller: {IsSeller}", userId, isBuyer, IsSeller);
            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "User Deleted Succesfuly"
            };
        }
        public async Task<ApiResponseDto<BuyerProfileResponseDto>> GetBuyerProfileAsync(int buyerId)
        {
            var buyer = await _buyerRepository.GetBuyerWithAddressesById(buyerId);
            if (buyer == null)
            {
                _logger.LogWarning("Buyer {BuyerId} profile Not Found ", buyerId);
                throw new NotFoundException("Buyer not found");
            }
            else
            {
                var buyerProfileDto = _mapper.Map<BuyerProfileResponseDto>(buyer);
                _logger.LogInformation("Buyer {BuyerId} profile retrieved successfully.", buyerId);
                return new ApiResponseDto<BuyerProfileResponseDto>
                {
                    Data = buyerProfileDto,
                    Message = "Buyer profile retrieved successfully"
                };
            }
        }

        public async Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileAsync(int sellerId)
        {
            var seller = await _sellerRepository.GetSellerWithUserById(sellerId);
            if (seller == null)
            {
                _logger.LogWarning("Seller {SellerId} profile Not Found ", sellerId);
                throw new NotFoundException("Seller not found");
            }
            else
            {
                var sellerProfileDto = _mapper.Map<SellerProfileResponseDto>(seller);
                _logger.LogInformation("Seller {SellerId} profile retrieved successfully.", sellerId);
                return new ApiResponseDto<SellerProfileResponseDto>
                {
                    Data = sellerProfileDto,
                    Message = "Seller profile retrieved successfully"
                };
            }
        }

        public async Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileByNationalIdAsync(string nationalId)
        {
            var seller = await _sellerRepository.GetSellerByNationalId(nationalId);
            if (seller == null)
            {
                _logger.LogWarning("Seller Serch nationalId by {nationalId} profile Not Found ", nationalId);
                throw new NotFoundException("Seller not found");
            }
            else
            {
                var sellerProfileDto = _mapper.Map<SellerProfileResponseDto>(seller);
                _logger.LogInformation("Seller {nationalId} profile retrieved successfully.", nationalId);
                return new ApiResponseDto<SellerProfileResponseDto>
                {
                    Data = sellerProfileDto,
                    Message = "Seller profile retrieved successfully"
                };
            }
        }

        public async Task<ApiResponseDto<string>> UpdateBuyerProfileAsync(int buyerId, UpdateBuyerProfileRequestDto updateBuyerProfile)
        {
            var buyer = await _buyerRepository.GetBuyerWithAddressesById(buyerId);
            if (buyer == null)
            {
                _logger.LogWarning("Buyer {BuyerId} profile Not Found for update.", buyerId);
                throw new NotFoundException("Buyer not found");
            }

            _mapper.Map(updateBuyerProfile, buyer.User);

            await _buyerRepository.SaveChangesAsync();
            _logger.LogInformation("Buyer {BuyerId} profile updated successfully.", buyerId);

            return new ApiResponseDto<string>
            {
                Message = "Buyer profile updated successfully",
                Data = null
            };
        }

        public async Task<ApiResponseDto<string>> UpdateSellerProfileAsync(int sellerId, UpdateSellerProfileRequestDto updateSellerProfile)
        {
            var seller = await _sellerRepository.GetSellerWithUserById(sellerId);
            if (seller == null)
            {
                _logger.LogWarning("Seller {SellerId} profile Not Found for update.", sellerId);
                throw new NotFoundException("Seller not found");
            }

            _mapper.Map(updateSellerProfile, seller);

            if (seller.User != null)
            {
                seller.User.PhoneNumber = updateSellerProfile.SellerPhoneNumber;
            }

            await _sellerRepository.SaveChangesAsync();
            _logger.LogInformation("Seller {SellerId} profile updated successfully.", sellerId);

            return new ApiResponseDto<string>
            {
                Message = "Seller profile updated successfully",
                Data = null
            };
        }
    }
}