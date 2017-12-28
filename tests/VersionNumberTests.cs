using System;
using CemuUpdateTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.tests
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
            Console.WriteLine(version.Build);   // should cause InvalidOperationException
        }
    }
}
