using MediatR;
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

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly UserManager<U> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient;

        public FacebookLoginCommandHandler(UserManager<U> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            string accessTokenResponse = await _httpClient
                .GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id=2626403204157763&client_secret=06cba76f0a17879f3e867cdc97477f78&grant_type=client_credentials");

            FacebookAccessTokenResponse facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");

            FacebookUserAccessTokenValidation validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");

                FacebookUserInfoResponse userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);


                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId , "FACEBOOK");

                U user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                bool result = user != null;

                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(userInfo.Email);
                    if (user == null)
                    {
                        user = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = userInfo.Email,
                            UserName = userInfo.Email,
                            NameSurname = userInfo.Name
                        };
                        var identityResult = await _userManager.CreateAsync(user);
                        result = identityResult.Succeeded;
                    }
                }

                if (result)
                {
                    await _userManager.AddLoginAsync(user, info);
                    
                    Token token = _tokenHandler.CreateAccessToken(5);
                    return new() {
                        Token = token
                    };
                }             
                
            }
            throw new Exception("Invalid external authentication.");
        }
    }
}
