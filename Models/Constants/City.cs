using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Models.Constants
{
    public class City
    {
        public int Id { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string CityValue { get; set; }


        public City() {}
        public City(string CityName, string CityValue)
        {
            this.CityName = CityName;
            this.CityValue = CityValue;
        }
    }
}