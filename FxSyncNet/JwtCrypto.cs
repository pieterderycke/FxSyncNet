using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public static class JwtCrypto
    {
        public static string GetJwtToken(RSACryptoServiceProvider rsa)
        {
            JwtHeader header = new JwtHeader();
            JwtPayload payload = new JwtPayload();
            JwtSecurityToken token = new JwtSecurityToken(
                //issuer: "https://api.accounts.firefox.com",
                audience: "https://token.services.mozilla.com",
                //lifetime: new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddHours(1)),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(rsa),
                    SecurityAlgorithms.RsaSha256Signature, 
                    SecurityAlgorithms.Sha256Digest)
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public static string Bundle(string jwtToken, string jsonWebCertificate)
        {
            return string.Format("{0}~{1}", jsonWebCertificate, jwtToken);
        }
    }
}
