using Articalproject.Models.Identity;
using Articalproject.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Articalproject.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? Instagram { get; set; }

        [Required]
        public string UserId { get; set; }
        public User user { get; set; }
        public ICollection<AuthorPost> authorPosts { get; set; }



    }
}
