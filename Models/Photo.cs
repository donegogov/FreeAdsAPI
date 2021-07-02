using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeAds.API.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public string Status { get; set; }
        public virtual ClassifiedAds ClassifiedAds { get; set; }
        public Guid ClassifiedAdsId { get; set; }
    }
}