using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    [DataContract]
    public class HistoryRecord
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember(Name = "histUri")]
        public Uri Uri { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public IEnumerable<Visit> Visits { get; set; }
    }
}
