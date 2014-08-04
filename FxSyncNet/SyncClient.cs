using FxSyncNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.Security;

namespace FxSyncNet
{
    public class SyncClient
    {
        private bool isSignedIn;

        private SyncKeys collectionKeys;

        private StorageClient storageClient;

        public SyncClient()
        {
        }

        public bool IsSignedIn { get { return isSignedIn; } }

        public async Task SignIn(string email, string password)
        {
            Credentials credentials = new Credentials(email, password);

            AccountClient account = new AccountClient();
            LoginResponse response = await account.Login(credentials, true);

            KeysResponse keysResponse = await account.Keys(response.KeyFetchToken);

            string key = Util.ToHexString(Credentials.DeriveHawkCredentials(response.KeyFetchToken, "keyFetchToken"));

            byte[] wrapKB = Credentials.UnbundleKeyFetchResponse(key, keysResponse.Bundle);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            TimeSpan duration = new TimeSpan(0, 1, 0, 0);

            CertificateSignResponse certificate = await account.CertificateSign(response.SessionToken, rsa, duration);

            string jwtToken = JwtCrypto.GetJwtToken(rsa);
            string assertion = JwtCrypto.Bundle(jwtToken, certificate.Certificate);

            byte[] kB = Util.Xor(wrapKB, credentials.UnwrapBKey);

            string syncClientState;
            using (SHA256 sha256 = new SHA256())
            {
                byte[] hash = sha256.ComputeHash(kB);
                syncClientState = Util.ToHexString(hash.Take(16).ToArray());
            }

            TokenClient tokenClient = new TokenClient();
            TokenResponse tokenResponse = await tokenClient.GetSyncToken(assertion, syncClientState);

            storageClient = new StorageClient(tokenResponse.ApiEndpoint, tokenResponse.Key, tokenResponse.Id);

            BasicStorageObject cryptoKeys = await storageClient.GetStorageObject("crypto/keys");

            SyncKeys syncKeys = Crypto.DeriveKeys(kB);
            collectionKeys = Crypto.DecryptCollectionKeys(syncKeys, cryptoKeys);

            isSignedIn = true;
        }

        public async Task<IEnumerable<Bookmark>> GetBookmarks()
        {
            if (!isSignedIn)
                throw new InvalidOperationException("Please sign in first.");

            if (storageClient == null || collectionKeys == null)
                throw new InvalidOperationException("Please make sure you are correctly logged in to the sync service.");

            IEnumerable<BasicStorageObject> collection = await storageClient.GetCollection("bookmarks", true);
            return Crypto.DecryptWbos<Bookmark>(collectionKeys, collection);
        }

        public async Task<IEnumerable<Tab>> GetTabs()
        {
            if (!isSignedIn)
                throw new InvalidOperationException("Please sign in first.");

            if (storageClient == null || collectionKeys == null)
                throw new InvalidOperationException("Please make sure you are correctly logged in to the sync service.");

            IEnumerable<BasicStorageObject> collection = await storageClient.GetCollection("tabs", true);
            return Crypto.DecryptWbos<Tab>(collectionKeys, collection);
        }
    }
}
