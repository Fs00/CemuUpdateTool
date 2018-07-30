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
            Assert.AreEqual(version.Length, 3);
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
        public void ValidFileVersionInfoConstructor_StandardLength()
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
            Assert.AreEqual(version.Length, 4);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 11);
            Assert.AreEqual(version.Build, 2);
            Assert.AreEqual(version.Revision, 0);
        }

        [TestMethod]
        public void ValidFileVersionInfoConstructor_CustomLength()
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
            Assert.AreEqual(version.Length, 2);
            Assert.AreEqual(version.Major, 1);
            Assert.AreEqual(version.Minor, 11);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ErraticParamsIntArrayConstructor()
        {
            VersionNumber version = new VersionNumber(-1,0,5);  // must throw ArgumentException because of -1
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

        [TestMethod()]
        public void ToStringCustomLength()
        {
            VersionNumber version = new VersionNumber(1,7,1);

            Assert.AreEqual(version.ToString(2), "1.7", "Failed version.ToString(2)");
            Assert.AreEqual(version.ToString(5), "1.7.1.0.0", "Failed version.ToString(5)");
            Assert.AreEqual(version.ToString(0), "", "Failed version.ToString(0)");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IncrementDecrementOperatorsTest()
        {
            VersionNumber version1 = new VersionNumber(1,1,2);
            VersionNumber version2 = new VersionNumber(2,0,0);

            version1++;
            version2--;     // throws ArgumentException

            Assert.AreEqual(version1.ToString(), "1.1.3");
        }

        [TestMethod()]
        public void MajorVersionBump()
        {
            VersionNumber version = new VersionNumber(1, 1, 5, 6);

            version.Bump(1);

            Assert.AreEqual(version.ToString(), "1.2.0.0");
        }

        [TestMethod()]
        public void CompareToTest()
        {
            // Same length
            VersionNumber version1 = new VersionNumber(1,5,4);
            VersionNumber version2 = new VersionNumber(1,8,3);
            // Different length, same value
            VersionNumber version3 = new VersionNumber(1,3);
            VersionNumber version4 = new VersionNumber(1,3,0,0);
            // Different length, same common fields
            VersionNumber version5 = new VersionNumber(1,7,0,1);
            VersionNumber version6 = new VersionNumber(1,7);

            int sameLengthResult = version1.CompareTo(version2);
            int diffLengthSameValueResult = version3.CompareTo(version4);
            int diffLengthDiffValueResult = version5.CompareTo(version6);
            int equalsResult = version1.CompareTo(version1);

            Assert.AreEqual(-3, sameLengthResult);
            Assert.AreEqual(0, diffLengthSameValueResult);
            Assert.AreEqual(1, diffLengthDiffValueResult);
            Assert.AreEqual(0, equalsResult);
        }

        [TestMethod()]
        public void ComparisonOperatorsTest()
        {
            // Same length
            VersionNumber version1 = new VersionNumber(1, 5, 4);
            VersionNumber version2 = new VersionNumber(1, 8, 3);
            // Different length, same value
            VersionNumber version3 = new VersionNumber(1, 3);
            VersionNumber version4 = new VersionNumber(1, 3, 0, 0);
            // Different length, same common fields
            VersionNumber version5 = new VersionNumber(1, 7, 0, 1);
            VersionNumber version6 = new VersionNumber(1, 7);

            bool gtResult = version1 < version2;
            bool notEqualsResult = version3 != version4;
            bool leResult = version6 <= version5;
            bool geResult = version1 >= version1;

            Assert.IsTrue(gtResult);
            Assert.IsFalse(notEqualsResult);
            Assert.IsTrue(leResult);
            Assert.IsTrue(geResult);
        }
    }
}
