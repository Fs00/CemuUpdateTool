using CemuUpdateTool.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [TestClass()]
    public class WebUtilsTests
    {
        // TO BE UPDATED WHEN THERE'S A NEW CEMU VERSION
        private readonly VersionNumber latestKnownCemuVersion = new VersionNumber(1, 15, 10);

        private readonly string cemuUrlPrefix = "http://cemu.info/releases/cemu_";
        private readonly string cemuUrlSuffix = ".zip";

        private RemoteVersionChecker versionChecker;

        public WebUtilsTests()
        {
            versionChecker = new RemoteVersionChecker(cemuUrlPrefix, cemuUrlSuffix, 3);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_NoStartingVersion()
        {
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_CorrectStartingVersion()
        {
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, new VersionNumber(1, 10, 0));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_WrongStartingVersion()
        {
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, new VersionNumber(65, 10, 0));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod()]
        public void RemoteVersionCheckTest_UselessExistingStartingVersion()
        {
            VersionNumber latestKnownCemuVersion = new VersionNumber(1, 4, 2);     // latest version of branch 1.4.x

            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(new VersionNumber(1,4), new VersionNumber(1, 7, 5));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }
    }
}