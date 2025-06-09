using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using Asp.Versioning;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                    _logger.Error("CreateUser request is null.");
                    return BadRequest("Invalid user data.");
                }

                var userProfile = await _userService.CreateUserAsync(request);
                _logger.Info("User created successfully.", userProfile);

                return Ok(ApiResponse<UserProfile>.SuccessResponse(userProfile, "User created successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating user.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error creating user."));
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
                    _logger.Warn("User with ID: {ID} not found.", id);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                _logger.Info("User retrieved successfully.", userProfile);
                return Ok(ApiResponse<UserProfile>.SuccessResponse(userProfile, "User retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving user with ID {id}.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error retrieving user."));
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userProfile = await _userService.GetUserByIdAsync(userId);
                //var userProfile = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userProfile == null)
                {
                    _logger.Warn("User with ID: {ID} not found for update.", userId);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                var updatedUserProfile = await _userService.UpdateUserAsync(userId, request);
                _logger.Info("User updated successfully.", updatedUserProfile);

                return Ok(ApiResponse<UserProfile>.SuccessResponse(updatedUserProfile, "User updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating user.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error updating user."));
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
                    _logger.Warn("User with ID: {ID} not found for deletion.", id);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }
                var isDeleted = await _userService.DeleteUserAsync(id);
                if (!isDeleted)
                {
                    _logger.Error("Error deleting user.");
                    return BadRequest(ApiResponse<string>.FailureResponse("Error deleting user."));
                }
                _logger.Info("User deleted successfully.", id);
                return Ok(ApiResponse<string>.SuccessResponse("User deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user with ID {id}.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error deleting user."));
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
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
            catch (Exception ex)
            {
                _logger.Error("Error retrieving all users.", ex);
                return BadRequest(ApiResponse<string>.FailureResponse("Error retrieving users."));
            }
        }
    }
}
