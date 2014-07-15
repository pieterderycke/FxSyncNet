using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_STORE
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using System.Runtime.InteropServices;
#endif

namespace FxSyncNet.Security
{
    public class RSACryptoServiceProvider : IDisposable
    {
#if WINDOWS_STORE
        private readonly CryptographicKey keyPair;
#else
        private readonly System.Security.Cryptography.RSACryptoServiceProvider provider;
#endif

        public RSACryptoServiceProvider(int dwKeySize)
        {
#if WINDOWS_STORE
            AsymmetricKeyAlgorithmProvider provider =
                    AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            keyPair = provider.CreateKeyPair((uint)dwKeySize);
#else
            provider = new System.Security.Cryptography.RSACryptoServiceProvider(dwKeySize);
#endif
        }

        public object PlatformProvider 
        { 
            get 
            { 
#if WINDOWS_STORE
                return keyPair;
#else
                return provider; 
#endif
            } 
        }

        public RSAParameters ExportParameters(bool includePrivateParameters)
        {
#if WINDOWS_STORE
            IBuffer export = keyPair.Export(CryptographicPrivateKeyBlobType.BCryptPrivateKey);

            byte[] result;
            CryptographicBuffer.CopyToByteArray(export, out result);

            BCryptRsaKeyBlob header = BCryptRsaKeyBlob.Load(result);
            int offset = Marshal.SizeOf<BCryptRsaKeyBlob>();

            byte[] exponent = result.Skip(offset).Take((int)header.cbPublicExp).ToArray();

            offset += (int)header.cbPublicExp;
            byte[] modulus = result.Skip(offset).Take((int)header.cbModulus).ToArray();

            return new RSAParameters(exponent, modulus);
#else
            System.Security.Cryptography.RSAParameters parameters =
                provider.ExportParameters(includePrivateParameters);

            return new RSAParameters(parameters.Exponent, parameters.Modulus);
#endif
        }

        public void Dispose()
        {
#if WINDOWS_STORE
#else
            this.provider.Dispose();
            GC.SuppressFinalize(this); // if DisposableClass isn't sealed
#endif
        }
    }
}
