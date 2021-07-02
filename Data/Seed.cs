using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeAds.API.Models;
using FreeAds.API.Models.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FreeAds.API.Data
{
    public class Seed
    {
        //.net core 2.1
        /* private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        } */

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<AppUser>>(userData);

                var roles = new List<AppRole>
                    {
                        new AppRole{Name="Member"},
                        new AppRole{Name="Admin"},
                        new AppRole{Name="Moderator"},
                    };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                foreach (var user in users)
                {
                    /*byte[] passwordHash, passwordSalt;*/

                    /*CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;*/
                    user.UserName = user.UserName.ToLower();

                    foreach(var ca in user.ClassifiedAds)
                    { 
                        ca.DateAdded = DateTime.Now;
                        ca.Status = "Approved";
                        foreach(var photo in ca.Photos)
                        {
                            photo.Status = "Approved";
                        }
                    }
                    
                    await userManager.CreateAsync(user, "Password123");
                    await userManager.AddToRoleAsync(user, "Member");
                }

                var admin = new AppUser
                {
                    UserName = "admin",
                    City = "Veles"

                };

                await userManager.CreateAsync(admin, "P@ssw0rd");
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
        }

        /*private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }*/

        public static void SeedCategories(DataContext context)
        {
            if (!context.Categories.Any())
            {
                /*
                List<String> CategoryName = new List<String>();
                List<String> CategoryValue = new List<String>();

                CategoryName.Add("PC");
                CategoryValue.Add("PC");

                CategoryName.Add("Lap Top");
                CategoryValue.Add("Lap Top");

                CategoryName.Add("TV");
                CategoryValue.Add("TV");

                CategoryName.Add("Phone");
                CategoryValue.Add("Phone");
                */

                Dictionary<String, String> Category = new Dictionary<string, string>();
                //Category.Add("Сите категории", "Сите категории");
                context.Categories.Add(
                        new Category("Сите категории", "Сите категории")
                        );
                context.SaveChanges();
                Category.Add("Мебел", "Мебел");
                Category.Add("Бела Техника", "Бела Техника");
                Category.Add("Електроника", "Електроника");
                Category.Add("Останато", "Останато");
                
                foreach (KeyValuePair<string, string> entry in Category)
                {
                    // do something with entry.Value or entry.Key
                    context.Categories.Add(
                        new Category(entry.Key, entry.Value)
                        );
                }
                //context.Categories.OrderBy(c => c.CategoryName.Equals("Сите категории"));
                context.SaveChanges();
            }
        }

        public static void SeedCites(DataContext context)
        {
            if (!context.Cities.Any())
            {
                /*
                List<String> CategoryName = new List<String>();
                List<String> CategoryValue = new List<String>();

                CategoryName.Add("PC");
                CategoryValue.Add("PC");

                CategoryName.Add("Lap Top");
                CategoryValue.Add("Lap Top");

                CategoryName.Add("TV");
                CategoryValue.Add("TV");

                CategoryName.Add("Phone");
                CategoryValue.Add("Phone");
                */

                Dictionary<String, String> City = new Dictionary<string, string>();
                //City.Add("Сите градови", "Сите градови");
                context.Cities.Add(
                        new City("Сите градови", "Сите градови")
                        );
                context.SaveChanges();
                context.Cities.Add(
                        new City("Скопје", "Скопје")
                        );
                context.SaveChanges();
                context.Cities.Add(
                        new City("Велес", "Велес")
                        );
                context.SaveChanges();
                City.Add("Берово", "Берово");
                City.Add("Битола", "Битола");
                City.Add("Валандово", "Валандово");
                //City.Add("Велес", "Велес");

                City.Add("Виница", "Виница");
                City.Add("Гевгелиjа", "Гевгелиjа");
                City.Add("Гостивар", "Гостивар");
                City.Add("Дебар", "Дебар");

                City.Add("Делчево", "Делчево");
                City.Add("Демир Хисар", "Демир Хисар");
                City.Add("Кавадарци", "Кавадарци");
                City.Add("Кичево", "Кичево");

                City.Add("Кочани", "Кочани");
                City.Add("Кратово", "Кратово");
                City.Add("Крива Паланка", "Крива Паланка");
                City.Add("Крушево", "Крушево");

                City.Add("Куманово", "Куманово");
                City.Add("Македонски Брод", "Македонски Брод");
                City.Add("Неготино", "Неготино");
                City.Add("Охрид", "Охрид");

                City.Add("Прилеп", "Прилеп");
                City.Add("Пробиштип", "Пробиштип");
                City.Add("Радовиш", "Радовиш");
                City.Add("Ресен", "Ресен");

                City.Add("Свети Николе", "Свети Николе");
                //City.Add("Скопjе", "Скопjе");
                City.Add("Струга", "Струга");
                City.Add("Струмица", "Струмица");

                City.Add("Тетово", "Тетово");
                City.Add("Штип", "Штип");

                foreach (KeyValuePair<string, string> entry in City)
                {
                    // do something with entry.Value or entry.Key
                    context.Cities.Add(
                        new City(entry.Key, entry.Value)
                        );
                }
                //context.Cities.OrderBy(c => c.CityName.Equals("Сите градови"));
                context.SaveChanges();
            }
        }
    }
}