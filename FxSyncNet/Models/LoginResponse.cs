using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    public class LoginResponse
    {
        public string Uid { get; set; }
        public string SessionToken { get; set; }
        public string KeyFetchToken { get; set; }
        public bool Verified { get; set; }
        public long AuthAt { get; set; }
    }
}
