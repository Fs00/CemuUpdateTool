using System.IO;
using System.Linq;
using CemuUpdateTool.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CemuUpdateTool.Tests
{
    [TestClass]
    public class FileUtilsTests
    {
        [ClassCleanup]
        public static void RemoveExtractedFiles()
        {
            Directory.Delete(@".\ZipExtractTest", recursive: true);
        }

        [DeploymentItem(@"Resources\ZipExtractTest\", @"ZipExtractTest\")]
        [TestMethod]
        public void ExtractZipFileContentsTest()
        {
            // The archive contains 1 directory and 2 files
            string testZip = @".\ZipExtractTest\testArchive.zip";

            FileUtils.ExtractZipArchiveInSameDirectory(testZip);

            string currentDirectory = Path.GetDirectoryName(testZip);
            Assert.AreEqual(1, Directory.EnumerateDirectories(currentDirectory).Count());
            Assert.AreEqual(3, Directory.EnumerateFiles(currentDirectory).Count());    // 2 extracted files + zip
        }

        [DeploymentItem(@"Resources\DirectoryContentsTest\", @"DirectoryContentsTest\")]
        [TestMethod]
        public void RemoveDirContentsTest()
        {
            string testDir = @".\DirectoryContentsTest";

            FileUtils.RemoveDirectoryContents(testDir);

            Assert.AreEqual(0, new DirectoryInfo(testDir).GetDirectories().Length);
        }
    }
}
