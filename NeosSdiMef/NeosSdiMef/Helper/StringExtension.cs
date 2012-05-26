using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeosSdiMef.Extension
{
    public static class StringExtension
    {

        /// <summary>
        /// The first letter of an identifier is uppercase and the first letter of each subsequent concatenated word is capitalized
        /// </summary>
        /// <param name="s">initial word</param>
        /// <returns>word formated in uppercamelcase</returns>
        public static string UpperCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// The first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized
        /// </summary>
        /// <param name="s">initial word</param>
        /// <returns>word formated in lowercamelcase</returns>
        public static string LowerCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

    }
}
