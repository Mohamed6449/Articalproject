
using Articalproject.Helper;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.Models
{
    public class Category : LocalizableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "NameArIsRequired")]
        [DataType(DataType.Text)] 
        public string NameAr { get; set; }
        [Required(ErrorMessage = "NameEnIsRequired")]
        [DataType(DataType.Text)]
        public string NameEn { get; set; }

        public ICollection<AuthorPost> AuthorPosts { get; set; }


    }
}
