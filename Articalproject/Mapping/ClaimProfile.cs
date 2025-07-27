using AutoMapper;
using Articalproject.Models.Identity;
using Articalproject.ViewModels.Claims;
using Articalproject.ViewModels.Identity.Users;

namespace Articalproject.Mapping
{
	public class ClaimProfile:Profile
	{
		public ClaimProfile()
		{
			CreateMap<Claim, GetClaimsViewModel>().
				ForMember(des => des.Name, opt => opt.MapFrom(src => src.Localize(src.NameAr, src.NameEn))) ;
			CreateMap<AddClaimViewModel, Claim>();

		}
	}
}
