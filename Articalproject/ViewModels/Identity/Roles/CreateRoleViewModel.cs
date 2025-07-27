using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity.Roles
{
	public class CreateRoleViewModel
	{
		[Required]
		public string Name { get; set; }
	}
}
