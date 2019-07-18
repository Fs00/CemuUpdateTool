using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using CemuUpdateTool.Utils;

namespace CemuUpdateTool.Tests
{
    [TestClass()]
    public class FileUtilsTests
    {
        [TestMethod()]
        public void ExtractZipFileContentsTest()
        {
            string testZip = @".\testFiles\test.zip";

            if (!File.Exists(testZip))
                Assert.Inconclusive("Missing test archive");

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
        }

        [TestMethod()]
        public void RemoveDirContentsTest()
        {
            string testDir = @".\testFiles\testDir\";

            if (!Directory.Exists(testDir))
                Assert.Inconclusive("Missing test directory");

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
            Assert.AreEqual(new DirectoryInfo(testDir).GetDirectories().Length, 0);
        }
    }
}
