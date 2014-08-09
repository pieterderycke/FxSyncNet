using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Diagnostics;
using FxSyncNet;
using FxSyncNet.Security;
using FxSyncNet.Util;

namespace HawkNet
{
    /// <summary>
    /// Hawk main class. It provides methods for generating a Hawk authorization header on the client side and authenticate it on the
    /// service side.
    /// </summary>
    internal static class Hawk
    {
        readonly static string[] RequiredAttributes = { "id", "ts", "mac", "nonce" };
        readonly static string[] OptionalAttributes = { "ext", "hash" };
        readonly static string[] SupportedAttributes;
        readonly static string[] SupportedAlgorithms = { "sha1", "sha256" };

        readonly static string RandomSource = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static Hawk()
        {
            SupportedAttributes = RequiredAttributes.Concat(OptionalAttributes).ToArray();
        }

        /// <summary>
        /// Creates a new Hawk Authorization header based on the provided parameters
        /// </summary>
        /// <param name="host">Host header</param>
        /// <param name="method">Request method</param>
        /// <param name="uri">Request uri</param>
        /// <param name="credential">Credential used to calculate the MAC</param>
        /// <param name="ext">Optional extension attribute</param>
        /// <param name="ts">Timestamp</param>
        /// <param name="nonce">Random Nonce</param>
        /// <param name="payloadHash">Hash of the request payload</param>
        /// <param name="type">Type used as header for the normalized string. Default value is 'header'</param>
        /// <returns>Hawk authorization header</returns>
        public static string GetAuthorizationHeader(string host, string method, Uri uri, HawkCredential credential, string ext = null, DateTime? ts = null, string nonce = null, string payloadHash = null, string type = null)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentException("The host can not be null or empty", "host");

            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("The method can not be null or empty", "method");

            if (credential == null)
                throw new ArgumentNullException("The credential can not be null", "credential");

            if (string.IsNullOrEmpty(nonce))
            {
                nonce = GetRandomString(6);
            }

            if (string.IsNullOrEmpty(type))
            {
                type = "header";
            }

            var normalizedTs = ((int)Math.Floor((ConvertToUnixTimestamp((ts.HasValue)
                ? ts.Value : DateTime.UtcNow)))).ToString();

            var mac = CalculateMac(host,
                method,
                uri,
                ext,
                normalizedTs,
                nonce,
                credential,
                type,
                payloadHash);

            var authorization = string.Format("id=\"{0}\", ts=\"{1}\", nonce=\"{2}\", mac=\"{3}\"",
                    credential.Id, normalizedTs, nonce, mac);

            if (!string.IsNullOrEmpty(payloadHash))
            {
                authorization += string.Format(", hash=\"{0}\"", payloadHash);
            }

            return authorization;
        }

        /// <summary>
        /// Gets a random string of a given size
        /// </summary>
        /// <param name="size">Expected size for the generated string</param>
        /// <returns>Random string</returns>
        public static string GetRandomString(int size)
        {
            var result = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < size; ++i)
            {
                result.Append(RandomSource[random.Next(RandomSource.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Computes a mac following the Hawk rules
        /// </summary>
        /// <param name="host">Host header</param>
        /// <param name="method">Request method</param>
        /// <param name="uri">Request uri</param>
        /// <param name="ext">Extesion attribute</param>
        /// <param name="ts">Timestamp</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="credential">Credential</param>
        /// <param name="payload">Hash of the request payload</param>
        /// <returns>Generated mac</returns>
        public static string CalculateMac(string host, string method, Uri uri, string ext, string ts, string nonce, HawkCredential credential, string type, string payloadHash = null)
        {
            var hmac = new HMAC("hmac" + credential.Algorithm);
            hmac.Key = BinaryHelper.FromHexString(credential.Key);

            var sanitizedHost = (host.IndexOf(':') > 0) ?
                host.Substring(0, host.IndexOf(':')) :
                host;

            var normalized = "hawk.1." + type + "\n" +
                        ts + "\n" +
                        nonce + "\n" +
                        method.ToUpper() + "\n" +
                        uri.PathAndQuery + "\n" +
                        sanitizedHost.ToLower() + "\n" +
                        uri.Port.ToString() + "\n" +
                        ((!string.IsNullOrEmpty(payloadHash)) ? payloadHash : "") + "\n" +
                        "\n";

            var messageBytes = Encoding.UTF8.GetBytes(normalized);

            var mac = hmac.ComputeHash(messageBytes);

            var encodedMac = Convert.ToBase64String(mac);

            return encodedMac;
        }

        /// <summary>
        /// Converts a Datatime to an equivalent Unix Timestamp, in seconds
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Generates a mac hash using the supplied payload and credentials
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="credential"></param>
        /// <returns></returns>
        public static string CalculatePayloadHash(string payload, string mediaType, HawkCredential credential)
        {
            var normalized = "hawk.1.payload\n" +
                        mediaType + "\n" +
                        payload + "\n";

            var algorithm = HashAlgorithm.Create(credential.Algorithm);

            var encodedMac = Convert.ToBase64String(algorithm
                .ComputeHash(Encoding.UTF8.GetBytes(normalized)));
            
            return encodedMac;
        }

        // Fixed time comparision
        private static bool IsEqual(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}
