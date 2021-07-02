using FreeAds.API.Data;
using FreeAds.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreeAds.API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IClassifiedAdsRepository _repo;

        public AdminController(UserManager<AppUser> userManager, IClassifiedAdsRepository repo)
        {
            _userManager = userManager;
            _repo = repo;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("user-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
                    City = u.City,
                    ClassifiedAds = u.ClassifiedAds.Select(ca => new
                    {
                        Id = ca.Id,
                        Title = ca.Title,
                        Description = ca.Description,
                        City = ca.City,
                        Category = ca.Category,
                        DateAdded = ca.DateAdded,
                        Email = ca.Email,
                        Phone = ca.Phone,
                        Status = ca.Status,
                        Photos = ca.Photos.Select(p => new
                        {
                            Id = p.Id,
                            ClassifiedAdsId = p.ClassifiedAdsId,
                            DateAdded = p.DateAdded,
                            Url = p.Url,
                            IsMain = p.IsMain,
                            PublicId = p.PublicId,
                            Status = p.Status,
                        }),
                        UserId = ca.AppUserId,
                    })
                })
                .ToListAsync();

            return Ok(new { users = users } );
        }

/*        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("user-with-roles-and-classified-ads-with-photos")]
        public async Task<ActionResult> GetUsersWithRolesAndClassifiedAdsWithPhotos()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
                    ClassifiedAds = u.ClassifiedAds.Select(ca => new
                    {
                        Id = ca.Id,
                        Title = ca.Title,
                        Description = ca.Description,
                        City = ca.City,
                        Category = ca.Category,
                        DateAdded = ca.DateAdded,
                        Email = ca.Email,
                        Phone = ca.Phone,
                        Status = ca.Status,
                        Photos = ca.Photos.Select(p => new
                        {
                            Id = p.Id,
                            ClassifiedAdsId = p.ClassifiedAdsId,
                            DateAdded = p.DateAdded,
                            Url = p.Url,
                            IsMain = p.IsMain,
                            PublicId = p.PublicId,
                            Status = p.Status,
                        }),
                        UserId = ca.AppUserId,
                    })
                })
                .ToListAsync();

            return Ok(users);
        }*/

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{id}")]
        public async Task<ActionResult> EditRoles(string id, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(new { status_code = StatusCodes.Status200OK, roles = await _userManager.GetRolesAsync(user) });
        }

        [Authorize(Policy = "ModerateAdsRole")]
        [HttpPost("approve-or-disaprove-classified-ads")]
        public async Task<ActionResult> ApproveOrDisapproveClassifiedAds([FromForm] Guid id, [FromForm] string approveOrDisapprove)
        {
            var classifiedAdsToApprove = await _repo.ApproveOrDisapproveClassifiedAds(id, approveOrDisapprove);

            await _repo.SaveAll();

            return Ok(new { classifiedad = classifiedAdsToApprove } );
        }

        [Authorize(Policy = "ModerateAdsRole")]
        [HttpPost("approve-or-disaprove-classified-ads-and-photos")]
        public async Task<ActionResult> ApproveOrDisapproveClassifiedAdsAndPhotos([FromForm] Guid id, [FromForm] string approveOrDisapprove)
        {
            var classifiedAdsAndPhotosToApprove = await _repo.ApproveOrDisapproveClassifiedAdsAndPhotos(id, approveOrDisapprove);

            await _repo.SaveAll();

            return Ok(new { status_code = StatusCodes.Status200OK, approvedOrDissapproved = classifiedAdsAndPhotosToApprove.Status });
        }

        [Authorize(Policy = "ModerateAdsRole")]
        [HttpGet("ads-to-moderate")]
        public ActionResult GetAdsForModeration()
        {
            var AdsForModeration = _repo.GetAdsForModeration();

            return Ok(AdsForModeration);
        }

        [Authorize(Policy = "ModerateAdsRole")]
        [HttpPost("approve-or-disaprove-photos")]
        public async Task<ActionResult> ApproveOrDisapprovePhotos([FromForm] Guid id, [FromForm] string approveOrDisapprove)
        {
            var classifiedAdsToApprove = _repo.ApproveOrDisapprovePhoto(id, approveOrDisapprove);

            await _repo.SaveAll();

            return Ok(new { classifiedad = classifiedAdsToApprove });
        }

        /*[Authorize(Policy = "ModerateAdsRole")]
        [HttpDelete("photo-to-moderate/{classifiedAdId}/{photoId}")]
        public async Task<IActionResult> DeletePhoto(string userId, Guid classifiedAdId, Guid photoId)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Unauthorized();
            }

            var classfiedAdFromRepo = await _classifiedAdsRepo.GetClassifiedAdDetail(classifiedAdId);

            if (!classfiedAdFromRepo.Photos.Any(p => p.Id == photoId))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _classifiedAdsRepo.GetPhoto(photoId);

            if (photoFromRepo.IsMain)
            {
                //return BadRequest("You cannot delete main photo");
                return BadRequest("Не можете да ја бришете главната слика");
            }

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _classifiedAdsRepo.Delete(photoFromRepo.Id);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _classifiedAdsRepo.Delete(photoFromRepo.Id);
            }

            if (await _classifiedAdsRepo.SaveAll())
            {
                return Ok();
            }

            //return BadRequest("Failed to delete the photo");
            return BadRequest("Грешка при бришење на сликата");
        }*/
    }
}
