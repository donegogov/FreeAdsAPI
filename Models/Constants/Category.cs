using System.ComponentModel.DataAnnotations;

namespace FreeAds.API.Models.Constants
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string CategoryValue { get; set; }

        public Category() {}
        public Category(string CategoryName, string CategoryValue)
        {
            this.CategoryName = CategoryName;
            this.CategoryValue = CategoryValue;
        }
    }
}