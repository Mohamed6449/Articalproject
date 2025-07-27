using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity.Users
{
	public class UpdateUserViewModel
	{
		[Required]

		public string Id { get; set; }

		[Required]
		public string NameAr { get; set; }
		[Required]
		public string NameEn { get; set; }

		public string? Address { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }


	}
}
