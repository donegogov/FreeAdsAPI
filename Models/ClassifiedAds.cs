using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreeAds.API.enums;
using Microsoft.EntityFrameworkCore;

namespace FreeAds.API.Models
{
    public class ClassifiedAds
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime ValidTo { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string Status { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public virtual ICollection<Like> LikersUsers { get; set; }
    }
}