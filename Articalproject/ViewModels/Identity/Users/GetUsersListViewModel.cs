using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity.Users
{
	public class GetUsersListViewModel
	{
		[Required]
		public string Id { get; set; }

		public string Name { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }


	}
}
