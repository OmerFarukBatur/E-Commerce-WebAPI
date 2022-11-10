using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommandRequest, AssignRoleCommandResponse>
    {
        public Task<AssignRoleCommandResponse> Handle(AssignRoleCommandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
