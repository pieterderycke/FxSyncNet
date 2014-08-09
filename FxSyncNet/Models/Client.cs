using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string ClientName { get; set; }

        public IEnumerable<Tab> Tabs { get; set; }
    }
}
