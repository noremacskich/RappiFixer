using System;
using System.Collections.Generic;
using System.Text;

namespace RappiFixer.Extensions
{
    public static class StringExtensions
    {
        public static String CenterString(this String s, int size)
        {
            return CenterString(s, size, ' ');
        }

        public static String CenterString(this String s, int size, char pad)
        {
            if (s == null || size <= s.Length)
                return s;

            StringBuilder sb = new StringBuilder(size);
            for (int i = 0; i < (size - s.Length) / 2; i++)
            {
                sb.Append(pad);
            }
            sb.Append(s);
            while (sb.Length < size)
            {
                sb.Append(pad);
            }
            return sb.ToString();
        }

    }
}
