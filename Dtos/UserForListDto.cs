using System.Collections.Generic;
using FreeAds.API.Models;

namespace FreeAds.API.Dtos
{
    public class UserForListDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public ICollection<ClassifiedAdsForUserDto> ClassifiedAds {get; set; }
        //public string[] Roles;
        //public ICollection<AppUserRole> UserRoles { get; set; }
    }
}