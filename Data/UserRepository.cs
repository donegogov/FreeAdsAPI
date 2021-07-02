using System.Collections.Generic;
using System.Threading.Tasks;
using FreeAds.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FreeAds.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;

        }
        public void Add(AppUser user)
        {
            _context.Add(user);
        }

        public async Task<bool> Delete(string id)
        {
            var userToDeleteAttach = new AppUser();
            userToDeleteAttach.Id = id;

            _context.Users.Attach(userToDeleteAttach);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id);

            userToDeleteAttach.Deleted = "UserIsDeletedToDelete";
            _context.Users.Update(userToDeleteAttach);

            return await SaveAll();
        }

        public async Task<AppUser> GetUser(string id)
        {
            //var user = await _context.Users.Include(u => u.ClassifiedAds).ThenInclude(ca => ca.Photos).FirstOrDefaultAsync(u => u.Id == id);
            //lazy loading
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Deleted != "UserIsDeletedToDelete");

            return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            //var users = await _context.Users.Include(u => u.ClassifiedAds).ThenInclude(ca => ca.Photos).ToListAsync();
            //lazy loading
            var users = await _context.Users.ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}