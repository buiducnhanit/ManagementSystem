using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ApplicationCore.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICustomLogger<AuthService> _logger;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ICustomLogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<bool> AddUserRolesAsync(ManageUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                _logger.Warn("User with ID {UserId} not found when trying to add roles.", request.UserId);
                throw new HandleException("User not found.", (int)HttpStatusCode.NotFound);
            }

            foreach (var roleName in request.RoleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.Warn("Role '{RoleName}' does not exist.", roleName);
                    throw new HandleException($"Role '{roleName}' does not exist.", (int)HttpStatusCode.NotFound);
                }

                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    var result = await _userManager.AddToRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        _logger.Error("Failed to add user {UserId} to role {RoleName}: {Errors}", null, null, null, request.UserId, roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                        throw new HandleException($"Failed to add role '{roleName}' to user: {string.Join(", ", result.Errors.Select(e => e.Description))}", (int)HttpStatusCode.InternalServerError);
                    }
                    _logger.Info("User {UserId} successfully added to role {RoleName}.", request.UserId, roleName);
                }
                else
                {
                    _logger.Info("User {UserId} is already in role {RoleName}.", request.UserId, roleName);
                }
            }
            return true;
        }

        public async Task<bool> RemoveUserRolesAsync(ManageUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                _logger.Warn("User with ID {UserId} not found when trying to remove roles.", request.UserId);
                throw new HandleException("User not found.", (int)HttpStatusCode.NotFound);
            }

            foreach (var roleName in request.RoleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.Warn("Role '{RoleName}' does not exist.", roleName);
                    throw new HandleException($"Role '{roleName}' does not exist.", (int)HttpStatusCode.NotFound);
                }

                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        _logger.Error("Failed to remove user {UserId} from role {RoleName}: {Errors}", null, null, null, request.UserId, roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                        throw new HandleException($"Failed to remove role '{roleName}' from user: {string.Join(", ", result.Errors.Select(e => e.Description))}", (int)HttpStatusCode.InternalServerError);
                    }
                    _logger.Info("User {UserId} successfully removed from role {RoleName}.", request.UserId, roleName);
                }
                else
                {
                    _logger.Info("User {UserId} is not in role {RoleName}.", request.UserId, roleName);
                }
            }
            return true;
        }
    }
}
