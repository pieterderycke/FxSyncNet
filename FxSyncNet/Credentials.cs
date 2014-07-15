using Medo.Security.Cryptography;
using RFC5869;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.Security;

namespace FxSyncNet
{
    public class Credentials
    {
        public Credentials(string email, string password)
        {
            Email = email;

            using (var hmac = new HMACSHA256())
            {
                Pbkdf2 pbkdf2 = new Pbkdf2(hmac, Encoding.UTF8.GetBytes(password), Util.Kwe("quickStretch", email), 1000);
                QuickStretchedPW = pbkdf2.GetBytes(32);

                HKDF hkdf = new HKDF(hmac, QuickStretchedPW);
                AuthPW = hkdf.Expand(Util.Kw("authPW"), 32);
                UnwrapBKey = hkdf.Expand(Util.Kw("unwrapBkey"), 32);
            }

            using(SHA256 sha256 = new SHA256())
            {
                byte[] hash = sha256.ComputeHash(UnwrapBKey);
                SyncClientState = Util.ToHexString(hash.Take(16).ToArray());
            }
        }

        public string Email { get; private set; }
        public byte[] AuthPW { get; private set; }
        public byte[] QuickStretchedPW { get; private set; }
        public byte[] UnwrapBKey { get; private set; }
        public string SyncClientState { get; private set; }

        public static byte[] UnbundleKeyFetchResponse(string key, string bundleHex)
        {
            byte[] bundle = Util.FromHexString(bundleHex); // bundle
            byte[] bundleKey = Util.FromHexString(key); // key

            BundleKeys keys = DeriveBundleKeys(bundleKey, "account/keys");

            byte[] cipherText = bundle.Take(64).ToArray();
            byte[] expectedHmac = bundle.Skip(bundle.Length - 32).Take(32).ToArray();

            using (var hmac = new HMACSHA256(keys.HmacKey))
            {
                if (!Util.AreEqual(expectedHmac, hmac.ComputeHash(cipherText)))
                    throw new Exception("Bad HMac.");
            }

            byte[] keyAWrapB = Util.Xor(bundle.Take(64).ToArray(), keys.XorKey);
            byte[] kA = keyAWrapB.Take(32).ToArray();
            byte[] wrapKB = keyAWrapB.Skip(32).ToArray();

            return wrapKB;
        }

        //deriveHawkCredentials
        public static byte[] DeriveHawkCredentials(string tokenHex, string context)
        {
            byte[] token = Util.FromHexString(tokenHex);
            byte[] info = Util.Kw(context);

            using (var hmac = new HMACSHA256())
            {
                HKDF hkdf = new HKDF(hmac, token);
                byte[] result = hkdf.Expand(info, 3 * 32);
                return result.Skip(64).ToArray();
            }
        }

        private static BundleKeys DeriveBundleKeys(byte[] key, string keyInfo)
        {
            byte[] info = Util.Kw(keyInfo);
            
            using (var hmac = new HMACSHA256())
            {
                HKDF hkdf = new HKDF(hmac, key);
                byte[] result = hkdf.Expand(info, 3 * 32);
                
                byte[] hmacKey = result.Take(32).ToArray();
                byte[] xorKey = result.Skip(32).ToArray();
                return new BundleKeys(hmacKey, xorKey);
            }
        }
    }
}
