using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public static class Util
    {
        public static byte[] Kw(string name)
        {
            return Encoding.UTF8.GetBytes(string.Format("identity.mozilla.com/picl/v1/{0}", name));
        }

        public static byte[] Kwe(string name, string email)
        {
            return Encoding.UTF8.GetBytes(string.Format("identity.mozilla.com/picl/v1/{0}:{1}", name, email));
        }

        public static string ToHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLowerInvariant();
        }

        public static byte[] FromHexString(string hexString)
        {
            int NumberChars = hexString.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hexString))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }

            return bytes;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        // caused by Microsoft changing the endianes in RSAParameters :-(
        public static BigInteger BigIntegerFromBigEndian(byte[] data)
        {
            data = data.Reverse().ToArray();
            if (data[data.Length - 1] > 127)
            {
                Array.Resize(ref data, data.Length + 1);
                data[data.Length - 1] = 0;
            }
            return new BigInteger(data);
        }

        public static byte[] Xor(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                throw new ArgumentException("The length of the input buffers does not match.");

            byte[] result = new byte[buffer1.Length];

            for (int i = 0; i < buffer1.Length; i++)
                result[i] = (byte)(buffer1[i] ^ buffer2[i]);
            return result;
        }

        public static bool AreEqual(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                return false;

            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                    return false;
            }

            return true;
        }
    }
}
