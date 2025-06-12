using AuthService.DTOs;

namespace AuthService.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddUserRolesAsync(ManageUserRolesRequest request);
        Task<bool> RemoveUserRolesAsync(ManageUserRolesRequest request);
    }
}
