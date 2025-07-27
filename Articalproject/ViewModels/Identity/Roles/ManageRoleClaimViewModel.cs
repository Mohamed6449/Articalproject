namespace Articalproject.ViewModels.Identity.Roles
{
	public class ManageRoleClaimViewModel
	{
		public string RoleId { get; set; }

		public List<RoleClaims> Claims { get; set; } = new List<RoleClaims>();
	
	}
}
