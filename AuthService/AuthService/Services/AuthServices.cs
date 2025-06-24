using AuthService.DTOs;
using AuthService.Entities;
using AuthService.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace AuthService.Services
{
    public class AuthServices : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICustomLogger<AuthServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PasswordOptions _passwordOptions;
        private readonly ITopicProducer<UserRegisteredEvent> _userRegisteredProducer;
        private readonly ITopicProducer<UnLockUserEvent> _unLockUserProducer;

        public AuthServices(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, ICustomLogger<AuthServices> logger,
            IConfiguration configuration, ISendMailService sendMailService, IRefreshTokenService refreshTokenService, IHttpClientFactory httpClientFactory, IOptions<IdentityOptions> passwordOptions,
            ITopicProducer<UserRegisteredEvent> userRegisteredProducer, ITopicProducer<UnLockUserEvent> unLockUserProducer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _configuration = configuration;
            _sendMailService = sendMailService;
            _refreshTokenService = refreshTokenService;
            _httpClientFactory = httpClientFactory;
            _passwordOptions = passwordOptions.Value.Password;
            _userRegisteredProducer = userRegisteredProducer;
            _unLockUserProducer = unLockUserProducer;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            ApplicationUser? user = null;
            try
            {
                var existingUser = await _userManager.FindByNameAsync(dto.UserName) ?? await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    throw new HandleException("Email is already registered.", 400);
                }

                user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    _logger.Warn("Failed to create user. Reasons: {@Reasons}", null, null, errors);
                    throw new HandleException("User registration failed.", 400, errors);
                }

                await _userManager.AddToRoleAsync(user, "User");
                _logger.Info("User {Email} created in AuthService DB.", null, null, user.Email);

                // Call User Service to create user profile using HTTP request
                //await CallUserServiceToCreateProfile(user, dto);
                var roles = await GetUserRolesAsync(user.Id.ToString());
                _logger.Debug("User's roles: {roles}", propertyValues: roles);
                // Publish event to create user profile in User Service using MassTransit
                await _userRegisteredProducer.Produce(new UserRegisteredEvent
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email!,
                    FirstName = dto.FirstName ?? string.Empty,
                    LastName = dto.LastName ?? string.Empty,
                    PhoneNumber = dto.PhoneNumber ?? string.Empty,
                    DateOfBirth = dto.DateOfBirth,
                    Address = dto.Address ?? string.Empty,
                    Gender = dto.Gender,
                    Roles = roles,
                });

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";
                var subject = "Confirm your email";
                var body = $"<p>Hello {user.UserName},</p>" +
                           $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your email address.</p>";
                await _sendMailService.SendEmailAsync(user.Email!, subject, body, isHtml: true);
                _logger.Debug("Confirm account for user ID: {ID} with token: {token}", null, null, user.Id, emailToken);
                //_logger.Info("User {Email} registered successfully and confirmation email sent.", user.Email);

                return null;
            }
            catch (HandleException)
            {
                if (user != null && await _userManager.FindByEmailAsync(user.Email ?? string.Empty) != null)
                {
                    await _userManager.DeleteAsync(user);
                    _logger.Warn("User {Email} created in AuthService was rolled back due to subsequent error.", null, null, user.Email!);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (user != null && await _userManager.FindByNameAsync(user.UserName ?? string.Empty) != null)
                {
                    await _userManager.DeleteAsync(user);
                    _logger.Warn("User {Email} created in AuthService was rolled back due to unexpected error.", null, null, user.Email!);
                }
                _logger.Error("Unexpected error in RegisterAsync for {Email}", ex, null, null, dto.Email);
                throw new HandleException("An unexpected error occurred during registration.", 500);
            }
        }

        public async Task CallUserServiceToCreateProfile(ApplicationUser user, RegisterDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["UserService:BaseUrl"]!);
            _logger.Debug("URL for User Service: {Url}", null, null, client.BaseAddress);

            var internalToken = _jwtTokenGenerator.GenerateInternalServiceToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToken);

            var createUserRequest = new UserRegisteredEvent
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email!,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                PhoneNumber = dto.PhoneNumber ?? string.Empty,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address ?? string.Empty,
            };

            try
            {
                var url = $"{client.BaseAddress}api/v1/users";
                _logger.Debug("Calling User Service to create profile for user {UserId} ({Email}) at {Url}", null, null, user.Id, user.Email!, url);
                var response = await client.PostAsJsonAsync("api/v1/users", createUserRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error("Failed to create user profile in user service.");
                    throw new HttpRequestException($"UserService responded with status {response.StatusCode}: {errorContent}");
                }
                _logger.Debug("User profile created successfully for user {UserId} ({Email}) in UserService.", null, null, user.Id, user.Email!);
            }
            catch (HttpRequestException)
            {
                _logger.Error("Network or HTTP error calling User Service");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error calling user service to create profile for user {UserId} ({Email}).", ex, null, null, user.Id, user.Email!);
                throw;
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto, string clientIp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    _logger.Warn("Login failed. Reason: User not found for email {Email}", null, null, dto.Email);
                    throw new HandleException("Invalid credentials.", 401, new List<string> { "Email or password incorrect." });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                {
                    if (result.IsNotAllowed)
                    {
                        var errors = new List<string>();

                        if (!user.EmailConfirmed)
                            errors.Add("Email is not confirmed.");

                        if (!user.PhoneNumberConfirmed)
                            errors.Add("Phone number is not confirmed.");

                        _logger.Warn("Login not allowed for user {Email}. Reasons: {@Reasons}", null, null, dto.Email, errors);

                        throw new HandleException("Login not allowed.", 403, errors);
                    }

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        _logger.Warn("Login failed. Reason: User is locked out: {Email}", null, null, dto.Email);
                        throw new HandleException("Login failed.", 400, new List<string> { "Account is locked out." });
                    }

                    if (!await _userManager.HasPasswordAsync(user))
                    {
                        _logger.Warn("Login failed. Reason: User has no password set: {Email}", null, null, dto.Email);
                        throw new HandleException("Login failed.", 400, new List<string> { "No password set for this account." });
                    }

                    _logger.Warn("Login failed. Reason: Invalid password for user {Email}", null, null, dto.Email);
                    throw new HandleException("Login failed.", 400, new List<string> { "Incorrect password." });
                }

                await _userManager.UpdateSecurityStampAsync(user);
                _logger.Info("Security stamp updated for user {UserId} ({Email}).", null, null, user.Id, user.Email!);

                await _refreshTokenService.RevokeAllTokensForUserAsync(user.Id.ToString(), "New login detected", clientIp);
                _logger.Info("Revoked all existing refresh tokens for user {UserId} ({Email}) due to new login.", null, null, user.Id, user.Email!);

                var accessToken = await _jwtTokenGenerator.GenerateTokenAsync(user);
                var accessTokenExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]!));
                _logger.Info("Generated new access token for user {UserId} ({Email}).", null, null, user.Id, user.Email!);

                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user, clientIp, dto.RememberMe);
                _logger.Info("Generated new refresh token for user {UserId} with expiry {ExpiryTime}.", null, null, user.Id, refreshToken.ExpiryTime.ToShortTimeString());

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = accessTokenExpires,
                    UserId = user.Id
                };
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in LoginAsync for {Email}", ex, null, null, dto.Email);
                throw new HandleException("An unexpected error occurred during login.", 500);
            }
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", null, null, userId);
                throw new HandleException("User not found.", 404);
            }
            _logger.Debug("User {UserId} ({Email}) retrieved successfully.", null, null, user.Id, user.Email!);
            return user;
        }

        public async Task UpdateSecurityStampAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", null, null, id);
                throw new HandleException("User not found.", 404);
            }
            await _userManager.UpdateSecurityStampAsync(user);
            _logger.Debug("Security stamp updated for user {UserId} ({Email}).", null, null, user.Id, user.Email!);
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn("User not found for ID {UserId}", userId);
                throw new HandleException("User not found.", 404);
            }
            var roles = await _userManager.GetRolesAsync(user);
            _logger.Info("Roles retrieved for user {UserId} ({Email}): {@Roles}", user.Id, user.Email, roles);
            return roles.ToList();
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string? clientIp)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", null, null, dto.UserId);
                    throw new HandleException("User not found.", 404);
                }
                var refreshedToken = await _refreshTokenService.RotateRefreshTokenAsync(dto.RefreshToken, dto.UserId, clientIp);

                if (refreshedToken == null)
                {
                    _logger.Warn("Failed to refresh token for user {UserId}. Invalid or expired refresh token.", null, null, dto.UserId);
                    throw new HandleException("Invalid or expired refresh token.", 401);
                }

                var newAccessToken = refreshedToken.Value.newAccessToken;
                var newRefreshToken = refreshedToken.Value.newRefreshToken;
                var accessTokenExpires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]!));
                _logger.Info("User {UserId} refreshed token successfully.", null, null, dto.UserId);

                return new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiryTime = accessTokenExpires
                };
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in RefreshTokenAsync for {UserId}", ex, null, null, dto.UserId);
                throw new HandleException("An unexpected error occurred during token refresh.", 500);
            }
        }

        public async Task LogoutAsync(string userId, string? clientIp)
        {
            try
            {
                string reason = "User logged out";
                await _refreshTokenService.RevokeAllTokensForUserAsync(userId, reason, clientIp);
                _logger.Debug("All tokens revoked for user {userId} due to logout. Reason: {reason}", null, null, userId, reason);
            }
            catch (Exception ex)
            {
                _logger.Error("Error revoking tokens for user {userId} during logout.", ex, null, null, userId);
                throw new HandleException("An unexpected error occurred during logout.", 500);
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.Warn("User not found for email {Email}", email);
                    throw new HandleException("User not found.", 404);
                }

                _logger.Info("User {UserId} ({Email}) retrieved successfully.", user.Id, user.Email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error("Fail to retrieved user.", ex);
                throw new HandleException("An unexpected error occurred during logout.", 500);
            }
        }

        public async Task ForgotPasswordAsyns(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.Error("User not found.");
                    throw new HandleException("User not found.", 400, ["Email does not exist in the system."]);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                _logger.Debug("Generate password reset token successfull.");

                // Send email to reset password
                var resetPasswordLink = $"{_configuration["Frontend:BaseUrl"]}/reset-password?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
                _logger.Debug("Frontend base url: {FrontendBaseUrl}", null, null, _configuration["Frontend:BaseUrl"]!);
                _logger.Debug("Reset password link: {resetPasswordLink}", null, null, resetPasswordLink);
                _logger.Debug("Send email to reset password for user {ID} with token: {token}", null, null, user.Id, token);
                await _sendMailService.SendEmailAsync(request.Email, "Reset password", $"Click the link to reset your password: <a href='{resetPasswordLink}'>Reset Password</a>");
            }
            catch (HandleException hex)
            {
                _logger.Error("Error in ForgotPasswordAsyns for email {Email}.", hex, null, null, request.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error generate password reset token.", ex);
                throw new HandleException("An unexpected error occurred during generate password reset token.", 500);
            }
        }

        public async Task<bool> ResetPasswordAsyns(ResetPasswordRequest dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                {
                    _logger.Error("User not found.");
                    throw new HandleException("User not found.", 404);
                }

                var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.Error("Failed to reset password.");
                    throw new HandleException("Failed to reset password.", 400, result.Errors.Select(e => e.Description).ToList());
                }

                return result.Succeeded;
            }
            catch (HandleException hex)
            {
                _logger.Error("Error in ResetPasswordAsyns for user ID {UserId}.", hex, null, null, dto.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Error reset password.", ex);
                throw new HandleException("An unexpected error occurred during reset password.", 500);
            }
        }

        public async Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", propertyValues: request.UserId);
                    throw new HandleException("User not found.", 404);
                }
                var result = await _userManager.ConfirmEmailAsync(user, request.Token);
                if (!result.Succeeded)
                {
                    _logger.Warn("Failed to confirm email for user {UserId}.", propertyValues: request.UserId);
                    throw new HandleException("Email confirmation failed.", 400, [.. result.Errors.Select(e => e.Description)]);
                }

                return true;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in ConfirmEmailAsync for {userId}", ex, null, null, request.UserId);
                throw new HandleException("An unexpected error occurred during email confirmation.", 500);
            }
        }

        public async Task<bool> ResendEmailConfirmationAsync(ResendEmailConfirmationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.Error("User not found for email {Email}", null, null, null, request.Email);
                throw new HandleException("User not found.", 404);
            }

            if (user.EmailConfirmed)
            {
                _logger.Warn("Email for user {UserId} is already confirmed.", null, null, user.Id);
                throw new HandleException("Email is already confirmed.", 400);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.Debug("Base URL for frontend: {FrontendBaseUrl}", null, null, _configuration["Frontend:BaseUrl"]!);
            var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var subject = "Confirm your email";
            var body = $"<p>Hello {user.UserName},</p>" +
                       $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your email address.</p>";
            try
            {
                await _sendMailService.SendEmailAsync(user.Email!, subject, body, isHtml: true);
                _logger.Debug("Resend email confirmation link to user {UserId} with token {Token}.", null, null, user.Id, token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to resend email confirmation for user {UserId} ({Email}).", ex, null, null, user.Id, user.Email!);
                throw new HandleException("Failed to resend email confirmation.", 500);
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.Warn("User not found for ID {UserId}", userId);
                    throw new HandleException("User not found.", 404);
                }
                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.Warn("Failed to change password for user {UserId}.", userId);
                    throw new HandleException("Failed to change password.", 400, result.Errors.Select(e => e.Description).ToList());
                }

                return true;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error in ChangePasswordAsync for {userId}", ex, null, null, userId);
                throw new HandleException("An unexpected error occurred during password change.", 500);
            }
        }

        public async Task UpdateUserInfoAsync(UpdateAuthEvent updateAuth)
        {
            bool userChanged = false;
            var user = await _userManager.FindByIdAsync(updateAuth.Id.ToString());

            if (!string.IsNullOrEmpty(updateAuth.Email) && user?.Email != updateAuth.Email)
            {
                _logger.Debug("Attempting to update Email for user {UserId} from {OldEmail} to {NewEmail}.", null, null, user.Id, user.Email!, updateAuth.Email);
                var setUsernameResult = await _userManager.SetUserNameAsync(user, updateAuth.Email);
                if (!setUsernameResult.Succeeded)
                {
                    var errors = string.Join(", ", setUsernameResult.Errors.Select(e => e.Description));
                    throw new HandleException($"Failed to set UserName: {errors}", 400, new List<string> { errors });
                }

                var setEmailResult = await _userManager.SetEmailAsync(user, updateAuth.Email);
                if (!setEmailResult.Succeeded)
                {
                    _logger.Error("Failed to update Email for user.");
                    throw new HandleException($"Failed to set Email: {string.Join(", ", setEmailResult.Errors.Select(e => e.Description))}", 400, new List<string> { "Failed to update email." });
                }
                user.EmailConfirmed = false;
                userChanged = true;

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailToken)}";
                var subject = "Confirm your email";
                var body = $"<p>Hello {user.UserName},</p>" +
                           $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your new email address.</p>";
                await _sendMailService.SendEmailAsync(updateAuth.Email, subject, body, isHtml: true);
            }

            if (!string.IsNullOrEmpty(updateAuth.PhoneNumber) && user.PhoneNumber != updateAuth.PhoneNumber)
            {
                _logger.Info("Attempting to update Phone Number for user {UserId} from {OldPhone} to {NewPhone}.", null, null, user.Id, user.PhoneNumber!, updateAuth.PhoneNumber);
                var setPhoneNumberResult = await _userManager.SetPhoneNumberAsync(user, updateAuth.PhoneNumber);
                if (!setPhoneNumberResult.Succeeded)
                {
                    var errors = string.Join(", ", setPhoneNumberResult.Errors.Select(e => e.Description));
                    throw new HandleException($"Failed to set PhoneNumber: {errors}", 400, new List<string> { errors });
                }
                user.PhoneNumberConfirmed = false;
                userChanged = true;
            }

            if (userChanged)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.Error("Failed to save user changes and SecurityStamp for user.");
                    throw new HandleException($"Failed to save user changes: {errors}", 400, new List<string> { "Failed to update user information." });
                }
                _logger.Info("User {UserId} SecurityStamp updated and changes saved.", null, null, user.Id);
            }
            else
            {
                _logger.Info("No relevant email or phone number changes for user {UserId}.", null, null, user.Id);
            }
        }

        public async Task<ApplicationUser> CreateUserByAdminAsync(CreateUserByAdminRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.Warn("User with Email {Email} already exists.", null, null, request.Email);
                throw new Exception("Email is already exists.");
            }

            var randomPassword = GenerateRandomPassword(_passwordOptions);
            _logger.Info("Generated random password for new user {Email}.", null, null, request.Email);

            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                //EmailConfirmed = true,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(request.PhoneNumber)
            };

            var createResult = await _userManager.CreateAsync(newUser, randomPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                _logger.Error("Failed to create user identity in AuthService for Email {Email}", null, null, request.Email);
                throw new Exception($"Failed to create user identity: {errors}");
            }

            var assignRoleResult = await _userManager.AddToRoleAsync(newUser, "User");
            if (!assignRoleResult.Succeeded)
            {
                _logger.Error("Failed to assign 'User' role to new user {UserId} with Email {Email}. Rolling back user identity creation.", null, null, null, newUser.Id, newUser.Email);
                await _userManager.DeleteAsync(newUser);
                throw new HandleException("Failed to assign default role, user identity creation rolled back.", (int)HttpStatusCode.InternalServerError);
            }
            _logger.Debug("New user identity {Email} created in AuthService and assigned 'User' role. User ID: {UserId}.", null, null, newUser.Email, newUser.Id.ToString());

            await CallToUserServiceToCreateAccount(newUser, request);

            try
            {
                var emailSubject = "Tài khoản của bạn đã được tạo!";
                var emailBody = $"Xin chào {request.Email},\n\n";
                emailBody += $"Tài khoản của bạn đã được tạo bởi quản trị viên.\n";
                emailBody += $"Email đăng nhập của bạn là: {request.Email}\n";
                emailBody += $"Mật khẩu tạm thời của bạn là: <strong>{randomPassword}</strong>\n\n";
                emailBody += "Vui lòng đăng nhập và thay đổi mật khẩu của bạn càng sớm càng tốt.\n\n";
                emailBody += "Trân trọng,\nĐội ngũ hỗ trợ của chúng tôi";

                await _sendMailService.SendEmailAsync(request.Email, emailSubject, emailBody);
                _logger.Info("Random password sent to user {Email}.", request.Email);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to send random password email to user {Email} after successful creation. This requires manual intervention.", ex, request.Email);
                throw new HandleException("Failed to send email with random password after user creation.", (int)HttpStatusCode.InternalServerError);
            }

            return newUser;
        }

        private string GenerateRandomPassword(PasswordOptions opts)
        {
            string[] randomChars = new[]
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",
                "abcdefghijkmnopqrstuvwxyz",
                "0123456789",
                "!@$?_-"
            };

            var rand = new Random();
            var chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public async Task CallToUserServiceToCreateAccount(ApplicationUser newUser, CreateUserByAdminRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var userServiceBaseUrl = _configuration["UserService:BaseUrl"] ?? throw new InvalidOperationException("UserService:BaseUrl is not configured.");
                httpClient.BaseAddress = new Uri(userServiceBaseUrl);
                _logger.Debug("User service url: {url}", propertyValues: userServiceBaseUrl);
                //var internalToken = _jwtInternalService.GenerateInternalServiceToken();
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToken);
                var roles = await GetUserRolesAsync(newUser.Id.ToString());
                _logger.Debug("User's roles: {Roles}", propertyValues: roles);
                var profileRequest = new CreateUserProfileRequest
                {
                    Id = newUser.Id,
                    UserName = request.UserName,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = newUser.Email!,
                    Address = request.Address,
                    PhoneNumber = newUser.PhoneNumber!,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    AvatarUrl = request.AvatarUrl,
                    Roles = roles,

                };

                var response = await httpClient.PostAsJsonAsync("api/v1/users", profileRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error("Failed to create user profile in UserService for UserId: {UserId}. Rolling back user identity.", null, null, null, newUser.Id);
                    await _userManager.DeleteAsync(newUser);
                    throw new HandleException($"Failed to create user profile in UserService: {errorContent}. User identity rolled back.", (int)response.StatusCode);
                }

                _logger.Debug("User profile created successfully in UserService for UserId: {UserId}.", null, null, newUser.Id);

            }
            catch (HttpRequestException ex)
            {
                _logger.Error("HTTP request failed when trying to create user profile in UserService for UserId: {UserId}. Rolling back user identity.", ex, null, null, newUser.Id);
                await _userManager.DeleteAsync(newUser);
                throw new HandleException($"Network or communication error with UserService: {ex.Message}. User identity rolled back.", (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.Error("An unexpected error occurred while calling UserService for UserId: {UserId}. Rolling back user identity.", ex, null, null, newUser.Id);
                await _userManager.DeleteAsync(newUser);
                throw new HandleException($"An unexpected error occurred during user profile creation. User identity rolled back.", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<bool> UnLockOutAsync(UnLockOutRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id) ?? throw new HandleException("User not found.", StatusCodes.Status404NotFound);
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);

                await _unLockUserProducer.Produce(new UnLockUserEvent { Id = request.Id });

                return true;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error unlockout for user.", ex);
            }
        }
    }
}
