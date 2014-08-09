using System;
using System.Text;
using System.Linq;
using FxSyncNet.Security;
using FxSyncNet.Util;

#if WINDOWS_STORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace FxSyncNet.Tests
{
    [TestClass]
    public class CredentialsTests
    {
        [TestMethod]
        public void TestClientStretchKdfVector()
        {
            string email = GetUTF8String("616e6472c3a9406578616d706c652e6f7267");
            string password = GetUTF8String("70c3a4737377c3b67264");

            Credentials credentials = new Credentials(email, password);

            Assert.AreEqual("e4e8889bd8bd61ad6de6b95c059d56e7b50dacdaf62bd84644af7e2add84345d", BinaryHelper.ToHexString(credentials.QuickStretchedPW));
            Assert.AreEqual("247b675ffb4c46310bc87e26d712153abe5e1c90ef00a4784594f97ef54f2375", BinaryHelper.ToHexString(credentials.AuthPW));
            Assert.AreEqual("de6a2648b78284fcb9ffa81ba95803309cfba7af583c01a8a1a63e567234dd28", BinaryHelper.ToHexString(credentials.UnwrapBKey));
        }

        [TestMethod]
        public void TestGenerateSyncClientState()
        {
            byte[] unwrapBKey = BinaryHelper.FromHexString("6b813696a1f83a87e41506b7f33b991b985f3d6e0c934f867544e711882c179c");

            HashAlgorithm sha256 = HashAlgorithm.Create("sha256");
            byte[] hash = sha256.ComputeHash(unwrapBKey);

            string clientState = BinaryHelper.ToHexString(hash.Take(16).ToArray());

            Assert.AreEqual("630b070cdd4369880a82762436c5399d", clientState);
        }

        [TestMethod]
        public void TestUnbundleKeyFetchResponse()
        {
            string key = "0EF298CBE1379952268796C56A319D02F5C77B1674FECC0ED02BC78C253DC771";
            string bundle = "1D1D1FBEBBF0FF02BD94ACEF93C9FA22E98284BABD35FE2FA25471B18D5B3151359D7B0566FA8FC8B37178766620CFB2249C0F669F992EC52E602A431E535DBE9C24DFDE4A2BB860406DE9F457B987DB0CC9175543015D80A0F4694D2CB02716";

            byte[] wrapKB = Credentials.UnbundleKeyFetchResponse(key, bundle);

            Assert.AreEqual("aa8b863d6332ad16e95c5c333c106a00e26a6caebdb4e4f80798ac8344c87da9", BinaryHelper.ToHexString(wrapKB));
        }

        [TestMethod]
        public void TestDeriveHawkCredentials()
        {
            string token = "64C15024284040D0B28A077654E361CF4475F44613A0C5B466633E05922647B3";

            byte[] bundleKey = Credentials.DeriveHawkCredentials(token, "keyFetchToken");

            Assert.AreEqual("57e1ad522d8df76322ba6fbcac96a60420f6645c20b7396aa4981049ae262d80", BinaryHelper.ToHexString(bundleKey));
        }

        private string GetUTF8String(string hexString)
        {
            byte[] buffer = BinaryHelper.FromHexString(hexString);
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
    }
}
