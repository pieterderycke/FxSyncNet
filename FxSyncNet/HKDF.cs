using System;
using System.Security.Cryptography;

namespace RFC5869
{
    /// <summary>
    /// HMAC-based Extract-and-Expand Key Derivation Function (HKDF)
    /// https://tools.ietf.org/html/rfc5869
    /// </summary>
    public class HKDF
    {
        private readonly HMAC hmac;
        private readonly int hashLength;
        private readonly byte[] prk;

        /// <summary>
        /// Initializes a new instance of the <see cref="HKDF"/> class.
        /// </summary>
        /// <param name="hmac">The HMAC hash function to use.</param>
        /// <param name="ikm">input keying material.</param>
        /// <param name="salt">optional salt value (a non-secret random value); if not provided, it is set to a string of HMAC.HashSize/8 zeros.</param>
        public HKDF(HMAC hmac, byte[] ikm, byte[] salt = null)
        {
            this.hmac = hmac;
            this.hashLength = hmac.HashSize / 8;

            // now we compute the PRK
            hmac.Key = salt ?? new byte[this.hashLength];
            this.prk = hmac.ComputeHash(ikm);
        }

        /// <summary>
        /// Expands the specified info.
        /// </summary>
        /// <param name="info">optional context and application specific information (can be a zero-length string)</param>
        /// <param name="l">length of output keying material in octets (&lt;= 255*HashLen)</param>
        /// <returns>OKM (output keying material) of L octets</returns>
        public byte[] Expand(byte[] info, int l)
        {
            if (info == null) info = new byte[0];

            hmac.Key = this.prk;

            var n = (int) System.Math.Ceiling(l * 1f / hashLength);
            var t = new byte[n * hashLength];

            using (var ms = new System.IO.MemoryStream())
            {
                var prev = new byte[0];

                for (var i = 1; i <= n; i++)
                {
                    ms.Write(prev, 0, prev.Length);
                    if (info.Length > 0) ms.Write(info, 0, info.Length);
                    ms.WriteByte((byte)(0x01 * i));

                    prev = hmac.ComputeHash(ms.ToArray());

                    Array.Copy(prev, 0, t, (i - 1) * hashLength, hashLength);

                    ms.SetLength(0); //reset
                }
            }

            var okm = new byte[l];
            Array.Copy(t, okm, okm.Length);

            return okm;
        }
    }
}
