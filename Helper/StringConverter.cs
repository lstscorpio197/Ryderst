using System;
using System.Text;
using System.Text.RegularExpressions;

public class StringConverter
{
    public static string ConvertToSlug(string input)
    {
        // Bước 1: Chuyển tất cả ký tự tiếng Việt có dấu thành không dấu
        string result = RemoveVietnameseDiacritics(input);

        // Bước 2: Chuyển tất cả ký tự thành chữ thường
        result = result.ToLower();

        // Bước 3: Thay thế khoảng trắng thành dấu gạch nối
        result = result.Replace(" ", "-");

        // Bước 4: Loại bỏ các ký tự không hợp lệ
        result = Regex.Replace(result, @"[^a-z0-9\-]", "");

        // Bước 5: Đảm bảo không có nhiều dấu gạch nối liên tiếp
        result = Regex.Replace(result, @"\-+", "-");

        // Bước 6: Đảm bảo không bắt đầu hoặc kết thúc bằng dấu gạch nối
        result = result.Trim('-');

        return result;
    }

    private static string RemoveVietnameseDiacritics(string input)
    {
        string[] vietnameseSigns = { "á", "à", "ả", "ã", "ạ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ",
                                    "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
                                    "í", "ì", "ỉ", "ĩ", "ị",
                                    "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
                                    "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
                                    "í", "ì", "ỉ", "ĩ", "ị",
                                    "ý", "ỳ", "ỷ", "ỹ", "ỵ",
                                    "đ", "Đ" };

        string[] noSigns = { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                             "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
                             "i", "i", "i", "i", "i",
                             "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
                             "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
                             "i", "i", "i", "i", "i",
                             "y", "y", "y", "y", "y", 
                             "d", "d"};

        for (int i = 0; i < vietnameseSigns.Length; i++)
        {
            input = input.Replace(vietnameseSigns[i], noSigns[i]);
        }

        return input;
    }
}