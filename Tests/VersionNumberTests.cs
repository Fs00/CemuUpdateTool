using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [DeploymentItem(@"Resources\versionTest.exe")]
    [TestClass]
    public class VersionNumberTests
    {
        [TestMethod]
        public void ValidStringConstructor()
        {
            string vNumber = "1.5.2";
            VersionNumber version = new VersionNumber(vNumber);
            Assert.AreEqual(version.Length, 3);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 5);
            Assert.AreEqual(version.Build, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PropertyAccessViolation()
        {
            string vNumber = "1.5";
            VersionNumber version = new VersionNumber(vNumber);
            Console.WriteLine(version.Build);   // should throw
        }

        [TestMethod]
        public void ValidFileVersionInfoConstructor_StandardLength()
        {
            FileVersionInfo testFile = FileVersionInfo.GetVersionInfo("./versionTest.exe");

            VersionNumber version = new VersionNumber(testFile);

            Assert.AreEqual(version.Length, 4);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 2);
            Assert.AreEqual(version.Build, 3);
            Assert.AreEqual(version[3], 0);
        }

        [TestMethod]
        public void ValidFileVersionInfoConstructor_CustomLength()
        {
            FileVersionInfo testFile = FileVersionInfo.GetVersionInfo("./versionTest.exe");

            VersionNumber version = new VersionNumber(testFile, 2);

            Assert.AreEqual(version.Length, 2);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ErraticParamsIntArrayConstructor()
        {
            new VersionNumber(-1,0,5);  // must throw ArgumentException because of -1
        }

        [TestMethod]
        public void ToStringTest()
        {
            string vNumber1 = "1.5.2";
            string vNumber2 = "3";

            VersionNumber version1 = new VersionNumber(vNumber1);
            VersionNumber version2 = new VersionNumber(vNumber2);

            Assert.AreEqual(version1.ToString(), vNumber1);
            Assert.AreEqual(version2.ToString(), vNumber2);
        }

        [TestMethod]
        public void CopyOfLength()
        {
            VersionNumber version = new VersionNumber(1,7,1);

            Assert.AreEqual("1.7", version.GetCopyOfLength(2).ToString());
            Assert.AreEqual("1.7.1.0.0", version.GetCopyOfLength(5).ToString());
            Assert.AreEqual(string.Empty, version.GetCopyOfLength(0).ToString());
        }

        [TestMethod]
        public void ToStringEmpty()
        {
            VersionNumber empty = VersionNumber.Empty;
            Assert.AreEqual(string.Empty, empty.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IncrementDecrementOperatorsTest()
        {
            VersionNumber version1 = new VersionNumber(1,1,2);
            VersionNumber version2 = new VersionNumber(2,0,0);

            version1++;

            Assert.AreEqual(version1.ToString(), "1.1.3");
            version2--;     // throws because last segment is 0
        }

        [TestMethod]
        public void CompareTo_SameLength()
        {
            VersionNumber version1 = new VersionNumber(1, 5, 4);
            VersionNumber version2 = new VersionNumber(1, 8, 3);

            Assert.AreEqual(-1, version1.CompareTo(version2));
            Assert.AreEqual(1, version2.CompareTo(version1));
            Assert.AreEqual(0, version1.CompareTo(version1));
        }

        [TestMethod]
        public void CompareTo_DifferentLengthSameValue()
        {
            VersionNumber version1 = new VersionNumber(1, 3);
            VersionNumber version2 = new VersionNumber(1, 3, 0, 0);

            Assert.AreEqual(0, version1.CompareTo(version2));
            Assert.AreEqual(0, version2.CompareTo(version1));
        }

        [TestMethod]
        public void CompareTo_DifferentLengthSameCommonSegments()
        {
            VersionNumber version1 = new VersionNumber(1, 7, 0, 1);
            VersionNumber version2 = new VersionNumber(1, 7);

            Assert.AreEqual(1, version1.CompareTo(version2));
            Assert.AreEqual(-1, version2.CompareTo(version1));
        }

        [TestMethod]
        public void ComparisonOperators_DifferentValues()
        {
            VersionNumber version1 = new VersionNumber(1, 9, 1);
            VersionNumber version2 = new VersionNumber(1, 10);

            Assert.IsFalse(version1 >= version2);
            Assert.IsTrue(version2 > version1);
            Assert.IsTrue(version1 != version2);
        }

        [TestMethod]
        public void ComparisonOperators_NullsLeft()
        {
            VersionNumber versionNumber = new VersionNumber(1, 0);

            Assert.IsTrue(null != versionNumber);
            Assert.IsTrue(null < versionNumber);

            Assert.IsFalse(null == versionNumber);
            Assert.IsFalse(null > versionNumber);
        }

        [TestMethod]
        public void ComparisonOperators_NullsRight()
        {
            VersionNumber versionNumber = new VersionNumber(1, 0);

            Assert.IsTrue(versionNumber != null);
            Assert.IsTrue(versionNumber > null);

            Assert.IsFalse(versionNumber == null);
            Assert.IsFalse(versionNumber < null);
        }

        [TestMethod]
        public void IsSubVersion()
        {
            VersionNumber version = new VersionNumber(1, 5);
            VersionNumber subVersion = new VersionNumber(1, 5, 3, 1);
            VersionNumber randomVersion = new VersionNumber(1, 7);

            Assert.IsTrue(subVersion.IsSubVersionOf(version));
            Assert.IsFalse(version.IsSubVersionOf(subVersion));
            Assert.IsFalse(version.IsSubVersionOf(version));
            Assert.IsFalse(subVersion.IsSubVersionOf(randomVersion));
        }

        [TestMethod]
        public void IsSubVersion_EmptyAndNull()
        {
            VersionNumber subVersion = new VersionNumber(1, 5, 3, 1);

            Assert.IsTrue(subVersion.IsSubVersionOf(VersionNumber.Empty));
            Assert.IsFalse(subVersion.IsSubVersionOf(null));
        }
    }
}
