using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Abstractions.Services.Authentication
{
    public interface IExternalAuthhentication
    {
        Task<T.Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime);
        Task<T.Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime);
    }
}
