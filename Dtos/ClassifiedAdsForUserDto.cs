using System;

namespace FreeAds.API.Dtos
{
    public class ClassifiedAdsForUserDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
    }
}