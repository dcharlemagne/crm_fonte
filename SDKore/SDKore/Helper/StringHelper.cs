using System.Text.RegularExpressions;

namespace SDKore.Helper
{
    public static class StringHelper
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string GetOnlyNumbers(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = Regex.Replace(value, @"[^0-9]+", "");
            }

            return value;
        }
    }
}