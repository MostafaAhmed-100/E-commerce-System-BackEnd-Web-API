using AutoMapper;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;

namespace WebApplication1.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;

        public AccountService
        (
            ILogger<AccountService> logger,
            UserManager<User> userManager,
            IUnitOfWork uow,
            IMapper mapper
        )
        {
            _logger = logger;
            _userManager = userManager;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<string>> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto, int userId)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} account Not Found ", userId);
                    throw new NotFoundException("User not found");
                }

                var changedPassword = await _userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword, changePasswordRequestDto.NewPassword);
                if (changedPassword.Succeeded == false)
                {
                    await transaction .RollbackAsync();
                    var errors = string.Join(", ", changedPassword.Errors.Select(e => e.Description));
                    _logger.LogWarning("User {UserId} failed to change password due to An Error {errors}.", userId, errors);
                    throw new BadRequestException($"There is an Error {errors}");
                }

                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} changed their password successfully.", userId);
                return new ApiResponseDto<string>
                {
                    Message = "Your PassWord Has Been Changed successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while changing password for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> DeleteAccountAsync(int userId)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
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
                    var Buyer = await _uow.BuyerRepository.GetBuyerByUserId(userId);
                    if (Buyer != null)
                        Buyer.IsDeleted = true;
                }

                if (IsSeller)
                {
                    var Seller = await _uow.SellerRepository.GetSellerIdByUserId(userId);
                    if (Seller != null)
                        Seller.IsDeleted = true;
                }

                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} account locked and profile soft-deleted. IsBuyer: {IsBuyer}, IsSeller: {IsSeller}", userId, isBuyer, IsSeller);
                return new ApiResponseDto<string>
                {
                    Data = null,
                    Message = "User Deleted Succesfuly"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while deleting account for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<BuyerProfileResponseDto>> GetBuyerProfileAsync(int buyerId)
        {
            try
            {
                var buyer = await _uow.BuyerRepository.GetBuyerWithAddressesById(buyerId);
                if (buyer == null)
                {
                    _logger.LogWarning("Buyer {BuyerId} profile Not Found ", buyerId);
                    throw new NotFoundException("Buyer not found");
                }

                var buyerProfileDto = _mapper.Map<BuyerProfileResponseDto>(buyer);
                _logger.LogInformation("Buyer {BuyerId} profile retrieved successfully.", buyerId);
                return new ApiResponseDto<BuyerProfileResponseDto>
                {
                    Data = buyerProfileDto,
                    Message = "Buyer profile retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving buyer profile for Buyer {BuyerId}", buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileAsync(int sellerId)
        {
            try
            {
                var seller = await _uow.SellerRepository.GetSellerWithUserById(sellerId);
                if (seller == null)
                {
                    _logger.LogWarning("Seller {SellerId} profile Not Found ", sellerId);
                    throw new NotFoundException("Seller not found");
                }

                var sellerProfileDto = _mapper.Map<SellerProfileResponseDto>(seller);
                _logger.LogInformation("Seller {SellerId} profile retrieved successfully.", sellerId);
                return new ApiResponseDto<SellerProfileResponseDto>
                {
                    Data = sellerProfileDto,
                    Message = "Seller profile retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving seller profile for Seller {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<SellerProfileResponseDto>> GetSellerProfileByNationalIdAsync(string nationalId)
        {
            try
            {
                var seller = await _uow.SellerRepository.GetSellerByNationalId(nationalId);
                if (seller == null)
                {
                    _logger.LogWarning("Seller Serch nationalId by {nationalId} profile Not Found ", nationalId);
                    throw new NotFoundException("Seller not found");
                }

                var sellerProfileDto = _mapper.Map<SellerProfileResponseDto>(seller);
                _logger.LogInformation("Seller {nationalId} profile retrieved successfully.", nationalId);
                return new ApiResponseDto<SellerProfileResponseDto>
                {
                    Data = sellerProfileDto,
                    Message = "Seller profile retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving seller profile by National ID {NationalId}", nationalId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> UpdateBuyerProfileAsync(int buyerId, UpdateBuyerProfileRequestDto updateBuyerProfile)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                var buyer = await _uow.BuyerRepository.GetBuyerWithAddressesById(buyerId);
                if (buyer == null)
                {
                    _logger.LogWarning("Buyer {BuyerId} profile Not Found for update.", buyerId);
                    throw new NotFoundException("Buyer not found");
                }

                _mapper.Map(updateBuyerProfile, buyer.User);

                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Buyer {BuyerId} profile updated successfully.", buyerId);
                return new ApiResponseDto<string>
                {
                    Message = "Buyer profile updated successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating buyer profile for Buyer {BuyerId}", buyerId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> UpdateSellerProfileAsync(int sellerId, UpdateSellerProfileRequestDto updateSellerProfile)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                var seller = await _uow.SellerRepository.GetSellerWithUserById(sellerId);
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

                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Seller {SellerId} profile updated successfully.", sellerId);
                return new ApiResponseDto<string>
                {
                    Message = "Seller profile updated successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating seller profile for Seller {SellerId}", sellerId);
                throw;
            }
        }
    }
}