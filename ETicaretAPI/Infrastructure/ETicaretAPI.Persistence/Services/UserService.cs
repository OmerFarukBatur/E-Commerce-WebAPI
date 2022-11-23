using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;

        
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                NameSurname = model.NameSurname
            }, model.Password);


            CreateUserResponse response = new() { Succeeded = true };

            if (result.Succeeded)
            {
                response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
            }
            else
            {
                response.Succeeded = false;
                foreach (var error in result.Errors)
                {
                    response.Message += $"{error.Code} - {error.Description}\n";
                }
            }

            return response;
        }        

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
            {
                throw new NotFoundUserException();
            }
            
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user,resetToken,newPassword);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                {
                    throw new PasswordChangeFailedException();
                }
            }
        }

        public async Task<List<ListUser>> GetAllUsersAsync(int page, int size)
        {
            var users = await _userManager.Users
                .Skip(page * size)
                .Take(size)
                .ToListAsync();
            return users.Select(user => new ListUser
            {
                Id = user.Id,
                NameSurname = user.NameSurname,
                Email = user.Email,
                TwoFactorEnabled =user.TwoFactorEnabled,
                UserName = user.UserName
            }).ToList();
        }

        public int TotalUsersCount => _userManager.Users.Count();

        public async Task AssignRoleToUserAsync(string userId, string[] roles)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                var userRoles = await _userManager.GetRolesAsync(appUser);
                await _userManager.RemoveFromRolesAsync(appUser,userRoles);
                await _userManager.AddToRolesAsync(appUser, roles);
            }
        }

        public async Task<string[]> GetRolesToUserAsync(string userId)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                return roles.ToArray();
            }
            return new string[] { };
        }
    }
}
