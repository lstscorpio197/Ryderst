using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ShopAdmin.Helper
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            // Bỏ dấu tiếng Việt
            string normalized = phrase.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            string str = sb.ToString().Normalize(NormalizationForm.FormC);

            // Chuyển thành lowercase
            str = str.ToLowerInvariant();

            // Thay khoảng trắng và dấu + thành -
            str = Regex.Replace(str, @"\s+", "-");
            str = str.Replace("+", "-");

            // Loại bỏ ký tự đặc biệt
            str = Regex.Replace(str, @"[^a-z0-9-]", "");

            // Xóa bớt dấu - thừa
            str = Regex.Replace(str, @"-+", "-").Trim('-');

            return str;
        }
    }

}
