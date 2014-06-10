using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.ApiModels
{
    [DataContract]
    public class CertificateSignResponse
    {
        [DataMember(Name = "cert")]
        public string Certificate { get; set; }
    }
}
