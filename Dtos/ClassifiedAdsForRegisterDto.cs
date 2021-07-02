using System;
using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Dtos
{
    public class ClassifiedAdsForRegisterDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Category { get; set; }
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        public string AppUserId { get; set; }
        //public DateTime ValidTo { get; set; }
        //public ICollection<Photo> Photos { get; set; }
    }
}