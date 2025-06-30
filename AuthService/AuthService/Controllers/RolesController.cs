using Asp.Versioning;
using AuthService.DTOs;
using AuthService.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Common.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ICustomLogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ICustomLogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("add-roles")]
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
                    return Ok(ApiResponse<string>.SuccessResponse("Roles added successfully."));
                }
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to add roles."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error adding roles for user {UserId}", ex, request.UserId);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
            }
        }

        [HttpPost("remove-roles")]
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
                    return Ok(ApiResponse<string>.SuccessResponse("Roles removed successfully."));
                }
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to remove roles."));
            }
            catch (HandleException ex)
            {
                _logger.Error("Error removing roles for user {UserId}", ex, request.UserId);
                return StatusCode(ex.StatusCode, ApiResponse<string>.FailureResponse(ex.Message, ex.StatusCode, ex.Errors));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateNewRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<string>.FailureResponse("Invalidation role data.", 400, errors));
                }

                var role = await _roleService.CreateNewRoleAsync(request);
                if (role != null)
                {
                    return Ok(ApiResponse<IdentityRole<Guid>>.SuccessResponse(role, "Role created successfully."));
                }

                return BadRequest(ApiResponse<string>.FailureResponse("Failed to create role."));
            }
            catch (HandleException ex)
            {
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                if (roles != null)
                {
                    return Ok(ApiResponse<IEnumerable<IdentityRole<Guid>>>.SuccessResponse(roles, "Get roles successfully."));
                }

                return BadRequest(ApiResponse<string>.FailureResponse("Failed to get roles."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateRole([FromForm] UpdateRoleRequest request)
        {
            try
            {
                var updatedRole = await _roleService.UpdateRoleAsync(request);
                if (updatedRole != null)
                {
                    return Ok(ApiResponse<IdentityRole<Guid>>.SuccessResponse(updatedRole, "Updated role successfully.", StatusCodes.Status200OK));
                }
                return BadRequest(ApiResponse<string>.FailureResponse("Failed to update role.", StatusCodes.Status400BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
