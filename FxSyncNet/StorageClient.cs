using FxSyncNet.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class StorageClient : ProxyBase
    {
        private readonly HawkNet.HawkCredential credential;

        public StorageClient(string apiEndpoint, string key, string id) : base (apiEndpoint + "/")
        {
            credential = new HawkNet.HawkCredential() { Algorithm = "sha256", Id = id, Key = Util.ToHexString(Encoding.UTF8.GetBytes(key)) };
        }

        public async Task GetQuotaInfo()
        {
            await Get("info/quota", credential);
        }

        public async Task GetCollectionInfo()
        {
            await Get("info/collections", credential);
        }

        public async Task GetCollectionUsage()
        {
            await Get("info/collection_usage", credential);
        }

        public async Task GetCollectionCounts()
        {
            await Get("info/collection_counts", credential);
        }

        public Task<IEnumerable<BasicStorageObject>> GetCollection(string collection, bool full)
        {
            return Get<IEnumerable<BasicStorageObject>>("storage/" + collection + ((full) ? "?full" : ""), credential);
        }

        public Task<BasicStorageObject> GetStorageObject(string storageObject)
        {
            return Get<BasicStorageObject>("storage/" + storageObject, credential);
        }
    }
}
