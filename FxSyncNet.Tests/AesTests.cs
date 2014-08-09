using FxSyncNet.Security;
using FxSyncNet.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Tests
{
    [TestClass]
    public class AesTests
    {
        [TestMethod]
        public void DecryptTest()
        {
            byte[] key = BinaryHelper.FromHexString("603deb1015ca71be2b73aef0857d77811f352c073b6108d72d9810a30914dff4");
            byte[] iv = BinaryHelper.FromHexString("000102030405060708090A0B0C0D0E0F");
            byte[] buffer = BinaryHelper.FromHexString("6bc1bee22e409f96e93d7e117393172a");

            Aes aes = new Aes(key, iv);
            string cipherText = BinaryHelper.ToHexString(aes.Decrypt(buffer));

            Assert.AreEqual("f58c4c04d6e5f1ba779eabfb5f7bfbd6", cipherText);
        }
    }
}
