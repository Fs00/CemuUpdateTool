using Microsoft.VisualStudio.TestTools.UnitTesting;
using CemuUpdateTool;
using System;

namespace CemuUpdateTool.Tests
{
    [TestClass()]
    public class WebUtilsTests
    {
        [TestMethod()]
        public void RemoteVersionCheckTest_NoStartingVersion()
        {
            MyWebClient client = new MyWebClient();
            client.BaseAddress = "http://cemu.info/releases/cemu_";
            string cemuUrlSuffix = ".zip";
            VersionNumber latestKnownCemuVersion = new VersionNumber(1,11,4);     // TO BE UPDATED EVERY TIME THERE'S A NEW VERSION

            VersionNumber result = WebUtils.GetLatestRemoteVersionInBranch(new VersionNumber(), client, cemuUrlSuffix, maxDepth: 3, null, null);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }
    }
}