using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRoleAsync(string name)
        {
            IdentityResult result = await _roleManager.CreateAsync(new() { Name = name });
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string name)
        {
            IdentityResult result = await _roleManager.DeleteAsync(new() { Name = name });
            return result.Succeeded;
        }
        public async Task<bool> UpdateRoleAsync(string id, string name)
        {
            IdentityResult result = await _roleManager.UpdateAsync(new() { Id = id ,Name = name });
            return result.Succeeded;
        }

        public async Task<IDictionary<string, string>> GetAllRolesAsync()
        {
            Dictionary<string ,string> roles = await _roleManager.Roles.ToDictionaryAsync(role => role.Id, role => role.Name);
            return roles;
        }

        public async Task<(string id, string name)> GetRoleByIdAsync(string id)
        {
            string role = await _roleManager.GetRoleIdAsync(new() { Id=id });
            return (id, role);
        }
    }
}
