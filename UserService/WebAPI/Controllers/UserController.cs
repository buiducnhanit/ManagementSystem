using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/user")]
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

        [HttpPost("create")]
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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userProfile = await _userService.GetUserByIdAsync(userId);
                if (userProfile == null)
                {
                    _logger.Warn("User with ID: {ID} not found for update.", userId);
                    return NotFound(ApiResponse<string>.FailureResponse("User not found.", 404));
                }

                var updatedUserProfile = await _userService.UpdateUserAsync(request);
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
    }
}
