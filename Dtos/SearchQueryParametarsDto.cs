using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Dtos
{
    public class SearchQueryParametarsDto
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
    }
}