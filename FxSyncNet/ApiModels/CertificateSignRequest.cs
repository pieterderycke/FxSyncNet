using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.ApiModels
{
    [DataContract]
    public class CertificateSignRequest
    {
        [DataMember(Name = "publicKey")]
        public PublicKey PublicKey { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }
    }
}
