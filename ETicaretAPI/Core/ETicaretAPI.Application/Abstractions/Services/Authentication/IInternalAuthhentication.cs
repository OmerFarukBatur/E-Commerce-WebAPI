using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Abstractions.Services.Authentication
{
    public interface IInternalAuthhentication
    {
        Task<T.Token> LoginAsync(string userNameOrEmail, string password, int accessTokenLifeTime);
        Task<T.Token> RefreshTokenLoginAsync(string refreshToken );
    }
}
