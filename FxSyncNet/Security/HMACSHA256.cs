using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Security
{
    public class HMACSHA256 : HMAC
    {
        public HMACSHA256()
            : base("HMACSHA256")
        {
        }

        public HMACSHA256(byte[] key)
            : base("HMACSHA256", key)
        {
        }
    }
}
