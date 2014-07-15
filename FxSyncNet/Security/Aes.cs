using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_STORE
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
#else
using System.Security.Cryptography;
#endif

namespace FxSyncNet.Security
{
    public class Aes
    {
        private readonly byte[] iv;
        private readonly byte[] key;

        public Aes(byte[] iv, byte[] key)
        {
            this.iv = iv;
            this.key = key;
        }

        public byte[] Decrypt(byte[] buffer)
        {
#if WINDOWS_STORE
            SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);
            CryptographicKey aes = provider.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(this.key));
            IBuffer result = CryptographicEngine.Decrypt(aes, CryptographicBuffer.CreateFromByteArray(buffer), CryptographicBuffer.CreateFromByteArray(this.iv));

            byte[] decrypted;
            CryptographicBuffer.CopyToByteArray(result, out decrypted);

            return decrypted;
#else
            using (System.Security.Cryptography.Aes aes = AesManaged.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;

                aes.IV = iv;
                aes.Key = key;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                }
            }
#endif
        }
    }
}
