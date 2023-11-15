using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ADProvider
{
    internal class Helpers
    {
        private static Random random = new Random();
        internal static string NameFix(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        internal static string Crc8(string data)
        {
            byte checksum = 0;
            for (int i = 0; i < data.Length; i++)
                checksum += (byte)data[i];

            return string.Format("{0:x2}", (byte)(-checksum));
        }

        internal static string GetPassword(bool simple = true)
        {
            return simple ? GetSimplePassword() : GetStrongPassword();
        }

        private static string GetSimplePassword()
        {
            // a,b,c,d,e,f,g,h
            char l1 = (char)random.Next('a', 'h' + 1);
            char l2 = (char)random.Next('A', 'H' + 1);
            char d = random.Next(2) == 0 ? '#' : '@';
            int code = random.Next(10000, 99999);
            return $"{l1}{l2}{d}{code}";
        }

        private static string GetStrongPassword()
        {
            byte[] rgb = new byte[10];
            RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
            rngCrypt.GetBytes(rgb);
            return Convert.ToBase64String(rgb);
        }
    }
}
