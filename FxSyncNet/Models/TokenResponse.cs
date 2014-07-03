using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "uid")]
        public string Uid { get; set; }

        [DataMember(Name = "api_endpoint")]
        public string ApiEndpoint { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }
    }
}
