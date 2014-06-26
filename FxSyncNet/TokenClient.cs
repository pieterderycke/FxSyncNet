using FxSyncNet.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public class TokenClient : ProxyBase
    {
        public TokenClient() : base("https://token.services.mozilla.com/1.0/")
        {
        }

        public async Task<TokenResponse> GetSyncToken(string browerIdAssertion, string password)
        {
            // hex(first16Bytes(sha256(kBbytes)))
            byte[] buffer = Encoding.UTF8.GetBytes(password);
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(buffer, 0, buffer.Length);

            string clientState = Util.ToHexString(GetFirst16Bytes(hash));

            return await Get<TokenResponse>("sync/1.5", browerIdAssertion, clientState);
        }

        private byte[] GetFirst16Bytes(byte[] hash)
        {
            byte[] buffer = new byte[16];
            Array.Copy(hash, buffer, 16);
            return buffer;
        }
    }
}
