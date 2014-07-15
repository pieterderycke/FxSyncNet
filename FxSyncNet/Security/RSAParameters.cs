using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Security
{
    public class RSAParameters
    {
        public RSAParameters(byte[] exponent, byte[] modulus)
        {
            this.Exponent = exponent;
            this.Modulus = modulus;
        }

        public byte[] Exponent { get; private set; }
        public byte[] Modulus { get; private set; }
    }
}
