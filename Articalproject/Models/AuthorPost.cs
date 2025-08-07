using Articalproject.Models.Identity;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Articalproject.Models
{
    public class AuthorPost
    {
        public int Id { get; set; }
        public string PostImage { get; set; }

        [Required(ErrorMessage = "PostTitleIsRequired")]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionIsRequired")]
        public string PostDescription { get; set; }

        public DateTime PostDate { get; set; } 
        public int CategoryId { get; set; }
        public string UserId { get; set; }

        public  Category Category { get; set; }
        public User user { get; set; }
    }
}
