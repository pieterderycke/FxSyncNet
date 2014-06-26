using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace FxSyncNet.Tests
{
    [TestClass]
    public class CredentialsTests
    {
        [TestMethod]
        public void TestClientStretchKdfVector()
        {
            string email = Encoding.UTF8.GetString(Util.FromHexString("616e6472c3a9406578616d706c652e6f7267"));
            string password = Encoding.UTF8.GetString(Util.FromHexString("70c3a4737377c3b67264"));

            Credentials credentials = new Credentials(email, password);

            Assert.AreEqual("E4E8889BD8BD61AD6DE6B95C059D56E7B50DACDAF62BD84644AF7E2ADD84345D", Util.ToHexString(credentials.QuickStretchedPW));
            Assert.AreEqual("247B675FFB4C46310BC87E26D712153ABE5E1C90EF00A4784594F97EF54F2375", Util.ToHexString(credentials.AuthPW));
            Assert.AreEqual("DE6A2648B78284FCB9FFA81BA95803309CFBA7AF583C01A8A1A63E567234DD28", Util.ToHexString(credentials.UnwrapBKey));
        }

        [TestMethod]
        public void TestGenerateSyncClientState()
        {
            byte[] unwrapBKey = Util.FromHexString("6b813696a1f83a87e41506b7f33b991b985f3d6e0c934f867544e711882c179c");

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(unwrapBKey);

            string clientState = Util.ToHexString(hash.Take(16).ToArray());

            Assert.AreEqual("630B070CDD4369880A82762436C5399D", clientState);
        }

        [TestMethod]
        public void TestUnbundleKeyFetchResponse()
        {
            string key = "0ef298cbe1379952268796c56a319d02f5c77b1674fecc0ed02bc78c253dc771";
            string bundle = "1d1d1fbebbf0ff02bd94acef93c9fa22e98284babd35fe2fa25471b18d5b3151359d7b0566fa8fc8b37178766620cfb2249c0f669f992ec52e602a431e535dbe9c24dfde4a2bb860406de9f457b987db0cc9175543015d80a0f4694d2cb02716";

            byte[] wrapKB = Credentials.UnbundleKeyFetchResponse(key, bundle);

            Assert.AreEqual("AA8B863D6332AD16E95C5C333C106A00E26A6CAEBDB4E4F80798AC8344C87DA9", Util.ToHexString(wrapKB));
        }

        [TestMethod]
        public void TestDeriveHawkCredentials()
        {
            string token = "64C15024284040D0B28A077654E361CF4475F44613A0C5B466633E05922647B3";

            byte[] bundleKey = Credentials.DeriveHawkCredentials(token, "keyFetchToken");

            Assert.AreEqual("57E1AD522D8DF76322BA6FBCAC96A60420F6645C20B7396AA4981049AE262D80", Util.ToHexString(bundleKey));
        }
    }
}
