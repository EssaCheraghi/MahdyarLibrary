using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mahdyar_Library
{
   public static class Cls_RegularExperssions
    {

        static bool Bol_invalid_mail;
        /// <summary>
        /// check email format 
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool Ext_IsValidEmail(this string strIn)
        {
            if (strIn.Length == 0) return false;

            Bol_invalid_mail = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (Bol_invalid_mail)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        public static bool Ext_IsStrongPassword(this string s)
        {
            bool isStrong = Regex.IsMatch(s, @"[\d]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[a-z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[A-Z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[\s~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
            if (isStrong) isStrong = s.Length > 7;
            return isStrong;
        }
        public static bool Ext_IsValidIPAddress(this string s)
        {
            return Regex.IsMatch(s,
                    @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

		[Obsolete("Must Change")]
		public static bool Ext_IsValidWebsiteAddress(this string s)
		{
			if ((s.StartsWith("http://") || s.StartsWith("https://") || s.StartsWith("www."))) return true;
			return false;
		}

		public static bool Ext_IsValidDate(this string s)
        {
            return Regex.IsMatch(s,
                    @"^(([1][3-9][0-9][0-9]|[0-9][0-9])[\/](([1][1-2])|[1-9])[\/](([0-3][0-9])|[1-9]))$");
        }
        public static bool Ext_IsValidTime(this string s)
        {
            return Regex.IsMatch(s,
                        @"^(([0-1][0-9]|[0-9])|[2][0-3]):(([0-5][0-9]|[1-9]))$");
        }
        public static bool Ext_IsValidDateTime(this string s)
        {
            return Regex.IsMatch(s,
                         @"^(([1][3-9][0-9][0-9]|[0-9][0-9])[\/](([1][1-2])|[1-9])[\/](([0-3][0-9])|[1-9])[ ](([0-1][0-9]|[0-9])|[2][0-3]):(([0-5][0-9]|[1-9])))$");
        }
        public static bool Ext_IsValidMobile(this string strIn)
        {
            if (strIn.Length == 0) return false;

            Bol_invalid_mail = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (Bol_invalid_mail)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"(^(00989|\+989|989|09|9)([0|3][0-9])[0-9]{3}[0-9]{4}$)|(^(00989|\+989|989|09|9)(1[0-9])[0-9]{3}[0-9]{4}$)|^(00989|\+989|989|09|9)(2[0-2])[0-9]{3}[0-9]{4}$",
                   RegexOptions.IgnoreCase);
        }
        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                Bol_invalid_mail = true;
            }
            return match.Groups[1].Value + domainName;
        }

    }
}
