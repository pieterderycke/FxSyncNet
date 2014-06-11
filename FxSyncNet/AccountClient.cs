﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.ApiModels;
using System.Security.Cryptography;
using System.Numerics;

namespace FxSyncNet
{
    public class AccountClient : ProxyBase
    {
        public AccountClient() : base("https://api.accounts.firefox.com/v1/")
        {
        }

        public Task<LoginResponse> Login(string email, string password)
        {
            return Login(email, password, false);
        }

        public Task<LoginResponse> Login(string email, string password, bool keys)
        {
            return Post<LoginRequest, LoginResponse>("account/login" + (keys ? "?keys=true" : ""), new LoginRequest(email, password));
        }

        public Task<DevicesResponse> Devices(string sessionToken)
        {
            return Get<DevicesResponse>("account/devices", sessionToken, "sessionToken", 2 * 32);
        }

        public Task<KeysResponse> Keys(string keyFetchToken)
        {
            return Get<KeysResponse>("account/keys", keyFetchToken, "keyFetchToken", 3 * 32);
        }

        public Task SessionStatus(string sessionToken)
        {
            return Get("session/status", sessionToken, "sessionToken", 2 * 32);
        }

        public Task<CertificateSignResponse> CertificateSign(string sessionToken, RSACryptoServiceProvider rsa, TimeSpan duration)
        {
            RSAParameters keyInfo = rsa.ExportParameters(false);

            string n = Util.BigIntegerFromBigEndian(keyInfo.Modulus).ToString();
            string e = Util.BigIntegerFromBigEndian(keyInfo.Exponent).ToString();

            CertificateSignRequest signRequest = new CertificateSignRequest();
            signRequest.PublicKey = new PublicKey() { Algorithm = "RS", E = e, N = n };
            signRequest.Duration = (long)duration.TotalMilliseconds;

            return Post<CertificateSignRequest, CertificateSignResponse>("certificate/sign", signRequest, sessionToken, "sessionToken", 2 * 32);
        }
    }
}