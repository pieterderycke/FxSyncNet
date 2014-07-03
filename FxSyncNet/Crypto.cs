using FxSyncNet.ApiModels;
using Newtonsoft.Json;
using RFC5869;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet
{
    public static class Crypto
    {
        public static SyncKeys DeriveKeys(byte[] kB)
        {
            byte[] info = Util.Kw("oldsync");

            using (var hmac = new HMACSHA256())
            {
                HKDF hkdf = new HKDF(hmac, kB);
                byte[] result = hkdf.Expand(info, 2 * 32);

                return new SyncKeys() { EncKey = result.Take(32).ToArray(), HmacKey = result.Skip(32).ToArray() };
            }
        }

        public static SyncKeys DecryptCollectionKeys(SyncKeys syncKeys, BasicStorageObject wbo) 
        {
            CryptoKeys decrypted = DecryptWbo<CryptoKeys>(syncKeys, wbo);

            byte[] encKey = Convert.FromBase64String(decrypted.Default[0]);
            byte[] hmacKey = Convert.FromBase64String(decrypted.Default[1]);

            return new SyncKeys() { EncKey = encKey, HmacKey = hmacKey };
        }

        public static IEnumerable<T> DecryptWbos<T>(SyncKeys syncKeys, IEnumerable<BasicStorageObject> wbos)
        {
            return (from wbo in wbos
                    select DecryptWbo<T>(syncKeys, wbo)).ToList();
        }

        public static T DecryptWbo<T>(SyncKeys syncKeys, BasicStorageObject wbo)
        {
            EncryptedPayload payload = JsonConvert.DeserializeObject<EncryptedPayload>(wbo.Payload);

            string computedHmac;
            using (var hmac = new HMACSHA256(syncKeys.HmacKey))
            {
                byte[] ciphertext = Encoding.UTF8.GetBytes(payload.CipherText);
                computedHmac = Util.ToHexString(hmac.ComputeHash(ciphertext));
            }

            if (computedHmac != payload.Hmac)
                throw new Exception(string.Format("The calculated HMAC is \"{0}\" does not match with the epected one \"{1}\".", computedHmac, payload.Hmac));

            byte[] iv = Convert.FromBase64String(payload.Iv).Take(16).ToArray();

            using (Aes aes = AesManaged.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;

                aes.IV = iv;
                aes.Key = syncKeys.EncKey;
                using(ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] ciphertext = Convert.FromBase64String(payload.CipherText);

                    byte[] result = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
                    string plaintext = Encoding.UTF8.GetString(result);
                    
                    return JsonConvert.DeserializeObject<T>(plaintext);
                }
            }
        }
    }
}
