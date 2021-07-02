using System;

namespace FreeAds.API.Dtos
{
    public class PhotoForReturnDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
    }
}