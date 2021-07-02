using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Dtos
{
    public class UserForUpdateDto
    {
        [Required]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }
}