using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_STORE
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
#endif

namespace FxSyncNet.Security
{
    public class SHA256 : HashAlgorithm, IDisposable
    {
        public override byte[] ComputeHash(byte[] buffer)
        {
#if WINDOWS_STORE
            HashAlgorithmProvider provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            CryptographicHash sha256 = provider.CreateHash();

            sha256.Append(CryptographicBuffer.CreateFromByteArray(buffer));
            IBuffer hashValue = sha256.GetValueAndReset();

            byte[] result;
            CryptographicBuffer.CopyToByteArray(hashValue, out result);

            return result;
#else
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(buffer);
            }
#endif
        }

        public void Dispose()
        {
        }
    }
}
