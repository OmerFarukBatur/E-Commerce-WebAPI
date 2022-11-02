using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Role.RemoveRole
{
    public class RemoveRoleCommandRequest : IRequest<RemoveRoleCommandResponse>
    {
        public string Name { get; set; }
    }
}