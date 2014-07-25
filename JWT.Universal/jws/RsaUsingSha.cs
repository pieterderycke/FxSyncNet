using System;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Jose
{
    public class RsaUsingSha : IJwsAlgorithm
    {
        private string hashMethod;

        public RsaUsingSha(string hashMethod)
        {
            this.hashMethod = hashMethod;
        }

        public byte[] Sign(byte[] securedInput, object key)
        {
            //using (var sha = HashAlgorithm)
            //{
                //var privateKey = Ensure.Type<AsymmetricAlgorithm>(key, "RsaUsingSha alg expects key to be of AsymmetricAlgorithm type.");

                AsymmetricKeyAlgorithmProvider provider = 
                    AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaSignPkcs1Sha256);

            //CryptographicKey cryptographicKey = (CryptographicKey)key;
            CryptographicKey cryptographicKey = provider.ImportKeyPair(((CryptographicKey)key).Export(CryptographicPrivateKeyBlobType.BCryptPrivateKey), 
                CryptographicPrivateKeyBlobType.BCryptPrivateKey);

                //provider.ImportKeyPair(null, CryptographicPrivateKeyBlobType.)

                IBuffer signedData =
                    CryptographicEngine.Sign(cryptographicKey, CryptographicBuffer.CreateFromByteArray(securedInput));

                byte[] result;
                CryptographicBuffer.CopyToByteArray(signedData, out result);

                return result;

                //var pkcs1 = new RSAPKCS1SignatureFormatter(privateKey);
                //pkcs1.SetHashAlgorithm(hashMethod);

                //return pkcs1.CreateSignature(sha.ComputeHash(securedInput));                    
            //} 
        }

        public bool Verify(byte[] signature, byte[] securedInput, object key)
        {
            //using (var sha = HashAlgorithm)
            //{
            //    var publicKey = Ensure.Type<AsymmetricAlgorithm>(key, "RsaUsingSha alg expects key to be of AsymmetricAlgorithm type."); 
                
            //    byte[] hash = sha.ComputeHash(securedInput);

            //    var pkcs1 = new RSAPKCS1SignatureDeformatter(publicKey);
            //    pkcs1.SetHashAlgorithm(hashMethod);

            //    return pkcs1.VerifySignature(hash, signature);
            //}

            return true;
        }

        private string GetAlgorithmName()
        { 
            switch(hashMethod)
            {
                case "SHA256":
                    return AsymmetricAlgorithmNames.RsaSignPkcs1Sha256;
                case "SHA384":
                    return AsymmetricAlgorithmNames.RsaSignPkcs1Sha384;
                case "SHA512":
                    return AsymmetricAlgorithmNames.RsaSignPkcs1Sha512;
                default:
                    throw new Exception("Unknown hash method.");
            }
        }
    }
}