using Microsoft.VisualStudio.TestTools.UnitTesting;
using CemuUpdateTool;
using System.IO;
using System.Diagnostics;

namespace CemuUpdateTool.Tests
{
    [TestClass()]
    public class FileUtilsTests
    {
        [TestMethod()]
        public void ExtractZipFileContentsTest()
        {
            string testZip = @".\testFiles\test.zip";

            if (!FileUtils.FileExists(testZip))
                Assert.Inconclusive("Missing test archive");

            FileUtils.ExtractZipFileContents(testZip, null, (err) => { Debug.WriteLine(err); });
        }
    }
}
