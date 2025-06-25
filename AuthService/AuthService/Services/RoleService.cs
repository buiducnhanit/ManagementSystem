using AuthService.DTOs;
using AuthService.Entities;
using AuthService.Interfaces;
using ManagementSystem.Shared.Common.Exceptions;
using ManagementSystem.Shared.Common.Logging;
using ManagementSystem.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuthService.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICustomLogger<AuthServices> _logger;
        private readonly ITopicProducer<UpdateUserProfileEvent> _producer;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ICustomLogger<AuthServices> logger, ITopicProducer<UpdateUserProfileEvent> producer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _producer = producer;
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

            // Publish an event to update user profile
            var roles = await _userManager.GetRolesAsync(user);
            var updateEvent = new UpdateUserProfileEvent
            {
                Id = request.UserId.ToString(),
                Roles = [..roles]
            };
            await _producer.Produce(updateEvent);

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
            // Publish an event to update user profile
            var roles = await _userManager.GetRolesAsync(user);
            var updateEvent = new UpdateUserProfileEvent
            {
                Id = request.UserId.ToString(),
                Roles = [.. roles]
            };
            await _producer.Produce(updateEvent);
            return true;
        }

        public async Task<IdentityRole<Guid>> CreateNewRoleAsync(CreateRoleRequest request)
        {
            try
            {
                var role = new IdentityRole<Guid>(request.RoleName);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    _logger.Error("Errors creating {Role}.", propertyValues: request.RoleName);
                    throw new HandleException("Errors creating {Role}.", StatusCodes.Status400BadRequest, [.. result.Errors.Select(d => d.Description)]);
                }

                _logger.Info("Creating {Role} role successfully.", propertyValues: request.RoleName);
                return role;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Unexpected error occurred while creating role {RoleId}.", e);
            }
        }

        public async Task<IdentityRole<Guid>> UpdateRoleAsync(UpdateRoleRequest request)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(request.OldRoleName);

                if (role == null)
                {
                    _logger.Warn("Role {RoleId} not found.", propertyValues: request.OldRoleName);
                    throw new HandleException("Role not found.", StatusCodes.Status404NotFound);
                }

                role.Name = request.NewRoleName;
                role.NormalizedName = request.NewRoleName.ToUpperInvariant();

                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    _logger.Error("Failed to update role {Role}. Errors: {@Errors}", propertyValues: [request.OldRoleName, errorMessages]);

                    throw new HandleException("Failed to update role.", StatusCodes.Status400BadRequest, errorMessages);
                }

                _logger.Info("Role {Role} updated successfully.", request.OldRoleName);
                return role;
            }
            catch (HandleException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error("Unexpected error occurred while updating role {Role}.", propertyValues: request.OldRoleName);
                throw new Exception("Errors update role", e);
            }
        }

        public async Task<List<IdentityRole<Guid>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                _logger.Info("Retrieved {Count} roles successfully.", roles.Count);
                return roles;
            }
            catch (Exception e)
            {
                _logger.Error("Unexpected error occurred while retrieving roles.", e);
                throw new Exception("Error retrieving roles", e);
            }
        }
    }
}
