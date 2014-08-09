using FxSyncNet.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FxSyncNet.Util;

#if WINDOWS_STORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace FxSyncNet.Tests
{
    [TestClass]
    public class SHA256Tests
    {
        [TestMethod]
        public void SHA256Test1()
        {
            SHA256 sha256 = new SHA256();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes("TestBufer"));

            Assert.AreEqual("35c31cc5553afe41b319d088523f90edecfe5435420bf57f1c939e14db105c3f", BinaryHelper.ToHexString(hash));
        }
    }
}
