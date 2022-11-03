using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<(object,int)> GetAllRolesAsync(int page, int size);
        Task<(string id, string name)> GetRoleByIdAsync(string id);
        Task<bool> CreateRoleAsync(string name);
        Task<bool> DeleteRoleAsync(string id);
        Task<bool> UpdateRoleAsync(string id,string name);
    }
}
