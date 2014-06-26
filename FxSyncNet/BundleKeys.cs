using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class BundleKeys
    {
        public BundleKeys(byte[] hmacKey, byte[] xorKey)
        {
            this.HmacKey = hmacKey;
            this.XorKey = xorKey;
        }

        public byte[] HmacKey { get; private set; }
        public byte[] XorKey { get; private set; }
    }
}
