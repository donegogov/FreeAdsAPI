using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FreeAds.API.Data;
using FreeAds.API.Dtos;
using FreeAds.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FreeAds.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUserRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _repo = repo;
        }

        /*[Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }*/

        [Authorize(Policy = "MemberAdsRole")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromForm]string id)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            var userToReturn = _mapper.Map<UserForListDto>(user);

            return Ok(userToReturn);
        }

        [Authorize(Policy = "MemberAdsRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UserForUpdateDto userForUpdateDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return Unauthorized();
            }

            var vUserFromRepo = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            vUserFromRepo.version += 1;
            vUserFromRepo.UserName = "v" + vUserFromRepo.version.ToString() + " " + vUserFromRepo.UserName;

            var result = await _userManager.CreateAsync(vUserFromRepo);

            if (!result.Succeeded) return BadRequest(result.Errors);

            //_mapper.Map(userForUpdateDto, userFromRepo);

            var userFromRepo = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            userFromRepo.UserName = userForUpdateDto.UserName;
            userFromRepo.Email = userForUpdateDto.Email;
            userFromRepo.City = userForUpdateDto.City;

            result = await _userManager.UpdateAsync(userFromRepo);

            if (!result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return Ok("Корисничките податоци се ажурирани");
            }

            //throw new Exception($"Updating user {id} failed on save");
            throw new Exception($"Грешка при зачувувањето на промените на корисникот {id}");
        }
    }
}