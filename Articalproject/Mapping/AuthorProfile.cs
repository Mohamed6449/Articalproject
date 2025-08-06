using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.ViewModels.Author;
using System.Text.RegularExpressions;

namespace Articalproject.Mapping
{
    public class AuthorProfile: AutoMapper.Profile
    {
        public AuthorProfile() {

            CreateMap<GetAuthorsViewModel, UpdateAuthorViewModel>();
            CreateMap<UpdateAuthorViewModel, User>();
            CreateMap<UpdateAuthorViewModel, Author>();



        }
    }
}
