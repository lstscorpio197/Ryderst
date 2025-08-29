namespace ShopAdmin.Common
{
    public static class StringUltilities
    {
        public static string ToStringNumber(this decimal? value)
        {
            if (value == null)
                return string.Empty;
            return $"{value:N0}";
        }
        public static string ToStringNumber(this decimal value)
        {
            return $"{value:N0}";
        }
    }
}
