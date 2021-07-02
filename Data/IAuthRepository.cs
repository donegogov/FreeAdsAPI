using System.Threading.Tasks;
using FreeAds.API.Dtos;
using FreeAds.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace FreeAds.API.Data
{
    public interface IAuthRepository
    {
         Task<AppUser> Register(AppUser user, string password);
         Task<AppUser> Login(string username, string password);
         Task<bool> UserExists(string username);
         /*SecurityToken CreateToken(UserForTokenDto userForToken, 
            SymmetricSecurityKey key, string userRole, string city);*/
    }
}