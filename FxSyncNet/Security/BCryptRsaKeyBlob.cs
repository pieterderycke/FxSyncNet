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
        public uint Magic;
        public uint BitLength;
        public uint cbPublicExp;
        public uint cbModulus;
        public uint cbPrime1;
        public uint cbPrime2;

        public static BCryptRsaKeyBlob Load(byte[] buffer)
        {
            BCryptRsaKeyBlob result = new BCryptRsaKeyBlob();

            using(BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
            {
                result.Magic = reader.ReadUInt32();
                result.BitLength = reader.ReadUInt32();
                result.cbPublicExp = reader.ReadUInt32();
                result.cbModulus = reader.ReadUInt32();
                result.cbPrime1 = reader.ReadUInt32();
                result.cbPrime2 = reader.ReadUInt32();
            }

            return result;
        }
    }
}
