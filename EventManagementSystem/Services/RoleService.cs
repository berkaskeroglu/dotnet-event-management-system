using EventManagementSystem.Models;
using EventManagementSystem.Repositories;


namespace EventManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            return await _roleRepository.CreateAsync(role);
        }

        public async Task UpdateAsync(Role role)
        {
            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteAsync(int id)
        {
            await _roleRepository.DeleteAsync(id);
        }
    }
}
