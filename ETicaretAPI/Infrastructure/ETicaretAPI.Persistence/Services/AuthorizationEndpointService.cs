using ETicaretAPI.Application.Abstractions.Services;
using  ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Repositories.Endpoint;
using ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Repositories.Menu;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        readonly IApplicationService _applicationService;
        readonly IEndpointReadRepository _endpointReadRepository;
        readonly IEndpointWriteRepository _endpointWriteRepository;
        readonly IMenuReadRepository _menuReadRepository;
        readonly IMenuWriteRepository _menuWriteRepository;

        public AuthorizationEndpointService(
            IApplicationService applicationService, 
            IEndpointReadRepository endpointReadRepository, 
            IEndpointWriteRepository endpointWriteRepository, 
            IMenuReadRepository menuReadRepository, 
            IMenuWriteRepository menuWriteRepository
            )
        {
            _applicationService = applicationService;
            _endpointReadRepository = endpointReadRepository;
            _endpointWriteRepository = endpointWriteRepository;
            _menuReadRepository = menuReadRepository;
            _menuWriteRepository = menuWriteRepository;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string code,string menu ,Type type)
        {
            Menu? _menu = await _menuReadRepository.GetSingleAsync(m => m.Name == menu);
            if (menu == null)
            {
                await _menuWriteRepository.AddAsync(new()
                {
                    Id = Guid.NewGuid(),
                    Name = menu
                });

                await _menuWriteRepository.SaveAsync();
            }
            Endpoint? endpoint = await _endpointReadRepository.Table.Include(m=> m.Menu).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);
            if (endpoint == null)
            {
                var action = _applicationService.GetAuthorizeDefinitionEndpoints(type).FirstOrDefault(m => m.Name == menu)?.Actions.FirstOrDefault(m => m.Code == code);
                
                await _endpointWriteRepository.AddAsync(new() {
                    Id = Guid.NewGuid(),
                    Code = action.Code,
                    ActionType = action.ActionType,
                    HttpType = action.HttpType,
                    Definition = action.Definition
                }); 
                
                await _endpointWriteRepository.SaveAsync();
            }

           
        }
    }
}
