using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        //[StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]
        public string Password { get; set; }
    }
}