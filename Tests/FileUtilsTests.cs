using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Diagnostics;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Tests
{
    [TestClass]
    public class FileUtilsTests
    {
        [ClassCleanup]
        public static void RemoveExtractedFiles()
        {
            Directory.Delete(@".\Resources\ZipExtractTest", recursive: true);
        }

        #region These tests work only if executed singularly (DeploymentItem seems not to work well with folders)
        [DeploymentItem(@"Resources\ZipExtractTest\testArchive.zip")]
        [TestMethod()]
        public void ExtractZipFileContentsTest()
        {
            // The archive contains 1 directory and 2 files
            string testZip = @".\Resources\ZipExtractTest\testArchive.zip";

            FileUtils.ExtractZipArchiveInSameDirectory(testZip, (msg, type, newLine) =>
            {
                switch (type)
                {
                    case EventLogEntryType.Error:
                    case EventLogEntryType.FailureAudit:
                        Debug.Write("ERR: ");
                        break;
                    case EventLogEntryType.Warning:
                        Debug.Write("WARN: ");
                        break;
                }
                Debug.WriteLine(msg);
            });

            string currentDirectory = Path.GetDirectoryName(testZip);
            Assert.AreEqual(1, Directory.EnumerateDirectories(currentDirectory).Count());
            Assert.AreEqual(3, Directory.EnumerateFiles(currentDirectory).Count());    // 2 extracted files + zip
        }

        [DeploymentItem(@"Resources\DirectoryContentsTest\")]
        [TestMethod()]
        public void RemoveDirContentsTest()
        {
            string testDir = @".\Resources\DirectoryContentsTest";

            FileUtils.RemoveDirectoryContents(testDir, (msg, type, newLine) =>
            {
                switch (type)
                {
                    case EventLogEntryType.Error:
                    case EventLogEntryType.FailureAudit:
                        Debug.Write("ERR: ");
                        break;
                    case EventLogEntryType.Warning:
                        Debug.Write("WARN: ");
                        break;
                }
                Debug.WriteLine(msg);
            });

            Assert.AreEqual(0, new DirectoryInfo(testDir).GetDirectories().Length);
        }
        #endregion
    }
}
