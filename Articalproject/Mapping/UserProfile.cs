using AutoMapper;
using Articalproject.Helper;
using Articalproject.Models.Identity;
using Articalproject.ViewModels.Identity;
using Articalproject.ViewModels.Identity.Users;
using Microsoft.AspNetCore.Identity;

namespace Articalproject.Mapping
{
    public class UserProfile:Profile
    {
        public UserProfile() {

            CreateMap<RegisterViewModel, User>()
                .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(des => des.UserName, opt => opt.MapFrom(src => src.UserName));
            CreateMap<User, GetUsersListViewModel>().ForMember(des => des.Name,
                opt => opt.MapFrom(src => CultureHelper.IsArabic() ? src.NameAr : src.NameEn));

            CreateMap<User, UpdateUserViewModel>().ReverseMap();
            CreateMap<User, GetUserByIdViewModel>().ReverseMap();
            
            CreateMap<IdentityRole, ManageRolesViewModel>().
                ForMember(des => des.RoleId, opt => opt.MapFrom(src => src.Id)).
                ForMember(des => des.RoleName, opt => opt.MapFrom(src => src.Name)); 


        }

    }
}
