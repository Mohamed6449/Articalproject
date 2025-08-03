using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Claims
{
	public class GetClaimsViewModel
	{
		public int Id { get; set; }
        public string Name { get; set; }
		public bool IsUserClaim { get; set; } = true;
		public bool IsRoleClaim { get; set; } = true;


	}
}
