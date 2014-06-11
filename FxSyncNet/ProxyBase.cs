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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        protected Task<TResponse> Get<TResponse>(string requestUri, string assertion, string clientState)
        {
            return (Task<TResponse>)Execute(HttpMethod.Get, requestUri, null, typeof(TResponse), AuthenticationMethod.BrowserId, assertion, null, 0, clientState);
        }

        protected Task<TResponse> Get<TResponse>(string requestUri, string token, string context, int size)
        {
            return (Task<TResponse>)Execute(HttpMethod.Get, requestUri, null, typeof(TResponse), AuthenticationMethod.Hawk, token, context, size, null);
        }

        protected Task Get(string requestUri, string token, string context, int size)
        {
            return Execute(HttpMethod.Get, requestUri, null, null, AuthenticationMethod.Hawk, token, context, size, null);
        }

        protected Task<TResponse> Post<TRequest, TResponse>(string requestUri, TRequest request)
        {
            return (Task<TResponse>)Execute(HttpMethod.Post, requestUri, request, typeof(TResponse), AuthenticationMethod.Anonymous, null, null, 0, null);
        }

        protected Task<TResponse> Post<TRequest, TResponse>(string requestUri, TRequest request, string token, string context, int size)
        {
            return (Task<TResponse>)Execute(HttpMethod.Post, requestUri, request, typeof(TResponse), AuthenticationMethod.Hawk, token, context, size, null);
        }

        // TODO: improve with relfection.emit/Expression Trees to provide better async performance
        private Task Execute(HttpMethod method, string requestUri, object request, Type responseType, AuthenticationMethod authenticationMethod, string token, string context, int size, string clientState)
        {
            if(authenticationMethod == AuthenticationMethod.Hawk)
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentNullException("token", "The token must be provided for Hawk authentication.");

                if (string.IsNullOrWhiteSpace(context))
                    throw new ArgumentNullException("context", "The context must be provided for Hawk authentication.");
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(method, requestUri);
            
            if(clientState != null)
                requestMessage.Headers.Add("X-Client-State", clientState);

            string jsonPayload = "";
            if (request != null)
            {
                jsonPayload = JsonConvert.SerializeObject(request);
                requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            switch(authenticationMethod)
            {
                case AuthenticationMethod.Anonymous:
                    break;
                case AuthenticationMethod.Hawk:
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Hawk", Hawk(method, requestUri, token, jsonPayload, context, size));
                    break;
                case AuthenticationMethod.BrowserId:
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("BrowserID", token);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unknown authentication method \"{0}\".", authenticationMethod), "authenticationMethod");
            }

            HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
            if(response.IsSuccessStatusCode)
            {
                if(responseType != null)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    
                    MethodInfo fromResult = typeof(Task).GetMethod("FromResult").MakeGenericMethod(responseType);
                    Task task = (Task)fromResult.Invoke(null, new object[] { JsonConvert.DeserializeObject(data, responseType) });

                    return task;
                }
                else
                {
                    return Task.Run(() => { });
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

        
        // https://github.com/hueniverse/hawk/blob/master/lib/client.js
        // https://github.com/mozilla/fxa-python-client
        private string Hawk(HttpMethod method, string requestUri, string token, string jsonPayload, string context, int size)
        {
            string tokenId;
            string reqHMACkey;

            using (var hmac = new HMACSHA256())
            {
                HKDF hkdf = new HKDF(hmac, Util.FromHexString(token));
                byte[] sessionToken = hkdf.Expand(Util.Kw(context), size);

                string buffer = Util.ToHexString(sessionToken);

                tokenId = buffer.Substring(0, 64);
                reqHMACkey = buffer.Substring(64, 64);
            }

            HawkNet.HawkCredential credential =
                new HawkNet.HawkCredential() { Algorithm = "sha256", Key = reqHMACkey, Id = tokenId };

            return HawkNet.Hawk.GetAuthorizationHeader(
                httpClient.BaseAddress.DnsSafeHost,
                method.ToString().ToUpperInvariant(),
                new Uri(httpClient.BaseAddress, requestUri), 
                credential,
                payloadHash: HawkNet.Hawk.CalculatePayloadHash(jsonPayload, "application/json", credential));
        }
    }
}
