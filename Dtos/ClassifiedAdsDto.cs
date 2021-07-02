using System;
using System.Collections.Generic;
using FreeAds.API.Models;

namespace FreeAds.API.Dtos
{
    public class ClassifiedAdsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
        public string AppUserId { get; set; }
        public bool IsLiked { get; set; }
    }
}