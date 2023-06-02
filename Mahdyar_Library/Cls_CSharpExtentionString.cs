using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using System.Threading.Tasks;

namespace Mahdyar_Library
{
    public static partial class Cls_CSharpExtention
    {
       public static string PasswordHash = "P@@Sw0rd";
       public static string SaltKey = "S@LT&KEY";
       public static string VIKey = "@1B2c3D4e5F6g7H8";
      

        public static string Ext_Reverse(this string Str_Text)
        {
            char[] charArray = Str_Text.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        /// <summary>
        /// example 
        /// input   {ali   ,  reza  ,  hasan} , '-'
        /// output ali-reza-hasan
        /// </summary>
        /// <param name="Str">string array to concat</param>
        /// <param name="c">char to put between</param>
        /// <returns></returns>
        public static string Ext_CharBetween(this string[] Str, char c)
        {
            string ret = "";
            foreach (string s in Str)
                ret += s + c.ToString();
            if (ret.Length > 0) ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }
        public static string Ext_Encrypt(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Ext_Decrypt(this string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());

        }
        public static string Ext_GetString(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return "";

            // Ansi as default
            Encoding encoding = Encoding.Default;

            /*
                EF BB BF	UTF-8 
                FF FE UTF-16	little endian 
                FE FF UTF-16	big endian 
                FF FE 00 00	UTF-32, little endian 
                00 00 FE FF	UTF-32, big-endian 
             */

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                encoding = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                encoding = Encoding.Unicode;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                encoding = Encoding.BigEndianUnicode; // utf-16be
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                encoding = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                encoding = Encoding.UTF7;

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static bool Ext_IsUnicode(this string value)
        {
            int asciiBytesCount = System.Text.Encoding.ASCII.GetByteCount(value);
            int unicodBytesCount = System.Text.Encoding.UTF8.GetByteCount(value);

            if (asciiBytesCount != unicodBytesCount)
            {
                return true;
            }
            return false;
        }
        public static string Ext_FixPersianChars(this string Value)
        {
            return Value.Replace('ي', 'ی').Replace("ك", "ک");
        }
      
        /// <summary>
        /// متدی برای تبدیل اعداد انگلیسی به فارسی
        /// </summary>
        public static string Ext_ToPersianNumber(this string input)
        {
            if (input.Trim() == "") return "";

            //۰۱۲۳۴۵۶۷۸۹
            input = input.Replace("0", "۰");
            input = input.Replace("1", "۱");
            input = input.Replace("2", "۲");
            input = input.Replace("3", "۳");
            input = input.Replace("4", "۴");
            input = input.Replace("5", "۵");
            input = input.Replace("6", "۶");
            input = input.Replace("7", "۷");
            input = input.Replace("8", "۸");
            input = input.Replace("9", "۹");

            return input;
        }
        public static string Ext_ToEnglishNumber(this string input)
        {
            if (input.Trim() == "") return "";

            //۰۱۲۳۴۵۶۷۸۹
            input = input.Replace("۰","0");
            input = input.Replace("۱","1");
            input = input.Replace("۲","2");
            input = input.Replace("۳","3");
            input = input.Replace("۴","4");
            input = input.Replace("۵","5");
            input = input.Replace("۶","6");
            input = input.Replace("۷","7");
            input = input.Replace("۸","8");
            input = input.Replace("۹", "9");

            return input;
        }
        public static string Ext_RemoveTags(this string input)
        {
            return input.Replace(@"<[^>]+>| ", "").Trim();
        }
        public static bool Ext_IsVariableName(this string input,bool AllowPersianChars=false)
        {
            if (string.IsNullOrEmpty(input?.Trim())) return false;input = input.Trim();
            if (AllowPersianChars) input = input.Ext_ToEnglishNumber();

            if (char.IsDigit(input[0]) || input.Ext_IsDigit(false)) return false;
            if (!AllowPersianChars && input.Ext_ContainsPersianLetter()) return false;
            if (input.Ext_IsLetterOrDigit(AllowPersianChars,"_ .")) return true;

            return false;
        }
        public static bool Ext_IsDigit(this string Str,bool CheckPersianDigits=true,bool EnableFloatingNumbers = false,bool ContainsNegativeNumbers = true)
        {
            if (Str.Trim().Length == 0) return false;
            if (CheckPersianDigits) Str = Str.Ext_ToEnglishNumber();
			if (ContainsNegativeNumbers && Str[0] == '-') Str = Str.Substring(1, Str.Length - 1);

			int counter = 0;
            foreach (char ch in Str)
            {
               if(char.IsDigit(ch)) { counter++; continue;}
               if (ch != '.') return false;
               if (EnableFloatingNumbers && (counter == 0 || counter == Str.Length - 1) && ch == '.') return false;
                counter++;
            }
            return true;
        }
        public static bool Ext_IsLetter(this string Str, bool CheckPersianLetters = true)
        {
            if (Str.Trim().Length == 0) return false;

            foreach (char ch in Str)
            {
                if (!char.IsLetter(ch) && !(CheckPersianLetters && ch >= 'ا' && ch <='ی')) return false;
            }
            return true;
        }
        public static bool Ext_IsLetterOrDigit(this string Str, bool CheckPersianLetters = true,string AllowedChars="")
        {
            if (Str.Trim().Length == 0) return false;
            if (CheckPersianLetters) Str = Str.Ext_ToEnglishNumber();

            foreach (char ch in Str)
            {
                if (!char.IsLetterOrDigit(ch) && !(CheckPersianLetters && ch >= 'ا' && ch <= 'ی') && !AllowedChars.Contains(ch)) return false;
               
            }
            return true;
        }
        public static bool Ext_ContainsPersianLetter(this string Str)
        {
            if (Str.Trim().Length == 0) return false;

            foreach (char ch in Str)
            {
                if (ch >= 'ا' && ch <= 'ی') return true;
            }
            return false;
        }
        public static string Ext_ConvertToAscii(this string input)
        {
            //اعداد یونیکد فارسی            ۰۱۲۳۴۵۶۷۸۹         
            //اعداد یونیکد عربی            ٠١٢٣٤٥٦٧٨٩         
            //return input.Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('۳', '3').Replace('٤', '4')
            //            .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
            //            .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('٣', '3').Replace('۴', '4')
            //            .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9')
            //            .Replace(((char)(8206)).ToString(CultureInfo.InvariantCulture), "")
            //            .Replace(((char)(8207)).ToString(CultureInfo.InvariantCulture), "");

            return input
                .Replace((char)0x0660, '0') //'٠'
                .Replace((char)0x0661, '1') //'١'
                .Replace((char)0x0662, '2') //'٢'
                .Replace((char)0x0663, '3') //'٣'
                .Replace((char)0x0664, '4') //'٤'
                .Replace((char)0x0665, '5') //'٥'
                .Replace((char)0x0666, '6') //'٦'
                .Replace((char)0x0667, '7') //'٧'
                .Replace((char)0x0668, '8') //'٨'
                .Replace((char)0x0669, '9') //'٩'

                .Replace((char)0x06F0, '0') //'٠'
                .Replace((char)0x06F1, '1') //'١'
                .Replace((char)0x06F2, '2') //'٢'
                .Replace((char)0x06F3, '3') //'٣'
                .Replace((char)0x06F4, '4') //'۴'
                .Replace((char)0x06F5, '5') //'۵'
                .Replace((char)0x06F6, '6') //'۶'
                .Replace((char)0x06F7, '7') //'٧'
                .Replace((char)0x06F8, '8') //'٨'
                .Replace((char)0x06F9, '9') //'۹'
                .Replace(((char)(8206)).ToString(CultureInfo.InvariantCulture), "")
                .Replace(((char)(8207)).ToString(CultureInfo.InvariantCulture), "");
        }
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string Ext_Compress(this string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                var compressedStream = new MemoryStream();

                // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, this approach avoids relying on that very odd behavior should it ever change
                using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }

                // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                compressedBytes = compressedStream.ToArray();
            }

            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        public static string Ext_Decompress(this string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }








        /// <summary>
        /// /????????????????????????????????????????
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="nop"></param>
        public static void Normalize2<T>(this T model, INormalizeOption nop)
        {
            var g = nop.NormOptions.HasFlag(NormalizeOption.TrimBeginAndEnd);

            if (model == null) throw new Exception("Model Is null");
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    string val = (string)prop.GetValue(null);
                    if (val == "") prop.SetValue(model, null);
                }
            }
        }
        public static void Normalize2<T>(this T model, NormalizeOption nop)
        {
            var g = nop.HasFlag(NormalizeOption.TrimBeginAndEnd);

            if (model == null) throw new Exception("Model Is null");
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    string val = (string)prop.GetValue(null);
                    if (val == "") prop.SetValue(model, null);
                }
            }
        }
        [Flags]
        public enum NormalizeOption
        {
            /// <summary>
            /// ابتدا و انتهای پروپرتی های رشته ای در صورتی که خالی باشد حذف می شود
            /// </summary>
            TrimBeginAndEnd = 0,
            /// <summary>
            /// اعداد فارسی بتدیل به انگلیسی می شوند
            /// </summary>
            PersianDiditsToEnglish = 1,
            /// <summary>
            /// اعداد انگلیسی تبدیل به اعداد فارسی می شوند
            /// </summary>
            EnglishDiditsToPersian = 2,
            /// <summary>
            /// حروف عربی  مثل ی و و تبدیل به معادل آنها در فارسی می شوند
            /// </summary>
            ArabicLettersToPersian = 4,
            /// <summary>
            /// همه خصوصیاتی که بصورت تاریخ میلادی ثبت شده باشند تبدیل به تاریخ شمسی می شوند
            /// </summary>
            ToPersianDateTime = 8,
            /// <summary>
            /// همه تاریخ هایی که بصورت شمسی ثبت شده اند تبدیل به تاریخ میلادی می شوند
            /// </summary>
            ToMiladiDate = 16,
            /// <summary>
            /// بجای اعداد صفر مقدار نال قرار می گیرد
            /// </summary>
            UseNullInsteadOfZero = 32,
            /// <summary>
            /// بجای رشته های خالی مقدار نال قرار می گیرد
            /// </summary>
            UseNullInsteadOfEmptyStrings = 64,

        }
        public interface INormalizeOption
        {
            NormalizeOption NormOptions { get; set; }
        }
        public class NormalizeClassForViews : INormalizeOption
        {
            public NormalizeOption NormOptions { get; set; } = NormalizeOption.EnglishDiditsToPersian | NormalizeOption.TrimBeginAndEnd;
        }
        public class NormalizeClassForProcess : INormalizeOption
        {
            public NormalizeOption NormOptions { get; set; } = NormalizeOption.EnglishDiditsToPersian;
        }
    }
}
