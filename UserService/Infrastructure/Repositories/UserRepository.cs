using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        public UserRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await Task.Run(() =>
            {
                if (user == null) throw new ArgumentNullException(nameof(user));
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            });
        }

        public Task<User> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
