﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using U = ETicaretAPI.Domain.Entities.Identity.AppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Abstractions.Token;
using System.Text.Json;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Abstractions.Services;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly IAuthService _authService;

        public FacebookLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {

           var token = await _authService.FacebookLoginAsync(request.AuthToken,15);
            return new()
            {
                Token = token
            }; 
        }
    }
}
