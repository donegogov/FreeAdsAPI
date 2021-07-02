using System;

namespace FreeAds.API.Dtos
{
    public class PhotoForDetailedDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}