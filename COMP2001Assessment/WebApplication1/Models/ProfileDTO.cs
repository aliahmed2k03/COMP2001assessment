using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ProfileDTO
    {
        [Required]
        public string ProfileName { get; set; } ="";
        
        [Required]
        public string Bio {get; set; } = "";
        
        [Required]
        public string Location { get; set; } = "";

        [Required]
        public int FavouriteActivity { get; set; }
    }
}
