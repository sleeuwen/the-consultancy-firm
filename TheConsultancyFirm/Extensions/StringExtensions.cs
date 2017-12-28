using System.Text.RegularExpressions;
using Unidecode.NET;

namespace TheConsultancyFirm.Extensions
{
    public static class StringExtensions
    {
        public static string Sluggify(this string text, string separator = "-")
        {
            var str = text.Unidecode().ToLower();
            // Remove any characters not a-z, 0-9, '-' or spaces.
            str = Regex.Replace(str, @"[^a-z0-9\s-]", " ");
            // Convert multiple consecutive spaces into one
            str = Regex.Replace(str, @"\s+", " ");
            // Trim spaces and replace spaces with dashes
            return Regex.Replace(str.Trim(), @"\s", "-");
        }
    }
}
