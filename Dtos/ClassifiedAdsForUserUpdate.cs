using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Dtos
{
    public class ClassifiedAdsForUserUpdate
    {
        [Required]
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
        //public ICollection<PhotoForDetailedDto> Photos { get; set; }
    }
}