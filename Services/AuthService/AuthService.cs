using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.DTOS.Response_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Exceptions;
using WebApplication1.Repository.UnitOfWork;
using WebApplication1.Services.EmailService;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService
        (
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IEmailService emailService,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        private string GenerateJwtToken(string role, User user, int profileId)
        {
            var clames = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role),
                new Claim("ProfileId", profileId.ToString())
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: clames,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddMinutes(15)
            );
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDto.Password))
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}.", loginRequestDto.Email);
                    throw new UnauthorizedException("Invalid email or password.");
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning("User {UserId} attempted to log in without confirming their email.", user.Id);
                    throw new UnauthorizedException("رجاءً تأكيد بريدك الإلكتروني أولاً عبر الرابط المرسل إليك.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Buyer;
                int profileId = 0;

                if (primaryRole == AppRoles.Buyer)
                {
                    var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(user.Id);
                    if (buyer == null)
                    {
                        _logger.LogWarning("Data inconsistency: Buyer profile missing for User {UserId}.", user.Id);
                        throw new NotFoundException("Profile not found or corrupted");
                    }
                    profileId = buyer.BuyerId;
                }
                else if (primaryRole == AppRoles.Seller)
                {
                    var seller = await _unitOfWork.SellerRepository.GetSellerIdByUserId(user.Id);
                    if (seller == null)
                    {
                        _logger.LogWarning("Data inconsistency: Seller profile missing for User {UserId}.", user.Id);
                        throw new NotFoundException("Profile not found or corrupted");
                    }
                    profileId = seller.SellerId;
                }

                _logger.LogInformation("User {UserId} logged in successfully as {Role}.", user.Id, primaryRole);

                var randomTokenString = GenerateRefreshTokenString();
                var refreshtoken = new RefreshToken
                {
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    RevokedOn = null,
                    Token = randomTokenString,
                    UserId = user.Id
                };

                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshtoken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDto<AuthResponseDto>
                {
                    Data = new AuthResponseDto
                    {
                        Token = GenerateJwtToken(primaryRole, user, profileId),
                        Expiration = DateTime.UtcNow.AddHours(1),
                        Email = user.Email,
                        Role = primaryRole,
                        RefreshToken = refreshtoken.Token,
                        RefreshTokenExpiration = refreshtoken.ExpiresOn
                    },
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while LoginAsync for User Email {Email}", loginRequestDto.Email);
                throw;
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: Attempt to register with already existing email {Email}.", registerRequestDto.Email);
                    throw new ConflictException("That Email Already Has An Account");
                }

                var user = new User
                {
                    Email = registerRequestDto.Email,
                    UserName = registerRequestDto.UserName,
                };
                var result = await _userManager.CreateAsync(user, registerRequestDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("User creation failed for {Email}. Errors: {Errors}", registerRequestDto.Email, errors);
                    throw new BadRequestException($"Failed to create user: {errors}");
                }
                await SentEmailConfirmation(user, registerRequestDto.Email);

                var buyer = new Buyer
                {
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    LoyaltyPoints = 0,
                    User = user,
                    UserId = user.Id,
                };
                await _unitOfWork.BuyerRepository.AddAsync(buyer);
                await _unitOfWork.SaveChangesAsync();

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

                _logger.LogInformation("User {UserId} registered successfully as a Buyer.", user.Id);

                var randomTokenString = GenerateRefreshTokenString();
                var refreshtoken = new RefreshToken
                {
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    RevokedOn = null,
                    Token = randomTokenString,
                    UserId = user.Id
                };
                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshtoken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDto<AuthResponseDto>
                {
                    Data = new AuthResponseDto
                    {
                        Token = GenerateJwtToken(AppRoles.Buyer, user, buyer.BuyerId),
                        Expiration = DateTime.UtcNow.AddHours(24),
                        Email = user.Email,
                        Role = AppRoles.Buyer,
                        RefreshToken = refreshtoken.Token,
                        RefreshTokenExpiration = refreshtoken.ExpiresOn,
                    },
                    Message = "User registered successfully. Please check your email to confirm your account."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while RegisterAsync for User {Email}", registerRequestDto.Email);
                throw;
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterSellerAsync(RegisterSellerRequestDto registerSellerRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var User = await _userManager.FindByEmailAsync(registerSellerRequestDto.SellerEmail);
                if (User != null)
                {
                    _logger.LogWarning("Seller Registration failed: Attempt to register with already existing email {Email}.", registerSellerRequestDto.SellerEmail);
                    throw new ConflictException("That Email Already Has An Account");
                }

                var user = new User
                {
                    Email = registerSellerRequestDto.SellerEmail,
                    UserName = registerSellerRequestDto.UserName,
                };
                var result = await _userManager.CreateAsync(user, registerSellerRequestDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Seller creation failed for {Email}. Errors: {Errors}", registerSellerRequestDto.SellerEmail, errors);
                    throw new BadRequestException($"Failed to create user: {errors}");
                }
                await SentEmailConfirmation(user, registerSellerRequestDto.SellerEmail);

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

                var seller = new Seller
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
                await _unitOfWork.SellerRepository.AddAsync(seller);
                await _unitOfWork.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, AppRoles.Seller);

                _logger.LogInformation("Seller User {UserId} registered successfully for Store {StoreName}.", user.Id, seller.StoreName);

                var randomTokenString = GenerateRefreshTokenString();
                var refreshtoken = new RefreshToken
                {
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    RevokedOn = null,
                    Token = randomTokenString,
                    UserId = user.Id
                };
                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshtoken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDto<AuthResponseDto>
                {
                    Data = new AuthResponseDto
                    {
                        Token = GenerateJwtToken(AppRoles.Seller, user, seller.SellerId),
                        Expiration = DateTime.UtcNow.AddHours(30),
                        Email = user.Email,
                        Role = AppRoles.Seller,
                        RefreshToken = refreshtoken.Token,
                        RefreshTokenExpiration = refreshtoken.ExpiresOn,
                    },
                    Message = "Seller registered successfully. Please check your email to confirm your account."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while RegisterSellerAsync for Email {Email}", registerSellerRequestDto.SellerEmail);
                throw;
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterAdminAsync(RegisterAdminRequestDto registerAdminRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (registerAdminRequestDto.AdminSecretCode != _configuration["AdminSecretKey"])
                {
                    _logger.LogWarning("SECURITY ALERT: Failed attempt to register Admin for email {Email} using an invalid secret code.", registerAdminRequestDto.AdminEmail);
                    throw new UnauthorizedException("The AdminSecretKey Is Wrong");
                }

                var User = await _userManager.FindByEmailAsync(registerAdminRequestDto.AdminEmail);
                if (User != null)
                {
                    _logger.LogWarning("Admin Registration failed: Attempt to register with already existing email {Email}.", registerAdminRequestDto.AdminEmail);
                    throw new ConflictException("That Email Already Has An Account");
                }

                var user = new User
                {
                    Email = registerAdminRequestDto.AdminEmail,
                    UserName = registerAdminRequestDto.UserName,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, registerAdminRequestDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Admin creation failed for {Email}. Errors: {Errors}", registerAdminRequestDto.AdminEmail, errors);
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

                _logger.LogInformation("Admin User {UserId} registered successfully.", user.Id);

                var randomTokenString = GenerateRefreshTokenString();
                var refreshtoken = new RefreshToken
                {
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    RevokedOn = null,
                    Token = randomTokenString,
                    UserId = user.Id
                };

                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshtoken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDto<AuthResponseDto>
                {
                    Data = new AuthResponseDto
                    {
                        Token = GenerateJwtToken(AppRoles.Admin, user, 0),
                        Expiration = DateTime.UtcNow.AddHours(100),
                        Email = user.Email,
                        Role = AppRoles.Admin,
                        RefreshToken = refreshtoken.Token,
                        RefreshTokenExpiration = refreshtoken.ExpiresOn
                    },
                    Message = "User registered successfully as an Admin."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while RegisterAdminAsync for Email {Email}", registerAdminRequestDto.AdminEmail);
                throw;
            }
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(refreshTokenRequestDto.Token);

                if (existingToken == null || !existingToken.IsActive)
                {
                    _logger.LogWarning("Security Warning: Attempted to use an invalid, expired, or revoked refresh token.");
                    throw new UnauthorizedException("Invalid or Expired Refresh Token. Please login again.");
                }

                existingToken.RevokedOn = DateTime.UtcNow;
                _unitOfWork.RefreshTokenRepository.Update(existingToken);

                var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
                if (user == null)
                {
                    throw new NotFoundException("User associated with this token no longer exists.");
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Buyer;
                int profileId = 0;

                if (primaryRole == AppRoles.Buyer)
                {
                    var buyer = await _unitOfWork.BuyerRepository.GetBuyerByUserId(user.Id);
                    if (buyer != null) profileId = buyer.BuyerId;
                }
                else if (primaryRole == AppRoles.Seller)
                {
                    var seller = await _unitOfWork.SellerRepository.GetSellerIdByUserId(user.Id);
                    if (seller != null) profileId = seller.SellerId;
                }

                var newJwtToken = GenerateJwtToken(primaryRole, user, profileId);
                var newRefreshTokenString = GenerateRefreshTokenString();

                var newRefreshToken = new RefreshToken
                {
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(7),
                    RevokedOn = null,
                    Token = newRefreshTokenString,
                    UserId = user.Id
                };

                await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Refresh token successfully exchanged for User {UserId}.", user.Id);

                return new ApiResponseDto<AuthResponseDto>
                {
                    Data = new AuthResponseDto
                    {
                        Token = newJwtToken,
                        Expiration = DateTime.UtcNow.AddHours(1),
                        Email = user.Email,
                        Role = primaryRole,
                        RefreshToken = newRefreshToken.Token,
                        RefreshTokenExpiration = newRefreshToken.ExpiresOn
                    },
                    Message = "Token refreshed successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while refreshing token");
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> ConfirmEmailAsync(int userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Email confirmation failed: User ID {UserId} not found.", userId);
                    throw new NotFoundException("User not found.");
                }

                var decodedTokenBytes = WebEncoders.Base64UrlDecode(code);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Email confirmation failed for User {UserId}. Errors: {Errors}", userId, errors);
                    throw new BadRequestException($"Invalid email confirmation token: {errors}");
                }

                _logger.LogInformation("User {UserId} successfully confirmed their email.", userId);

                return new ApiResponseDto<string>
                {
                    Data = null,
                    Message = "Your email has been confirmed successfully! You can now log in."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while confirming email for User {UserId}", userId);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            try
            {
                var User = await _userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
                if (User == null)
                {
                    _logger.LogWarning("try Forgot Password failed: Attempt to rest Password for a non existing email {Email}.", forgotPasswordRequestDto.Email);
                    throw new BadRequestException("That Email does not Have An Account");
                }
                await SentForgotPassword(User, forgotPasswordRequestDto.Email);
                return new ApiResponseDto<string>
                {
                    Data = null,
                    Message = "Email Has Been Sent"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ForgotPassword for Email {Email}", forgotPasswordRequestDto.Email);
                throw;
            }
        }

        public async Task<ApiResponseDto<string>> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(requestDto.Email);
                if (user == null)
                {
                    throw new BadRequestException("Invalid Request.");
                }

                var decodedTokenBytes = WebEncoders.Base64UrlDecode(requestDto.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, requestDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for {Email}. Errors: {Errors}", requestDto.Email, errors);
                    throw new BadRequestException($"Failed to reset password: {errors}");
                }

                _logger.LogInformation("Password for {Email} has been reset successfully.", requestDto.Email);

                return new ApiResponseDto<string>
                {
                    Data = null,
                    Message = "Your password has been reset successfully. You can now log in."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during ResetPassword for Email {Email}", requestDto.Email);
                throw;
            }
        }

        #region Private Functions
        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task SentEmailConfirmation(User user, string Email)
        {
            var emailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmation));
            string link = $"https://localhost:7132/api/Auth/Confirm-Email?userId={user.Id}&code={confirmationToken}";

            string emailBody = $@"
            <!DOCTYPE html>
            <html lang='ar' dir='rtl'>
            <head>
                <meta charset='UTF-8'>
            </head>
            <body style='font-family: Arial, sans-serif; text-align: center; padding: 30px; background-color: #f9f9f9;'>
                <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 500px; margin: auto; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>أهلاً بك يا هندسة! 🎉</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.5;'>
                        سعداء بانضمام متجرك إلينا. لتفعيل حسابك والبدء، يرجى الضغط على الزرار أدناه:
                    </p>
                    <br>
                    <a href='{link}' style='background-color: #512BD4; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                        تأكيد الحساب
                    </a>
                    <br><br>
                    <p style='color: #999; font-size: 12px;'>
                        لو واجهت أي مشكلة، تقدر تتجاهل الرسالة دي.
                    </p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(Email, "Confirm your email", emailBody);
        }

        private async Task SentForgotPassword(User user, string Email)
        {
            var PasswordReset = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(PasswordReset));
            string link = $"https://localhost:7132/reset-password?email={user.Email}&token={confirmationToken}";

            string emailBody = $@"
            <!DOCTYPE html>
            <html lang='ar' dir='rtl'>
            <head>
                <meta charset='UTF-8'>
            </head>
            <body style='font-family: Arial, sans-serif; text-align: center; padding: 30px; background-color: #f9f9f9;'>
                <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 500px; margin: auto; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>إعادة تعيين كلمة المرور 🔒</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.5;'>
                        لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. لتعيين كلمة مرور جديدة، يرجى الضغط على الزرار أدناه:
                    </p>
                    <br>
                    <a href='{link}' style='background-color: #512BD4; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                        تغيير كلمة المرور
                    </a>
                    <br><br>
                    <p style='color: #999; font-size: 12px;'>
                        إذا لم تكن أنت من طلب هذا التغيير، يمكنك تجاهل هذه الرسالة بأمان ولن يتم تغيير كلمة المرور.
                    </p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(Email, "Confirm your Password", emailBody);
        }
        #endregion
    }
}