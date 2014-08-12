using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.Security;

namespace FxSyncNet.Util
{
    public static class JwtCryptoHelper
    {
        public static string GetJwtToken(RSACryptoServiceProvider rsa)
        {
            var payload = new Dictionary<string, object>() 
            {
                { "iss", "https://api.accounts.firefox.com" },
                { "aud", "https://token.services.mozilla.com" }
            };

            return Jose.JWT.Encode(payload, rsa.PlatformProvider, Jose.JwsAlgorithm.RS256);
        }

        public static string Bundle(string jwtToken, string jsonWebCertificate)
        {
            return string.Format("{0}~{1}", jsonWebCertificate, jwtToken);
        }
    }
}
