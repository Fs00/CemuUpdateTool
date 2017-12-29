using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [TestClass]
    public class VersionNumberTests
    {
        [TestMethod]
        public void ValidStringConstructor()
        {
            string vNumber = "1.5.2";
            VersionNumber version = new VersionNumber(vNumber);
            Assert.AreEqual(version.Depth, 3);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 5);
            Assert.AreEqual(version.Build, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyAccessViolation()
        {
            string vNumber = "1.5";
            VersionNumber version = new VersionNumber(vNumber);
            Console.WriteLine(version.Build);   // should throw InvalidOperationException
        }

        [TestMethod]
        public void ValidFileVersionInfoConstructor_StandardDepth()
        {
            FileVersionInfo testFile = null;
            try
            {
                testFile = FileVersionInfo.GetVersionInfo("./versionTest.exe");
            }
            catch (System.IO.FileNotFoundException)
            {
                Assert.Inconclusive("Test file not found.");
            }

            VersionNumber version = new VersionNumber(testFile);
            Assert.AreEqual(version.Depth, 4);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 11);
            Assert.AreEqual(version.Build, 2);
            Assert.AreEqual(version.Private, 0);
        }

        [TestMethod]
        public void ValidFileVersionInfoConstructor_CustomDepth()
        {
            FileVersionInfo testFile = null;
            try
            {
                testFile = FileVersionInfo.GetVersionInfo("./versionTest.exe");
            }
            catch (System.IO.FileNotFoundException)
            {
                Assert.Inconclusive("Test file not found.");
            }

            VersionNumber version = new VersionNumber(testFile, 2);
            Assert.AreEqual(version.Depth, 2);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 11);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            string vNumber1 = "1.5.2";
            string vNumber2 = "3";

            VersionNumber version1 = new VersionNumber(vNumber1);
            VersionNumber version2 = new VersionNumber(vNumber2);

            Assert.AreEqual(version1.ToString(), vNumber1, "Failed version1.ToString()");
            Assert.AreEqual(version2.ToString(), vNumber2, "Failed version2.ToString()");
        }
    }
}
