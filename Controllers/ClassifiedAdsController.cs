using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FreeAds.API.Data;
using FreeAds.API.Dtos;
using FreeAds.API.Helpers;
using FreeAds.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreeAds.API.Controllers
{
    [Authorize]
    public class ClassifiedAdsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IClassifiedAdsRepository _repo;
        private readonly IMapper _mapper;

        public ClassifiedAdsController(UserManager<AppUser> userManager, IClassifiedAdsRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetClassifiedAds([FromQuery]ClassifiedAdsParams classifiedAdsParams)
        {
            var classifiedAds = _repo.GetClassifiedAds(classifiedAdsParams);

            var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsDto>>(classifiedAds);

            Response.AddPagination(classifiedAds.CurrentPage, classifiedAds.PageSize, classifiedAds.TotalCount, classifiedAds.TotalPages);

            return Ok(classifiedAdsToReturn);
        }

        [HttpGet("relevant")]
        public IActionResult GetRelevantClassifiedAds([FromQuery]ClassifiedAdsParams classifiedAdsParams)
        {
            /*if (classifiedAdsParams.userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Unauthorized();
            }*/

            var city = User.FindFirst(ClaimTypes.StateOrProvince).Value;

            var classifiedAds = _repo.GetRelevantClassifiedAds(city, classifiedAdsParams);

            /*var classifiedAdsToReturnWithLiked = classifiedAds;

            var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsDto>>(classifiedAds);

            foreach(var ca in classifiedAdsToReturn)
            {
                ca.IsLiked = classifiedAdsToReturnWithLiked.FirstOrDefault(lus => lus.LikersUsers.Where(lu => lu.LikerUserId == classifiedAdsParams.userId).FirstOrDefault() != null) != null ? true : false;
            }*/

            var classifiedAdsToReturn = new List<ClassifiedAdsDto>();
            foreach(var ca in classifiedAds)
            {
                ClassifiedAdsDto classifiedAd = new ClassifiedAdsDto();
                classifiedAd.Id = ca.Id;
                classifiedAd.Title = ca.Title;
                classifiedAd.Description = ca.Description;
                classifiedAd.City = ca.City;
                classifiedAd.Age = ca.DateAdded.CalculateAge();
                classifiedAd.PhotoUrl = ca.Photos.FirstOrDefault(p => p.IsMain).Url;
                classifiedAd.AppUserId = ca.AppUserId;
                classifiedAd.IsLiked = ca.LikersUsers.Where(lu => lu.LikerUserId == classifiedAdsParams.userId).FirstOrDefault() != null ? true : false;

                classifiedAdsToReturn.Add(classifiedAd);
            }

            Response.AddPagination(classifiedAds.CurrentPage, classifiedAds.PageSize, classifiedAds.TotalCount, classifiedAds.TotalPages);

            return Ok(classifiedAdsToReturn);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassifiedAdDetail(Guid id)
        {
            var classifiedAds = await _repo.GetClassifiedAdDetail(id);

            var classifiedAdsToReturn = _mapper.Map<ClassifiedAdsForDetailedDto>(classifiedAds);

            return Ok(classifiedAdsToReturn);
        }


        [HttpPost("user-created-ads")]
        public async Task<IActionResult> GetClassifiedAdForUserSingleAds([FromForm] Guid id)
        {
            /*var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return Unauthorized();
            }*/

            var classifiedAds = await _repo.GetClassifiedAdDetaiForUser(id);

            var classifiedAdsToReturn = _mapper.Map<ClassifiedAdsForDetailedDto>(classifiedAds);

            return Ok(classifiedAdsToReturn);
        }

        [HttpPost("user")]
        public async Task<IActionResult> GetClassifiedAdForUser([FromForm]string id)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return Unauthorized();
            }

            var classifiedAds = await _repo.GetClassifiedAdsForUser(id);

            var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsForUserDto>>(classifiedAds);

            return Ok(classifiedAdsToReturn);
        }

        [AllowAnonymous]
        [HttpPost("search/{userId?}")]
        public async Task<IActionResult> SearchClassifiedAdForUser(SearchQueryParametarsDto searchQueryParametars, string? userId = null)
        {
            if (userId != null)
            {
                var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                {
                    return Unauthorized();
                }
            }

            var classifiedAds = await _repo.SearchClassifiedAds(searchQueryParametars, userId);

            if (userId != null)
            {
                var classifiedAdsToReturnForUser = classifiedAds.Select(ca => new
                {
                    Id = ca.Id,
                    Title = ca.Title,
                    Description = ca.Description,
                    City = ca.City,
                    Age = ca.DateAdded.CalculateAge(),
                    PhotoUrl = ca.Photos.FirstOrDefault(p => p.IsMain).Url,
                    AppUserId = ca.AppUserId,
                    IsLiked = ca.LikersUsers.Where(lu => lu.LikerUserId == userId).FirstOrDefault() != null ? true : false
                });
                /*var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsDto>>(classifiedAds);*/

                return Ok(classifiedAdsToReturnForUser);
            }

            var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsDto>>(classifiedAds);

            return Ok(classifiedAdsToReturn);
        }

        [HttpPut("add")]
        public async Task<IActionResult> Register([FromForm] ClassifiedAdsForRegisterDto classifiedAdForRegisterDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == classifiedAdForRegisterDto.AppUserId);

            if (user == null)
            {
                return Unauthorized();
            }

            ClassifiedAds classifiedAd = new ClassifiedAds
            {
                Title = classifiedAdForRegisterDto.Title,
                Description = classifiedAdForRegisterDto.Description,
                City = classifiedAdForRegisterDto.City,
                Category = classifiedAdForRegisterDto.Category,
                Email = classifiedAdForRegisterDto.Email,
                Phone = classifiedAdForRegisterDto.Phone,
                DateAdded = DateTime.Now,
                ValidTo = DateTime.Today.AddMonths(1),
                AppUserId = classifiedAdForRegisterDto.AppUserId
            };

            await _repo.Add(classifiedAd);

            if (await _repo.SaveAll())
            {
                var classifiedAdsToReturn = _mapper.Map<ClassifiedAdsForRegisterDto>(classifiedAd);

                return Ok(classifiedAdsToReturn);
            }

            //return BadRequest("Failed to add the Classified Ad");
            return BadRequest("Грешка при додавање на огласот");
        }

        /*[HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _repo.Delete(id);
        }*/

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClassifiedAds(string id, ClassifiedAdsForUserUpdate classifiedAdsForUserUpdate)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return Unauthorized();
            }

            var classifiedAdsForUserUpdateFromRepo = await _repo.GetClassifiedAdDetail(classifiedAdsForUserUpdate.Id);

            if (classifiedAdsForUserUpdateFromRepo.AppUserId != id)
            {
                return Unauthorized();
            }

            _mapper.Map(classifiedAdsForUserUpdate, classifiedAdsForUserUpdateFromRepo);

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            //throw new Exception($"Updating classified ad {id} failed on save");
            throw new Exception($"Зачувувањето на огласот со ид= {id} не беше успешно");
        }

        [Produces("application/json")]
        [HttpPost("{userId}/like/{classifiedAdId}")]
        public async Task<IActionResult> LikeClassifiedAd(string userId, Guid classifiedAdId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var like = await _repo.GetLike(userId, classifiedAdId);

            if (like != null)
            {
                if (await _repo.DeleteLike(like))
                    //return Ok("You have disliked classified ad ");
                    return Ok("Успешно недопаѓање на огласот ");

                //return BadRequest("Failed to unlike the classified ad");
                return BadRequest("Грешно недопаѓање на огласот");
            }

            if (await _repo.GetClassifiedAdDetail(classifiedAdId) == null)
                return NotFound();

            like = new Like
            {
                LikerUserId = userId,
                LikedClassifiedAdsId = classifiedAdId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
                //return Ok("You have liked classified ad ");
                return Ok("Допаѓањето на огласот беше успешно ");

            //return BadRequest("Failed to like the classified ad");
            return BadRequest("Грешка при допаѓање на огласот");
        }

        [HttpGet("user/likes")]
        public async Task<IActionResult> GetLikedClassifiedAds([FromQuery]ClassifiedAdsParams classifiedAdsParams)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == classifiedAdsParams.userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var classifiedAds = await _repo.GetUserLikedClassifiedAds(classifiedAdsParams);

            var classifiedAdsToReturn = _mapper.Map<IEnumerable<ClassifiedAdsDto>>(classifiedAds);

            Response.AddPagination(classifiedAds.CurrentPage, classifiedAds.PageSize, classifiedAds.TotalCount, classifiedAds.TotalPages);

            return Ok(classifiedAdsToReturn);
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet("likes/{classifiedAdId}")]
        public async Task<IActionResult> GetNumberOfLikesClassifiedAds(Guid classifiedAdId)
        {
            if (await _repo.GetClassifiedAdDetail(classifiedAdId) == null)
                return NotFound();

            var classifiedAds = await _repo.GetNumberOfLikesOfClassifiedAd(classifiedAdId);

            return Ok(classifiedAds);
        }

        [AllowAnonymous]
        [HttpGet("get-classified-ads-for-search")]
        public IActionResult GetClassifiedAdsForSearch()
        {
            var classifiedAds = _repo.GetClassifiedAdsForSearch();

            return Ok(new { status_code = StatusCodes.Status200OK, classifiedAds = classifiedAds });
        }
    }
}