using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class SyncKeys
    {
        public byte[] EncKey { get; set; }
        public byte[] HmacKey { get; set; }
    }
}
