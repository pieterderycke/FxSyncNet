using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    [DataContract]
    public class PublicKey
    {
        [DataMember(Name = "algorithm")]
        public string Algorithm { get; set; }

        [DataMember(Name = "n", EmitDefaultValue = false)]
        public string N { get; set; }

        [DataMember(Name = "e", EmitDefaultValue = false)]
        public string E { get; set; }

        [DataMember(Name = "y", EmitDefaultValue = false)]
        public string Y { get; set; }

        [DataMember(Name = "p", EmitDefaultValue = false)]
        public string P { get; set; }

        [DataMember(Name = "q", EmitDefaultValue = false)]
        public string Q { get; set; }

        [DataMember(Name = "g", EmitDefaultValue = false)]
        public string G { get; set; }
    }
}
