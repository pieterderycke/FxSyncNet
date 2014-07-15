using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Security
{
    public abstract class HashAlgorithm
    {
        public static HashAlgorithm Create(string algorithmName)
        {
            return new SHA256();
        }

        public abstract byte[] ComputeHash(byte[] buffer);
    }
}
