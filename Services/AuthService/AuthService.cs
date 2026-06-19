using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.SpecificRepository.BuyerRepository;
using WebApplication1.Repository.SpecificRepository.SellerRepository;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IBuyerRepository _buyerRepository;
        private readonly ISellerRepository _sellerRepository;

        public AuthService
        (
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            IBuyerRepository buyerRepository,
            ISellerRepository sellerRepository
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _buyerRepository = buyerRepository;
            _sellerRepository = sellerRepository;
        }

        private string GenerateJwtToken(string role, User user, int profileId)
        {
            var Clames = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.Email , user.Email!),
                new Claim(ClaimTypes.Role , role),
                new Claim("ProfileId", profileId.ToString())
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var Credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: Clames,
                    signingCredentials: Credentials,
                    expires: DateTime.UtcNow.AddDays(2)
                );
            var JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return JwtSecurityTokenHandler.WriteToken(Token);
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var User = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            if (User == null || !await _userManager.CheckPasswordAsync(User, loginRequestDto.Password))
                throw new UnauthorizedException("Invalid email or password.");

            var userRoles = await _userManager.GetRolesAsync(User);
            var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Buyer;
            int profileId = 0;

            if (primaryRole == AppRoles.Buyer)
            {
                var buyer = await _buyerRepository.GetBuyerByUserId(User.Id);
                if (buyer == null)
                    throw new NotFoundException("Profile not found or corrupted");

                profileId = buyer.BuyerId;
            }
            else if (primaryRole == AppRoles.Seller)
            {
                var seller = await _sellerRepository.GetSellerIdByUserId(User.Id);
                if (seller == null)
                    throw new NotFoundException("Profile not found or corrupted");

                profileId = seller.SellerId;
            }

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(primaryRole, User, profileId),
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Email = User.Email,
                    Role = primaryRole
                },
                Message = "Login successful."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var Email = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (Email != null)
                throw new ConflictException("That Email Already Has An Account");

            var user = new User
            {
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.UserName,
            };
            var Result = await _userManager.CreateAsync(user, registerRequestDto.Password);

            if (!Result.Succeeded)
            {
                var errors = string.Join(", ", Result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            var Buyer = new Buyer
            {
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                LoyaltyPoints = 0,
                User = user,
                UserId = user.Id,
            };
            await _buyerRepository.AddAsync(Buyer);
            await _buyerRepository.SaveChangesAsync();

            if (!await _roleManager.RoleExistsAsync(AppRoles.Buyer))
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = AppRoles.Buyer,
                    Description = "Standard buyer role",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
            await _userManager.AddToRoleAsync(user, AppRoles.Buyer);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(AppRoles.Buyer, user, Buyer.BuyerId),
                    Expiration = DateTime.UtcNow.AddHours(24),
                    Email = user.Email,
                    Role = AppRoles.Buyer
                },
                Message = "User registered successfully as a Buyer."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAdminAsync(RegisterAdminRequestDto registerAdminRequestDto)
        {
            if (registerAdminRequestDto.AdminSecretCode != _configuration["AdminSecretKey"])
                throw new UnauthorizedException("The AdminSecretKey Is Wrong");

            var Email = await _userManager.FindByEmailAsync(registerAdminRequestDto.AdminEmail);
            if (Email != null)
                throw new ConflictException("That Email Already Has An Account");

            var user = new User
            {
                Email = registerAdminRequestDto.AdminEmail,
                UserName = registerAdminRequestDto.UserName,
            };
            var Result = await _userManager.CreateAsync(user, registerAdminRequestDto.Password);

            if (!Result.Succeeded)
            {
                var errors = string.Join(", ", Result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync(AppRoles.Admin))
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = AppRoles.Admin,
                    Description = "System Administrator role",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
            await _userManager.AddToRoleAsync(user, AppRoles.Admin);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(AppRoles.Admin, user, 0),
                    Expiration = DateTime.UtcNow.AddHours(100),
                    Email = user.Email,
                    Role = AppRoles.Admin
                },
                Message = "User registered successfully as a Admin."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterSellerAsync(RegisterSellerRequestDto registerSellerRequestDto)
        {
            var Email = await _userManager.FindByEmailAsync(registerSellerRequestDto.SellerEmail);
            if (Email != null)
                throw new ConflictException("That Email Already Has An Account");

            var user = new User
            {
                Email = registerSellerRequestDto.SellerEmail,
                UserName = registerSellerRequestDto.UserName,
            };
            var Result = await _userManager.CreateAsync(user, registerSellerRequestDto.Password);

            if (!Result.Succeeded)
            {
                var errors = string.Join(", ", Result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync(AppRoles.Seller))
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = AppRoles.Seller,
                    Description = "Seller role for store owners",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
            var Seller = new Seller
            {
                StoreName = registerSellerRequestDto.StoreName,
                CreatedAt = DateTime.UtcNow,
                BankAccountNumber = registerSellerRequestDto.BankAccountNumber,
                BankName = registerSellerRequestDto.BankName,
                IsDeleted = false,
                PhoneNumber = registerSellerRequestDto.PhoneNumber,
                TaxNumber = registerSellerRequestDto.TaxNumber,
                User = user,
                UserId = user.Id,
            };
            await _sellerRepository.AddAsync(Seller);
            await _sellerRepository.SaveChangesAsync();
            await _userManager.AddToRoleAsync(user, AppRoles.Seller);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(AppRoles.Seller, user, Seller.SellerId),
                    Expiration = DateTime.UtcNow.AddHours(30),
                    Email = user.Email,
                    Role = AppRoles.Seller
                },
                Message = "User registered successfully as a Seller."
            };
        }
    }
}