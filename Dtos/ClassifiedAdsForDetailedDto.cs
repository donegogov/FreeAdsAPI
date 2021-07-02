using System;
using System.Collections.Generic;
using FreeAds.API.Models;

namespace FreeAds.API.Dtos
{
    public class ClassifiedAdsForDetailedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime ValidTo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ICollection<PhotoForDetailedDto> Photos { get; set; }
        public string AppUserId { get; set; }
    }
}