using System.Text.RegularExpressions;

namespace TaskManagement.Server.Common.Extensions
{
    public static class StringExtensions
    {
        public static string SanitizeFileName(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var fileName = Path.GetFileName(input);
            fileName = Regex.Replace(fileName, @"[^\w\-. ]+", "_");
            
            if (fileName.Length > 120)
            {
                var ext = Path.GetExtension(fileName);
                var name = Path.GetFileNameWithoutExtension(fileName);
                fileName = name[..Math.Min(name.Length, 120 - ext.Length)] + ext;
            }
            
            return fileName;
        }

        public static bool IsValidDate(this string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return false;

            return DateOnly.TryParse(dateString, out _);
        }

        public static string TrimOrDefault(this string? input, string defaultValue = "")
        {
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
        }
    }
}
