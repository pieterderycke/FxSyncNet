using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Security
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BCryptRsaKeyBlob
    {
        public ulong Magic;
        public ulong BitLength;
        public ulong cbPublicExp;
        public ulong cbModulus;
        public ulong cbPrime1;
        public ulong cbPrime2;

        public static BCryptRsaKeyBlob Load(byte[] buffer)
        {
            BCryptRsaKeyBlob result = new BCryptRsaKeyBlob();

            using(BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
            {
                result.Magic = reader.ReadUInt64();
                result.BitLength = reader.ReadUInt64();
                result.cbPublicExp = reader.ReadUInt64();
                result.cbModulus = reader.ReadUInt64();
                result.cbPrime1 = reader.ReadUInt64();
                result.cbPrime2 = reader.ReadUInt64();
            }

            return result;
        }
    }
}
