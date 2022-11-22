using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword;
using ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest createUserCommandRequest)
        {
            CreateUserCommandResponse response = await _mediator.Send(createUserCommandRequest);
            return Ok(response);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            UpdatePasswordCommandResponse response = await _mediator.Send(updatePasswordCommandRequest);    
            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get All Users", Menu = AuthorizeDefinitionConstants.Users)]
        public async Task<IActionResult>GetAllUsers([FromQuery]GetAllUsersQueryRequest getAllUsersQueryRequest)
        {
            GetAllUsersQueryResponse response = await _mediator.Send(getAllUsersQueryRequest);
            return Ok(response);
        }
    }
}
