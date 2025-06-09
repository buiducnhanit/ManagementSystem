using ApplicationCore.DTOs;
using ApplicationCore.Services;
using Asp.Versioning;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ICustomLogger<AuthController> _logger;

        public RolesController(RoleService roleService, ICustomLogger<AuthController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddUserRoles([FromBody] ManageUserRolesRequest request)
        {
            try
            {
                var result = await _roleService.AddUserRolesAsync(request);
                if (result)
                {
                    return Ok(new { message = "Roles added successfully." });
                }
                return BadRequest(new { message = "Failed to add roles." });
            }
            catch (HandleException ex)
            {
                _logger.Error("Error adding roles for user {UserId}", ex, request.UserId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error("An unhandled error occurred while adding roles for user {UserId}", ex, request.UserId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveUserRoles([FromBody] ManageUserRolesRequest request)
        {
            try
            {
                var result = await _roleService.RemoveUserRolesAsync(request);
                if (result)
                {
                    return Ok(new { message = "Roles removed successfully." });
                }
                return BadRequest(new { message = "Failed to remove roles." });
            }
            catch (HandleException ex)
            {
                _logger.Error("Error removing roles for user {UserId}", ex, request.UserId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error("An unhandled error occurred while removing roles for user {UserId}", ex, request.UserId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
