using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Role.RemoveRole
{
    public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommandRequest, RemoveRoleCommandResponse>
    {
        readonly IRoleService _roleService;

        public RemoveRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<RemoveRoleCommandResponse> Handle(RemoveRoleCommandRequest request, CancellationToken cancellationToken)
        {
            bool result = await _roleService.DeleteRoleAsync(request.Name);
            
            return new()
            {
                Succeeded = result
            };
        }
    }
}
