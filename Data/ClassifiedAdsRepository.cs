using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeAds.API.Dtos;
using FreeAds.API.Helpers;
using FreeAds.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreeAds.API.Data
{
    public class ClassifiedAdsRepository : IClassifiedAdsRepository
    {
        private readonly DataContext _context;
        public ClassifiedAdsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ClassifiedAds> Add(ClassifiedAds classifiedAds)
        {
            classifiedAds.Status = "Pending";

            await _context.ClassifiedAds.AddAsync(classifiedAds);

            //await _context.SaveChangesAsync();

            return classifiedAds;
        }

        //public void Delete<T>(T entity) where T : class
        //{
        //    _context.Remove(entity);
        //}

        public async Task<bool> Delete(Guid id)
        {
            var classifiedAdsToDeleteAttach = new ClassifiedAds();
            classifiedAdsToDeleteAttach.Id = id;

            _context.ClassifiedAds.Attach(classifiedAdsToDeleteAttach);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id);

            classifiedAdsToDeleteAttach.Status = "UserIsDeletedToDeleteClassifiedAds";
            _context.ClassifiedAds.Update(classifiedAdsToDeleteAttach);

            await SaveAll();

            return true;
        }

        public async Task<bool> AdminDelete(Guid id)
        {
            var classifiedAdsToDeleteAttach = new ClassifiedAds();
            classifiedAdsToDeleteAttach.Id = id;

            _context.ClassifiedAds.Attach(classifiedAdsToDeleteAttach);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id);

            classifiedAdsToDeleteAttach.Status = "AdminDeletedClassifiedAds";
            _context.ClassifiedAds.Update(classifiedAdsToDeleteAttach);

            await SaveAll();

            return true;
        }

        public async Task<bool> DeletePhoto(Photo photo)
        {
            //var photoToDeleteAttach = new Photo();
            //photoToDeleteAttach.Id = id;

            _context.Photos.Attach(photo);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id);

            photo.Status = "UserIsDeletedToDeleteUserPhoto";
            _context.Photos.Update(photo);

            return true;
        }

        public async Task<bool> AdminDeletePhoto(Guid id)
        {
            var photoToDeleteAttach = new Photo();
            photoToDeleteAttach.Id = id;

            _context.Photos.Attach(photoToDeleteAttach);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id); UserIsDeletedToDeleteUserPhoto AdminDeletedPhoto

            photoToDeleteAttach.Status = "AdminDeletedPhoto";
            _context.Photos.Update(photoToDeleteAttach);

            await SaveAll();

            return true;
        }

        public async Task<bool> DeleteLike(Like like)
        {
            //var likeToDeleteAttach = new Like();
            //likeToDeleteAttach.Id = id;

            //_context.Likes.Attach(like);
            //var classifiedAdsToDelete = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id == id);

            //photoToDeleteAttach.Deleted = "UserIsDeletedToDelete";
            _context.Likes.Remove(like);

            //await SaveAll();

            return await SaveAll();
        }

        public async Task<ClassifiedAds> GetClassifiedAdDetail(Guid id)
        {
            var classifiedAd = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id.ToString().ToUpper() == id.ToString() && ca.Status == "Approved");

            var photos = classifiedAd.Photos.AsEnumerable();

            photos = photos.Where(photo => photo.Status == "Approved").ToList();

            if(photos != null)
            {
                Collection<Photo> collection = new Collection<Photo>(photos.ToList());

                classifiedAd.Photos = collection;
            }
            

            //classifiedAd.Category = ConvertEngToMkd(classifiedAd.Category);

            //classifiedAd.City = ConvertEngToMkd(classifiedAd.City);

            return classifiedAd;
        }

        public async Task<ClassifiedAds> GetClassifiedAdDetaiForUser(Guid id)
        {
            var classifiedAd = await _context.ClassifiedAds.Include(p => p.Photos.Where(p1 => p1.Status != "UserIsDeletedToDeleteUserPhoto")).FirstOrDefaultAsync(ca => ca.Id.ToString().ToUpper() == id.ToString());

            //classifiedAd.Category = ConvertEngToMkd(classifiedAd.Category);

            //classifiedAd.City = ConvertEngToMkd(classifiedAd.City);

            return classifiedAd;
        }

        public async Task<ClassifiedAds> GetClassifiedAdDetailAndPhotos(Guid id)
        {
            var classifiedAd = await _context.ClassifiedAds.FirstOrDefaultAsync(ca => ca.Id.ToString().ToUpper() == id.ToString().ToUpper());

            return classifiedAd;
        }

        public PagedList<ClassifiedAds> GetClassifiedAds(ClassifiedAdsParams classifiedAdsParams)
        {
            var classifiedAds =  _context.ClassifiedAds.AsEnumerable().Where(vd => vd.DateAdded.CalculateValidTo() && vd.Status == "Approved").OrderByDescending(ca => ca.DateAdded).ToList();

            return PagedList<ClassifiedAds>.Create(classifiedAds, classifiedAdsParams.PageNumber, classifiedAdsParams.PageSize);
        }

        public async Task<PagedList<ClassifiedAds>> GetUserLikedClassifiedAds(ClassifiedAdsParams classifiedAdsParams)
        {
            var userLikes = await GetUserLikesClassifiedAds(classifiedAdsParams.userId);

            var classifiedAds = await _context.ClassifiedAds.Where(ca => userLikes.Contains(ca.Id) && ca.Status == "Approved").ToListAsync();

            return PagedList<ClassifiedAds>.Create(classifiedAds, classifiedAdsParams.PageNumber, classifiedAdsParams.PageSize);
        }

        public async Task<IEnumerable<Guid>> GetUserLikesClassifiedAds(string userId)
        {
            return await _context.Likes.Where(l => l.LikerUserId == userId).Select(l => l.LikedClassifiedAdsId).ToListAsync();
        }

        public PagedList<ClassifiedAds> GetRelevantClassifiedAds(string city, ClassifiedAdsParams classifiedAdsParams)
        {
            var classifiedAds = _context.ClassifiedAds.AsEnumerable().Where(vd => vd.DateAdded.CalculateValidTo() && vd.AppUserId != classifiedAdsParams.userId && vd.Status == "Approved").OrderByDescending(ca => ca.City.Equals(city)).ToList();

            return PagedList<ClassifiedAds>.Create(classifiedAds, classifiedAdsParams.PageNumber, classifiedAdsParams.PageSize);
            
            //classifiedAds = classifiedAds.OrderByDescending(ca => ca.City.Equals(city)).ToList();
            //var top5ClassifiedAdsOrderByCity = await classifiedAds.OrderBy(ca => ca.City.Equals(city)).Take(5).ToListAsync();
        }

        public async Task<IEnumerable<ClassifiedAds>> GetClassifiedAdsForUser(string userId)
        {
            var classifiedAds = await _context.ClassifiedAds.Include(p => p.Photos).Where(uid => uid.AppUserId == userId).ToListAsync();

            return classifiedAds;
        }

        public async Task<IEnumerable<ClassifiedAds>> SearchClassifiedAds(SearchQueryParametarsDto searchQueryParametars, string? userId)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            List<string> queryWords = searchQueryParametars.Query.Split(delimiterChars).ToList();
            List<ClassifiedAds> classifiedAdsFromSearch = new List<ClassifiedAds>();

            if (searchQueryParametars.City.Equals("Сите градови") && searchQueryParametars.Category.Equals("Сите категории"))
            {
                classifiedAdsFromSearch =
                    (from ca in _context.ClassifiedAds.AsEnumerable()
                     where ca.DateAdded.CalculateValidTo()
                         // && ca.City == searchQueryParametars.City
                         // && ca.Category == searchQueryParametars.Category
                         && (queryWords.Any(str => ca.Title.Contains(str, StringComparison.OrdinalIgnoreCase))
                         || queryWords.Any(str => ca.Description.Contains(str, StringComparison.OrdinalIgnoreCase)))
                     select ca
                    ).ToList();
            }
            else if (searchQueryParametars.City.Equals("Сите градови") && !searchQueryParametars.Category.Equals("Сите категории"))
            {
                classifiedAdsFromSearch =
                    (from ca in _context.ClassifiedAds.AsEnumerable()
                     where ca.DateAdded.CalculateValidTo()
                         // && ca.City == searchQueryParametars.City
                         && ca.Category == searchQueryParametars.Category
                         && (queryWords.Any(str => ca.Title.Contains(str, StringComparison.OrdinalIgnoreCase))
                         || queryWords.Any(str => ca.Description.Contains(str, StringComparison.OrdinalIgnoreCase)))
                     select ca
                    ).ToList();
            }
            else if (!searchQueryParametars.City.Equals("Сите градови") && searchQueryParametars.Category.Equals("Сите категории"))
            {
                classifiedAdsFromSearch =
                    (from ca in _context.ClassifiedAds.AsEnumerable()
                     where ca.DateAdded.CalculateValidTo()
                         && ca.City == searchQueryParametars.City
                         // && ca.Category == searchQueryParametars.Category
                         && (queryWords.Any(str => ca.Title.Contains(str, StringComparison.OrdinalIgnoreCase))
                         || queryWords.Any(str => ca.Description.Contains(str, StringComparison.OrdinalIgnoreCase)))
                     select ca
                    ).ToList();
            }
            else if (!searchQueryParametars.City.Equals("Сите градови") && !searchQueryParametars.Category.Equals("Сите категории"))
            {
                classifiedAdsFromSearch =
                    (from ca in _context.ClassifiedAds.AsEnumerable()
                     where ca.DateAdded.CalculateValidTo()
                         && ca.City == searchQueryParametars.City
                         && ca.Category == searchQueryParametars.Category
                         && (queryWords.Any(str => ca.Title.Contains(str, StringComparison.OrdinalIgnoreCase))
                         || queryWords.Any(str => ca.Description.Contains(str, StringComparison.OrdinalIgnoreCase)))
                     select ca
                    ).ToList();
            }
            if (userId != null)
            {
                var classifiedAdsToReturn = classifiedAdsFromSearch.Where(cads => cads.AppUserId != userId);

                return classifiedAdsToReturn;
            }
            classifiedAdsFromSearch = classifiedAdsFromSearch.Where(ca => ca.Status == "Approved").OrderByDescending(ca => ca.DateAdded).ToList();

            return classifiedAdsFromSearch;
        }

        public async Task<ClassifiedAds> ApproveOrDisapproveClassifiedAds(Guid classifiedAdId, string approvedOrDisapproved)
        {
            var classifiedAds = await _context.ClassifiedAds.Where(ca => ca.Id == classifiedAdId && ca.Status == "Pending").FirstOrDefaultAsync();

            classifiedAds.Status = approvedOrDisapproved;

            return classifiedAds;
        }

        public async Task<ClassifiedAds> ApproveOrDisapproveClassifiedAdsAndPhotos(Guid classifiedAdId, string approvedOrDisapproved)
        {
            var classifiedAds = await _context.ClassifiedAds.Where(ca => ca.Id == classifiedAdId && ca.Status == "Pending").FirstOrDefaultAsync();

            classifiedAds.Status = approvedOrDisapproved;

            foreach(var photo in classifiedAds.Photos)
            {
                photo.Status = approvedOrDisapproved;
            }

            return classifiedAds;
        }

        public IEnumerable<ClassifiedAds> GetAdsForModeration()
        {
            var classifiedAds = _context.ClassifiedAds.Where(ca => ca.Status == "Pending").ToList();

            return classifiedAds;
        }

        public async Task<Photo> ApproveOrDisapprovePhoto(Guid photoId, string approvedOrDisapproved)
        {
            var photo = await _context.Photos.Where(p => p.Id == photoId && p.Status == "Pending").FirstOrDefaultAsync();

            photo.Status = approvedOrDisapproved;

            return photo;
        }

        public async Task<bool> SaveAll()
    {
        return await _context.SaveChangesAsync() > 0;
    }

        public async Task<Photo> GetPhoto(Guid id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhotoForClassifiedAd(Guid classfiedAdId)
        {
            var mainPhoto = await _context.Photos.Where(p => p.ClassifiedAdsId == classfiedAdId && p.Status == "Approved").FirstOrDefaultAsync(p => p.IsMain);

            return mainPhoto;
        }

        public async Task<Photo> GetMainPhotoForClassifiedAdForUserUpdate(Guid classfiedAdId)
        {
            var mainPhoto = await _context.Photos.Where(p => p.ClassifiedAdsId == classfiedAdId).FirstOrDefaultAsync(p => p.IsMain);

            return mainPhoto;
        }

        public async Task<Like> GetLike(string userId, Guid classifiedAdId)
        {
            return await _context.Likes.FirstOrDefaultAsync(l => l.LikerUserId == userId && l.LikedClassifiedAdsId == classifiedAdId);
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public async Task<int> GetNumberOfLikesOfClassifiedAd(Guid classifiedAdId)
        {
            return await _context.Likes.CountAsync(l => l.LikedClassifiedAdsId == classifiedAdId);

        }

        public List<ClassifiedAdsForSearchDto> GetClassifiedAdsForSearch()
        {
            var classifiedAds = _context.ClassifiedAds.Include(p => p.Photos.Where(p => p.IsMain && p.Url != null)).AsEnumerable().Where(vd => vd.DateAdded.CalculateValidTo() && vd.Status == "Approved" && vd.Title != null).ToList();

            List<ClassifiedAdsForSearchDto> classifiedAdsForSearch = classifiedAds.OrderByDescending(ca => ca.DateAdded).ToList()
                .Select(ca => new ClassifiedAdsForSearchDto
                {
                    MainPhotoUrl = ca.Photos.FirstOrDefault(photo => photo.IsMain).Url,
                    Title = ca.Title
                })
                .ToList();

            return classifiedAdsForSearch;
        }

        /*public String ConvertEngToMkd(String input)
        {
            String expectedMacedonianKirilicaLowerCase = "а б в г д ѓ е ж з ѕ и ј к л љ м н њ о п р с т ќ у ф х ц ч џ ш";

            String expectedMacedonianKirilicaUpperCase = "А Б В Г Д Ѓ Е Ж З Ѕ И Ј К Л Љ М Н Њ О П Р С Т Ќ У Ф Х Ц Ч Џ Ш";

            String expectedMacedonianLatinLowerCase = "a b v g d gjs e zhs z dzs i j k l ljs m n njs o p r s t kjs u f h c chs djs shs";

            String expectedMacedonianLatinUpperCase = expectedMacedonianLatinLowerCase.ToUpper();

            String []expectedMacedonianKirilicaLowerCaseArray = expectedMacedonianKirilicaLowerCase.Split(" ");

            String []expectedMacedonianKirilicaUpperCaseArray = expectedMacedonianKirilicaUpperCase.Split(" ");

            String []expectedMacedonianLatinLowerCaseArray = expectedMacedonianLatinLowerCase.Split(" ");

            String []expectedMacedonianLatinUpperCaseArray = expectedMacedonianLatinUpperCase.Split(" ");

            String result = "";
            bool secondUpperLetter = true;

            for(int i = 0; i < input.Length; i++)
            {
                String c = input[i].ToString();

                if(i < input.Length - 2)
                {
                    if(("s".Equals(input[i+2]) || "S".Equals(input[i+2])) && ("gjs".Contains(String.Concat(c, input[i+1], input[i+2])) || 
                    "zhs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "dzs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "ljs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "njs".Contains(String.Concat(c, input[i+1], input[i+2])) || 
                    "kjs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "chs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "djs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "shs".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "ZHS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "DZS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "LJS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "NJS".Contains(String.Concat(c, input[i+1], input[i+2])) || 
                    "KJS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "CHS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "DJS".Contains(String.Concat(c, input[i+1], input[i+2])) ||
                    "SHS".Contains(String.Concat(c, input[i+1], input[i+2]))))
                    {
                        c = input.Substring(i, i+2);
                        i += 2;
                    }
                }

                int charPosition = Array.IndexOf(expectedMacedonianLatinUpperCaseArray, c.ToString());

                if(charPosition == -1)
                {
                    charPosition = Array.IndexOf(expectedMacedonianLatinLowerCaseArray, c.ToString());

                    if(charPosition == -1)
                    {
                        return input;
                    }
                    else
                    {
                        result += expectedMacedonianKirilicaLowerCaseArray[charPosition];
                    }
                }
                else
                {
                    if(!secondUpperLetter)
                    {
                        result += " ";
                        result += expectedMacedonianKirilicaUpperCaseArray[charPosition];
                        continue;
                    }
                    else
                    {
                        result += expectedMacedonianKirilicaUpperCaseArray[charPosition];
                        secondUpperLetter = false;
                    }
                }

                
            }

            return result;
        }*/
    }
}