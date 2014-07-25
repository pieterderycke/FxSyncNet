using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public static class BinaryReaderExtensions
    {
        public static ulong ReadBigEndianUInt64(this BinaryReader reader)
        {
            byte[] buffer = reader.ReadBytes(8);

            Array.Reverse(buffer);

            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
