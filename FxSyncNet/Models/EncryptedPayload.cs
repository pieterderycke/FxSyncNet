using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    public class EncryptedPayload
    {
        public string CipherText { get; set; }
        public string Hmac { get; set; }
        public string Iv { get; set; }
    }
}
