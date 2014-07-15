﻿using System;
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
    public class HMAC : IDisposable
    {
#if WINDOWS_STORE
        private readonly string algorithmName;
        private readonly byte[] key;
        private readonly int hashSize;
#else
        private System.Security.Cryptography.HMAC hmac;
#endif

        public HMAC(string algorithmName)
        {
            if (algorithmName != "HMACSHA256")
                throw new ArgumentException(string.Format("Unsupported algorihtm \"{0}\".", algorithmName), "algorithmName");

#if WINDOWS_STORE
            this.algorithmName = algorithmName;
            this.hashSize = 256;
#else
            this.hmac = System.Security.Cryptography.HMAC.Create(algorithmName);
#endif
        }

        public HMAC(string algorithmName, byte[] key) : this(algorithmName)
        {
#if WINDOWS_STORE
            this.key = key;
#else
            hmac.Key = key;
#endif
        }

        public byte[] Key 
        { 
            get 
            { 
#if WINDOWS_STORE
                return key;
#else
                return hmac.Key; 
#endif
            } 
            set 
            { 
#if WINDOWS_STORE
#else
                hmac.Key = value; 
#endif
            } 
        }

        public int HashSize 
        { 
            get 
            { 
#if WINDOWS_STORE
                return hashSize;
#else
                return hmac.HashSize;
#endif
            } 
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
#if WINDOWS_STORE
            MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(algorithmName);
            CryptographicHash result = provider.CreateHash(CryptographicBuffer.CreateFromByteArray(Key));
            result.Append(CryptographicBuffer.CreateFromByteArray(buffer));
            
            byte[] hash;
            CryptographicBuffer.CopyToByteArray(result.GetValueAndReset(), out hash);

            return hash;
#else
            return hmac.ComputeHash(buffer, offset, count);
#endif
        }

        public void Dispose()
        {
#if WINDOWS_STORE
#else
            this.hmac.Dispose();
            GC.SuppressFinalize(this); // if DisposableClass isn't sealed
#endif
        }
    }
}
