using Microsoft.VisualStudio.TestTools.UnitTesting;
using CemuUpdateTool;
using System.IO;

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

            Worker wk = new Worker(".", ".", null);
            FileUtils.ExtractZipFileContents(testZip, wk);
        }
    }
}
