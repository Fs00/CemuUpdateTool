using System;
using System.Threading;
using CemuUpdateTool.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [TestClass]
    public class WebUtilsTests
    {
        // TO BE UPDATED WHEN THERE'S A NEW CEMU VERSION
        private readonly VersionNumber latestKnownCemuVersion = new VersionNumber(1, 15, 11);

        private const string CEMU_URL_PREFIX = "http://cemu.info/releases/cemu_";
        private const string CEMU_URL_SUFFIX = ".zip";

        private RemoteVersionChecker CreateDefaultVersionCheckerForCemu()
        {
            return new RemoteVersionChecker(CEMU_URL_PREFIX, CEMU_URL_SUFFIX, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void RemoteVersionCheckTest_CancelledBeforeStarting()
        {
            var versionChecker = new RemoteVersionChecker(CEMU_URL_PREFIX, CEMU_URL_SUFFIX, 3, new CancellationToken(true));
            versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty);
        }

        [TestMethod]
        public void RemoteVersionCheckTest_NoStartingVersion()
        {
            var versionChecker = CreateDefaultVersionCheckerForCemu();
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty);
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod]
        public void RemoteVersionCheckTest_CorrectStartingVersion()
        {
            var versionChecker = CreateDefaultVersionCheckerForCemu();
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, new VersionNumber(1, 10, 0));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod]
        public void RemoteVersionCheckTest_WrongStartingVersion()
        {
            var versionChecker = CreateDefaultVersionCheckerForCemu();
            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(VersionNumber.Empty, new VersionNumber(65, 10, 0));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }

        [TestMethod]
        public void RemoteVersionCheckTest_UselessExistingStartingVersion()
        {
            var versionChecker = CreateDefaultVersionCheckerForCemu();
            VersionNumber latestKnownCemuVersion = new VersionNumber(1, 4, 2);     // latest version of branch 1.4.x

            VersionNumber result = versionChecker.GetLatestRemoteVersionInBranch(new VersionNumber(1,4), new VersionNumber(1, 7, 5));
            Assert.AreEqual(latestKnownCemuVersion, result);
        }
    }
}