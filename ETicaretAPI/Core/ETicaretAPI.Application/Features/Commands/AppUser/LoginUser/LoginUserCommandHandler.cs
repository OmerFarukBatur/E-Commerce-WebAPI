using MediatR;
using Microsoft.AspNetCore.Identity;
using U = ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Exceptions;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly UserManager<U.AppUser> _userManager;
        readonly SignInManager<U.AppUser> _signInManager;

        public LoginUserCommandHandler(UserManager<U.AppUser> userManager, SignInManager<U.AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            U.AppUser appUser = await  _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (appUser == null)
                appUser = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
            
            if (appUser == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(appUser, request.Password,false);

            if (result.Succeeded) // Authentication başarılı
            { 
                //   Yetkileri belirlemeliyiz.
                return new();
            }

            return null;
        }
    }
}
