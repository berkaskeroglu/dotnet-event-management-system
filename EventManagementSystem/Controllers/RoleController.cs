using EventManagementSystem.Services;
using EventManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET api/role
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        // GET api/role/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        // POST api/role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest();
            }
            var createdRole = await _roleService.CreateAsync(role);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }

        // PUT api/role/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            var existingRole = await _roleService.GetByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound();
            }

            await _roleService.UpdateAsync(role);
            return NoContent();
        }

        // DELETE api/role/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
