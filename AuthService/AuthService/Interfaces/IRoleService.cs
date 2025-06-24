using AuthService.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddUserRolesAsync(ManageUserRolesRequest request);
        Task<bool> RemoveUserRolesAsync(ManageUserRolesRequest request);
        Task<IdentityRole<Guid>> CreateNewRoleAsync(CreateRoleRequest request);
        Task<IdentityRole<Guid>> UpdateRoleAsync(UpdateRoleRequest request);
        Task<List<IdentityRole<Guid>>> GetAllRolesAsync();
    }
}
