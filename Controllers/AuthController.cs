using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FreeAds.API.Data;
using FreeAds.API.Dtos;
using FreeAds.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FreeAds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserForRegisterDto userForRegisterDto)
        {
            // validate request podocno kje dodademe validacija

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == userForRegisterDto.Username.ToLower());

            if (user != null)
                //return BadRequest("Username already exists");
                return BadRequest("Корисничкото име веќе постои");
            
            var userToCreate = new AppUser
            {
                UserName = userForRegisterDto.Username.ToLower(),
                City = userForRegisterDto.City
            };

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(userToCreate, "Member");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == userForLoginDto.Username.ToLower());

            if (user == null)
                return Unauthorized("Грешно корисничко име");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.StateOrProvince, user.City)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                Id = user.Id,
                Username = user.UserName,
                Token = tokenHandler.WriteToken(token),
                City = user.City
            });
        }

        /*[HttpPost("token")]
        public IActionResult CreateToken(UserForTokenDto userForToken)
        {
            if(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) != userForToken.id)
                return Unauthorized();
            
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            string userRole = User.FindFirst(ClaimTypes.Role).Value;
            string city = User.FindFirst(ClaimTypes.StateOrProvince).Value;

            var token = _repo.CreateToken(userForToken, key, userRole, city);

            var tokenHandler = new JwtSecurityTokenHandler();

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }*/
    }
}