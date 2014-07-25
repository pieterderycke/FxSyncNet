using System;
using System.Collections.Generic;

namespace Jose
{
    public class AesGcmKeyWrapManagement : IKeyManagement
    {
        private int keyLengthBits;

        public AesGcmKeyWrapManagement(int keyLengthBits)
        {
            this.keyLengthBits = keyLengthBits;
        }


        public byte[][] WrapNewKey(int cekSizeBits, object key, IDictionary<string, object> header)
        {
            byte[] sharedKey = Ensure.Type<byte[]>(key, "AesGcmKeyWrapManagement alg expectes key to be byte[] array.");
            Ensure.BitSize(sharedKey, keyLengthBits, string.Format("AesGcmKeyWrapManagement management algorithm expected key of size {0} bits, but was given {1} bits", keyLengthBits, sharedKey.Length * 8));

            byte[] iv = Arrays.Random(96);
            byte[] cek = Arrays.Random(cekSizeBits);

            byte[][] cipherAndTag = AesGcm.Encrypt(sharedKey, iv, null, cek);
            
            header["iv"] = Compact.Base64UrlEncode(iv);
            header["tag"] = Compact.Base64UrlEncode(cipherAndTag[1]);

            return new[] {cek, cipherAndTag[0]};
        }

        public byte[] Unwrap(byte[] encryptedCek, object key, int cekSizeBits, IDictionary<string, object> header)
        {
            byte[] sharedKey = Ensure.Type<byte[]>(key, "AesGcmKeyWrapManagement alg expectes key to be byte[] array.");
            Ensure.BitSize(sharedKey, keyLengthBits, string.Format("AesGcmKeyWrapManagement management algorithm expected key of size {0} bits, but was given {1} bits", keyLengthBits, sharedKey.Length * 8));

            Ensure.Contains(header, new[] { "iv" }, "AesGcmKeyWrapManagement algorithm expects 'iv' param in JWT header, but was not found");
            Ensure.Contains(header, new[] { "tag" }, "AesGcmKeyWrapManagement algorithm expects 'tag' param in JWT header, but was not found");

            byte[] iv = Compact.Base64UrlDecode((string) header["iv"]);
            byte[] authTag = Compact.Base64UrlDecode((string) header["tag"]);

            return AesGcm.Decrypt(sharedKey, iv, null, encryptedCek, authTag);
        }
    }
}