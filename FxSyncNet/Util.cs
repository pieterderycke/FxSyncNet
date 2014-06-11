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
            return BitConverter.ToString(data).Replace("-", "");
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

        public static byte[] FromHexString(string hexString, bool isLittleEndian)
        {
            byte[] data = FromHexString(hexString);

            if (isLittleEndian)
            {
                for (int i = 0; i < data.Length; i+=4)
                {
                    Array.Reverse(data, i, 4);
                }
            }
            
            return data;
        }

        public static byte[] ToByteArray(int[] array, bool reverseEndianness)
        {
            if (!reverseEndianness)
            {
                byte[] result = new byte[array.Length * sizeof(int)];
                Buffer.BlockCopy(array, 0, result, 0, result.Length);

                return result;
            }
            else
            {
                byte[] result = new byte[array.Length * 4];

                for (int i = 0; i < array.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(array[i]);
                    Array.Reverse(bytes);

                    Buffer.BlockCopy(bytes, 0, result, i * 4, 4);
                }

                return result;
            }
        }

        public static int[] ToIntArray(byte[] array)
        {
            return ToIntArray(array, false);
        }

        public static int[] ToIntArray(byte[] array, bool isLittleEndian)
        {
            int[] result = new int[array.Length / 4];
            byte[] buffer = new byte[4];            
            
            for (int i = 0; i < result.Length; i++)
            {
                Buffer.BlockCopy(array, i * 4, buffer, 0, 4);

                if (isLittleEndian)
                    Array.Reverse(buffer);

                Buffer.BlockCopy(buffer, 0, result, i * 4, 4);
            }

            return result;
        }

        public static void SwapEndianness(byte[] array)
        {
            for (int i = 0; i + 3 < array.Length; i+=4)
            {
                //if(i + 3 < array.Length)

                Array.Reverse(array, i, 4);
            }
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
    }
}
