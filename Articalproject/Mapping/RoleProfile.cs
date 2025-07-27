using AutoMapper;
using Articalproject.Models.Identity;
using Articalproject.ViewModels.Identity.Roles;
using Microsoft.AspNetCore.Identity;

namespace Articalproject.Mapping
{
    public class RoleProfile:Profile
    {
       public RoleProfile()
        {
            CreateMap<IdentityRole, GetRolesViewModel>();
            CreateMap<IdentityRole, UpdateRoleViewModel>();
            CreateMap<IdentityRole, GetRoleByIdViewModel>();
            CreateMap< UpdateRoleViewModel, IdentityRole>();
            CreateMap<User, ManageUsersInRoleViewModel>().
                ForMember(des => des.UserId, opt => opt.MapFrom(src => src.Id)).
                ForMember(des => des.UserName, opt => opt.MapFrom(src => src.UserName));
                
            ;
        }
    }
}
