using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class HMACSHA256Tests
    {
        [TestMethod]
        public void HMACSHA256Test1()
        {
            HMACSHA256 hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes("TestKey"));
            byte[] hmac = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes("TestBufer"));

            Assert.AreEqual("91e401e8fcb18f0754e748c12a9615581b36176a13947033505bfc532760d366", BinaryHelper.ToHexString(hmac));
        }
    }
}
