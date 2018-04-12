﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            FileUtils.ExtractZipFileContents(testZip, (msg, type, newLine) =>
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

            if (!FileUtils.DirectoryExists(testDir))
                Assert.Inconclusive("Missing test directory");

            FileUtils.RemoveDirContents(testDir, (msg, type, newLine) =>
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