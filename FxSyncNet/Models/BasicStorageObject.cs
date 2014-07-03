using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    public class BasicStorageObject
    {
        public string Id { get; set; }
        public double Modified { get; set; }
        public int SortIndex { get; set; }
        public string Payload { get; set; }
        public int Ttl { get; set; }
    }
}
