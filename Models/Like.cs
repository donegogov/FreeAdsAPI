using System;
using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Models
{
    public class Like
    {
        public string LikerUserId { get; set; }
        public Guid LikedClassifiedAdsId { get; set; }
        public virtual AppUser LikerUser { get; set; }
        public virtual ClassifiedAds LikedClassifiedAds { get; set; }
    }
}