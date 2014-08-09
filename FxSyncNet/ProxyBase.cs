using Newtonsoft.Json;
using RFC5869;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.Security;
using FxSyncNet.Util;

namespace FxSyncNet
{
    public abstract class ProxyBase
    {
        private readonly HttpClient httpClient;

        public ProxyBase(string baseAddress)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
        }

        protected HttpRequestHeaders RequestHeaders { get { return httpClient.DefaultRequestHeaders; } }

        protected Task<TResponse> Get<TResponse>(string requestUri, HawkNet.HawkCredential credential)
        {
            AuthenticationHeaderValue authenticationHeader = GetHawkAuthenticationHeader(HttpMethod.Get, requestUri, null, credential);
            return Task.Run(() => (TResponse)Execute(HttpMethod.Get, requestUri, null, typeof(TResponse), authenticationHeader, null));
        }

        protected Task<TResponse> Get<TResponse>(string requestUri, string assertion, string clientState)
        {
            AuthenticationHeaderValue authenticationHeader = GetBrowserIdAuthenticationHeader(assertion);
            return Task.Run(() => (TResponse)Execute(HttpMethod.Get, requestUri, null, typeof(TResponse), authenticationHeader, clientState));
        }

        protected Task<TResponse> Get<TResponse>(string requestUri, string token, string context, int size)
        {
            AuthenticationHeaderValue authenticationHeader = GetHawkAuthenticationHeader(HttpMethod.Get, requestUri, null, token, context, size);
            return Task.Run(() => (TResponse)Execute(HttpMethod.Get, requestUri, null, typeof(TResponse), authenticationHeader, null));
        }

        protected Task Get(string requestUri, HawkNet.HawkCredential credential)
        {
            AuthenticationHeaderValue authenticationHeader = GetHawkAuthenticationHeader(HttpMethod.Get, requestUri, null, credential);
            return Task.Run(() => Execute(HttpMethod.Get, requestUri, null, null, authenticationHeader, null));
        }

        protected Task Get(string requestUri, string token, string context, int size)
        {
            AuthenticationHeaderValue authenticationHeader = GetHawkAuthenticationHeader(HttpMethod.Get, requestUri, null, token, context, size);
            return Task.Run(() => Execute(HttpMethod.Get, requestUri, null, null, authenticationHeader, null));
        }

        protected Task<TResponse> Post<TRequest, TResponse>(string requestUri, TRequest request)
        {
            string jsonPayload = GetJsonPayload(request);
            return Task.Run(() => (TResponse)Execute(HttpMethod.Post, requestUri, jsonPayload, typeof(TResponse), null, null));
        }

        protected Task<TResponse> Post<TRequest, TResponse>(string requestUri, TRequest request, string token, string context, int size)
        {
            string jsonPayload = GetJsonPayload(request);
            AuthenticationHeaderValue authenticationHeader = GetHawkAuthenticationHeader(HttpMethod.Post, requestUri, jsonPayload, token, context, size);
            return Task.Run(() => (TResponse)Execute(HttpMethod.Post, requestUri, jsonPayload, typeof(TResponse), authenticationHeader, null));
        }

        private object Execute(HttpMethod method, string requestUri, string jsonPayload, Type responseType, AuthenticationHeaderValue authenticationHeader, string clientState)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, requestUri);
            
            if(clientState != null)
                requestMessage.Headers.Add("X-Client-State", clientState);

            if (jsonPayload != null)
            {
                requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            if (authenticationHeader != null)
                requestMessage.Headers.Authorization = authenticationHeader;

            HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
            if(response.IsSuccessStatusCode)
            {
                if(responseType != null)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(data, responseType);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                    throw new ServiceNotAvailableException("The service is not responding and seems to be down.");
                else
                    throw new Exception("Execute failed.");
            }
        }

        private string GetJsonPayload(object request)
        {
            return JsonConvert.SerializeObject(request);
        }
        
        // https://github.com/hueniverse/hawk/blob/master/lib/client.js
        // https://github.com/mozilla/fxa-python-client
        private AuthenticationHeaderValue GetHawkAuthenticationHeader(HttpMethod method, string requestUri, string jsonPayload, string token, string context, int size)
        {
            string tokenId;
            string reqHMACkey;

            using (var hmac = new HMACSHA256())
            {
                HKDF hkdf = new HKDF(hmac, BinaryHelper.FromHexString(token));
                byte[] sessionToken = hkdf.Expand(BinaryHelper.Kw(context), size);

                string buffer = BinaryHelper.ToHexString(sessionToken);

                tokenId = buffer.Substring(0, 64);
                reqHMACkey = buffer.Substring(64, 64);
            }

            HawkNet.HawkCredential credential =
                new HawkNet.HawkCredential() { Algorithm = "sha256", Key = reqHMACkey, Id = tokenId };

            return GetHawkAuthenticationHeader(method, requestUri, jsonPayload, credential);
        }

        private AuthenticationHeaderValue GetHawkAuthenticationHeader(HttpMethod method, string requestUri, string jsonPayload, HawkNet.HawkCredential credential)
        {
            string parameter = HawkNet.Hawk.GetAuthorizationHeader(
                httpClient.BaseAddress.DnsSafeHost,
                method.ToString().ToUpperInvariant(),
                new Uri(httpClient.BaseAddress, requestUri),
                credential,
                payloadHash: HawkNet.Hawk.CalculatePayloadHash(jsonPayload, "application/json", credential));

            return new AuthenticationHeaderValue("Hawk", parameter);
        }

        private AuthenticationHeaderValue GetBrowserIdAuthenticationHeader(string assertion)
        {
            return new AuthenticationHeaderValue("BrowserID", assertion);
        }
    }
}
