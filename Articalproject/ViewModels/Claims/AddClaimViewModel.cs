using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Claims
{
	public class AddClaimViewModel
	{
		[Required]
        [DataType(DataType.Text)]

        public string NameAr { get; set; }

        [Required]
        [DataType(DataType.Text)]
		public string NameEn { get; set; }

		public bool IsUserClaim { get; set; } = true;
		public bool IsRoleClaim { get; set; } = true;
	}
}
