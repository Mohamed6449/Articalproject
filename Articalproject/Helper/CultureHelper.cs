using System.Globalization;
using System.Text.RegularExpressions;

namespace Articalproject.Helper
{
	public static class CultureHelper
	{
		public static bool IsRightToLeft()
		{
			return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
			
		}

		public static bool IsArabic()
		{
			return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.StartsWith("ar");
		}
	}
}
