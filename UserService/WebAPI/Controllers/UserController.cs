using Asp.Versioning;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs;
using WebAPI.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomLogger<UserController> _logger;

        public UserController(IUserService userService, ICustomLogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.Error("CreateUser request is null.", null, null, null);
                    return BadRequest("Invalid user data.");
                }

                var userProfile = await _userService.CreateUserAsync(request);
                _logger.Info("User created successfully.", null, null, userProfile);

                return Ok(ApiResponse<UserProfile>.SuccessResponse(userProfile, "User created successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error creating user.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error creating user.", errors: ex.Errors));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var userProfile = await _userService.GetUserByIdAsync(id);
                if (userProfile == null)
                {
                    _logger.Warn("User with ID: {ID} not found.", null, null, id);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                _logger.Info("User retrieved successfully.", null, null, userProfile);
                return Ok(ApiResponse<UserProfile>.SuccessResponse(userProfile, "User retrieved successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error retrieving user with ID {ID}.", ex, null, null, id);
                return BadRequest(ApiResponse<string>.FailureResponse("Error retrieving user.", errors:ex.Errors));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userProfile = await _userService.GetUserByIdAsync(id);
                if (userProfile == null)
                {
                    _logger.Warn("User with ID: {ID} not found for update.", null, null, id);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                var updatedUserProfile = await _userService.UpdateUserAsync(id, request);
                _logger.Info("User updated successfully.", null, null, updatedUserProfile);

                return Ok(ApiResponse<UserProfile>.SuccessResponse(updatedUserProfile, "User updated successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error updating user.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error updating user.", errors: ex.Errors));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var userProfile = await _userService.GetUserByIdAsync(id);
                if (userProfile == null)
                {
                    _logger.Warn("User with ID: {ID} not found for deletion.", null, null, id);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }
                var isDeleted = await _userService.DeleteUserAsync(id);
                if (!isDeleted)
                {
                    _logger.Error("Error deleting user.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Error deleting user."));
                }
                _logger.Info("User deleted successfully.", null, null, id);
                return Ok(ApiResponse<string>.SuccessResponse("User deleted successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error deleting user with ID {ID}.", ex, null, null, id);
                return BadRequest(ApiResponse<string>.FailureResponse("Error deleting user.", errors: ex.Errors));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                if (users == null || !users.Any())
                {
                    _logger.Warn("No users found.");
                    return NotFound(ApiResponse<string>.FailureResponse("No users found.", 404));
                }
                _logger.Info("All users retrieved successfully.");
                return Ok(ApiResponse<IEnumerable<UserProfile>>.SuccessResponse(users, "Users retrieved successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error retrieving all users.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error retrieving users.", errors: ex.Errors));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warn("User ID not found in claims.");
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }
                var userProfile = await _userService.GetUserByIdAsync(Guid.Parse(userId));
                if (userProfile == null)
                {
                    _logger.Warn("User profile not found for ID: {ID}.", null, null, userId);
                    return NotFound(ApiResponse<string>.FailureResponse("User profile not found.", 404));
                }
                _logger.Info("User profile retrieved successfully.", null, null, userProfile);
                return Ok(ApiResponse<UserProfile>.SuccessResponse(userProfile, "User profile retrieved successfully."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error retrieving user profile.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error retrieving user profile.", errors: ex.Errors));
            }
        }
    }
}
