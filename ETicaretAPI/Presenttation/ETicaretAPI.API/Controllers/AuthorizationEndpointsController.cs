using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationEndpointsController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthorizationEndpointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async  Task<IActionResult> AssignRoleEndpoint(AssignRoleCommandRequest assignRoleCommandRequest)
        {
            AssignRoleCommandResponse response = await _mediator.Send(assignRoleCommandRequest);
            return Ok(response);
        }
    }
}
