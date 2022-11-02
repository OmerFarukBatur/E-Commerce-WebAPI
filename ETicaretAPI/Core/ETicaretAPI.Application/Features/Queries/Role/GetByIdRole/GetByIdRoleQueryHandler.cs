using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Role.GetByIdRole
{
    public class GetByIdRoleQueryHandler : IRequestHandler<GetByIdRoleQueryRequest, GetByIdRoleQueryResponse>
    {
        readonly IRoleService _roleService;

        public GetByIdRoleQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetByIdRoleQueryResponse> Handle(GetByIdRoleQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleService.GetRoleByIdAsync(request.Id);

            return new()
            {
                Id = result.id,
                Name = result.name
            };
        }
    }
}
