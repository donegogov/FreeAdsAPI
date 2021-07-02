using System.Collections.Generic;
using System.Threading.Tasks;
using FreeAds.API.Models;

namespace FreeAds.API.Data
{
    public interface IUserRepository
    {
         void Add(AppUser user);
        Task<bool> Delete(string id);
         Task<bool> SaveAll();
         Task<IEnumerable<AppUser>> GetUsers();
         Task<AppUser> GetUser(string id);
    }
}