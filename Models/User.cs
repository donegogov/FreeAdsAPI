using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FreeAds.API.enums;
using Microsoft.AspNetCore.Identity;

namespace FreeAds.API.Models
{
    public class AppUser : IdentityUser
    {
        //public int Id { get; set; }
        //public string Username { get; set; }
        public string City { get; set; }
        //public byte[] PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        //public string UserRole { get; set; }
        public string Deleted { get; set; }
        public int version { get; set; } = 0;
        public virtual ICollection<ClassifiedAds> ClassifiedAds { get; set; }
        public virtual ICollection<Like> LikedClassifiedAds { get; set; }
        public virtual ICollection<Message> MessagesSent { get; set; }
        public virtual ICollection<Message> MessagesReceived { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }

    }
}