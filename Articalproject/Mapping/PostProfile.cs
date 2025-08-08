using Articalproject.Models;
using Articalproject.ViewModels.Post;

namespace Articalproject.Mapping
{
    public class PostProfile: AutoMapper.Profile
    {

        public PostProfile() {

            CreateMap<CreatePostViewModel, AuthorPost>();
            CreateMap<AuthorPost,UpdatePostViewModel >();
            CreateMap<UpdatePostViewModel, AuthorPost>();


        }



    }
    
}
