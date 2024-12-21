using EventManagementSystem.Repositories;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using EventManagementSystem.Utilities;

namespace EventManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher; 

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task CreateAsync(User user, string password)
        {
            var salt = _passwordHasher.GetSalt();

            user.PasswordHash = _passwordHasher.HashPassword(password, salt);
            user.PasswordSalt = _passwordHasher.GetSalt();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.CreateAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateUserCredentials(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return false;

            var isPasswordValid = _passwordHasher.ValidatePassword(password, user.PasswordSalt, user.PasswordHash);
            return isPasswordValid;
        }
    }
}
