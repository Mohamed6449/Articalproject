using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity.Roles
{
	public class UpdateRoleViewModel
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Name { get; set; }

	}
}
