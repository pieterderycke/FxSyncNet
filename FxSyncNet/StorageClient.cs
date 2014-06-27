using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class StorageClient : ProxyBase
    {
        public StorageClient(string apiEndpoint, string key, string id) : base (apiEndpoint)
        {
        }

        //public async Task GetQuotaInfo()
        //{
        //    return await Get<TokenResponse>("sync/1.5", browerIdAssertion, clientState);
        //}
    }
}
