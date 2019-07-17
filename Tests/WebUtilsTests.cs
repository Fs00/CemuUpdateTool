using CemuUpdateTool.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [TestClass()]
    public class WebUtilsTests
    {
        // TO BE UPDATED WHEN THERE'S A NEW CEMU VERSION
        private readonly VersionNumber latestKnownCemuVersion = new VersionNumber(1, 15, 10);

        [TestMethod()]
        public void RemoteVersionCheckTest_NoStartingVersion()
        {
            MyWebClient client = new MyWebClient();
            client.BaseAddress = "http://cemu.info/releases/cemu_";
            string cemuUrlSuffix = ".zip";

            VersionNumber result = WebUtils.GetLatestRemoteVersionInBranch(VersionNumber.Empty, client, cemuUrlSuffix, maxVersionLength: 3, null);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_CorrectStartingVersion()
        {
            MyWebClient client = new MyWebClient();
            client.BaseAddress = "http://cemu.info/releases/cemu_";
            string cemuUrlSuffix = ".zip";
            VersionNumber startingVersion = new VersionNumber(1, 10, 0);

            VersionNumber result = WebUtils.GetLatestRemoteVersionInBranch(VersionNumber.Empty, client, cemuUrlSuffix, maxVersionLength: 3, startingVersion);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_WrongStartingVersion()
        {
            MyWebClient client = new MyWebClient();
            client.BaseAddress = "http://cemu.info/releases/cemu_";
            string cemuUrlSuffix = ".zip";
            VersionNumber startingVersion = new VersionNumber(65, 10, 0);

            VersionNumber result = WebUtils.GetLatestRemoteVersionInBranch(VersionNumber.Empty, client, cemuUrlSuffix, maxVersionLength: 3, startingVersion);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_WrongExistingStartingVersion_BranchLengthMoreThan0()
        {
            MyWebClient client = new MyWebClient();
            client.BaseAddress = "http://cemu.info/releases/cemu_";
            string cemuUrlSuffix = ".zip";
            VersionNumber startingVersion = new VersionNumber(1, 7, 5);
            VersionNumber latestKnownCemuVersion = new VersionNumber(1, 4, 2);     // latest version of branch 1.4.x

            VersionNumber result = WebUtils.GetLatestRemoteVersionInBranch(new VersionNumber(1,4), client, cemuUrlSuffix, maxVersionLength: 3, startingVersion);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }
    }
}