using Medo.Security.Cryptography;
using RFC5869;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.ApiModels
{
    [DataContract]
    public class LoginRequest
    {
        public LoginRequest(string email, string password)
        {
            this.Email = email;
            this.AuthPW = CalculateAuthPW(password);
        }

        [DataMember(Name="email")]
        public string Email { get; private set; }

        [DataMember(Name="authPW")]
        public string AuthPW  { get; private set; }

        private string CalculateAuthPW(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                Pbkdf2 pbkdf2 = new Pbkdf2(hmac, Encoding.UTF8.GetBytes(password), Kwe("quickStretch", Email), 1000);
                byte[] quickStretchedPW = pbkdf2.GetBytes(32);

                HKDF hkdf = new HKDF(hmac, quickStretchedPW);
                byte[] authPW = hkdf.Expand(Kw("authPW"), 32);

                return Hex(authPW);
            }
        }

        private byte[] Kw(string name)
        {
            return Encoding.UTF8.GetBytes(string.Format("identity.mozilla.com/picl/v1/{0}", name));
        }

        private byte[] Kwe(string name, string email)
        {
            return Encoding.UTF8.GetBytes(string.Format("identity.mozilla.com/picl/v1/{0}:{1}", name, email));
        }

        private string Hex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }
    }
}
