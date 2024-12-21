using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagementSystem.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task CreateAsync(User user, string password);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> ValidateUserCredentials(string username, string password);
    }
}
