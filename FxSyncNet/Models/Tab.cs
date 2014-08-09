using FxSyncNet.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    public class Tab
    {
        public string Title { get; set; }
        public string Icon { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime LastUsed { get; set; }
    }
}
