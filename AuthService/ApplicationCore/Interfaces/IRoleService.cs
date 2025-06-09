using ApplicationCore.DTOs;

namespace ApplicationCore.Interfaces
{
    public interface IRoleService
    {
        Task<bool> AddUserRolesAsync(ManageUserRolesRequest request);
        Task<bool> RemoveUserRolesAsync(ManageUserRolesRequest request);
    }
}
