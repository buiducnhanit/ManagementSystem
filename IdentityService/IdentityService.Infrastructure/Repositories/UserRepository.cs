using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return new User
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!
            };
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            return new User
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!
            };
        }

        public Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return null;

            return new User
            {
                Id = appUser.Id,
                UserName = appUser.UserName!,
                Email = appUser.Email!
            };
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
