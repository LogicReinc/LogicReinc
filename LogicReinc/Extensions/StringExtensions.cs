using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Extensions
{
    public static class StringExtensions
    {
        public static int ToInt32(this string str)
        {
            return Convert.ToInt32(str);
        }

        public static string UrlDecode(this string str)
        {
            return Uri.UnescapeDataString(str);
        }

        public static string UrlEncode(this string str)
        {
            return Uri.EscapeDataString(str);
        }

        public static string Capitalise(this string s)
        {
            if (s.Length > 0)
            {
                char[] chars = s.ToCharArray();

                chars[0] = char.ToUpper(s[0]);

                return new string(chars);
            }
            return s;
        }

        public static int Difference(this string s1, string s2)
        {
            return s1.LevenshteinDistance(s2);
        }

        public static int LevenshteinDistance(this String s1, String s2)
        {
            if (s1 == s2)
                return 0;
            if (s1.Length == 0)
                return s2.Length;
            if (s2.Length == 0)
                return s1.Length;

            int[] distances1 = new int[s2.Length + 1];
            int[] distances2 = new int[s2.Length + 1];

            for (int i = 0; i < distances1.Length; i++)
                distances1[i] = i;
            for (int i = 0; i < s1.Length; i++)
            {
                distances2[0] = i + 1;

                for (int i2 = 0; i2 < s2.Length; i2++)
                {
                    int cost = 0;
                    if (s1[i] != s2[i2])
                        cost = 1;


                    int lowest = distances2[i2] + 1;
                    int dist1 = distances1[i2 + 1] + 1;
                    int dist2 = distances1[i2] + cost;

                    if (lowest > dist1)
                        lowest = dist1;
                    if (lowest > dist2)
                        lowest = dist2;


                    distances2[i2 + 1] = lowest;
                }

                for (int i2 = 0; i2 < distances1.Length; i2++)
                    distances1[i2] = distances2[i2];
            }
            return distances2[s2.Length];
        }

        public static byte[] GetBytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}
