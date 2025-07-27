using System.Globalization;

namespace Articalproject.Helper
{
    public class LocalizableEntity
    {
        public string Localize(string nameAr ,string nameEn)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.ToLower() == "ar")
                return nameAr;
            
        
                return nameEn;
            
        }
    }
}
