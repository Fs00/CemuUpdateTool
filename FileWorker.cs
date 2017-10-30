using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace CemuUpdateTool
{
    public enum WorkOutcome
    {
        Success,
        Aborted,
        CancelledByUser,
        CompletedWithErrors
    }

    public class FileWorker
    {
        public string baseSourcePath { private set; get; }                  // older Cemu folder
        public string baseDestinationPath { private set; get; }             // new Cemu folder

        public bool workIsCancelled { set; get; } = false;
        public bool workAborted { set; get; } = false;
        public bool errorsEncountered { set; get; } = false;

        public byte currentFolderIndex { set; get; } = 0;                   // index of the currently copying folder
        public List<long> foldersSizes { set; get; }                         // contains the sizes (in bytes) of the folders to copy
        public List<FileInfo> filesAlreadyCopied { set; get; }               // list of files that have already been copied, necessary if you want to restore the original situation when you cancel the operation
        public List<DirectoryInfo> directoriesAlreadyCopied { set; get; }    // list of directories that have already been copied, necessary if you want to restore the original situation when you cancel the operation

        public FileWorker(string usrInputSrcPath, string usrInputDestPath)
        {
            baseSourcePath = usrInputSrcPath;
            baseDestinationPath = usrInputDestPath;

            filesAlreadyCopied = new List<FileInfo>();
            directoriesAlreadyCopied = new List<DirectoryInfo>();
        }

        public void PerformOperations(List<string> foldersToCopy, FolderInfoCallback PerformingWork, ActualFileCallback CopyingFile, FileCopiedCallback FileCopied, CompletionCallback WorkCompleted)
        {
            foreach (string folder in foldersToCopy)
            {
                if (foldersSizes[currentFolderIndex] > 0)       // avoiding to copy empty/unexisting folders
                {
                    PerformingWork(folder, foldersSizes[currentFolderIndex]);      // tell the main form which folder I'm about to copy
                    FileOperations.CopyDir(folder, CopyingFile, FileCopied, this);
                }
                currentFolderIndex++;

                if (workIsCancelled || workAborted)
                {
                    try
                    {
                        if (filesAlreadyCopied.Count > 0 || directoriesAlreadyCopied.Count > 0)
                        {
                            // Ask if the user wants to remove files that have already been copied and, if the user accepts, performs the task. Then exit from the function.
                            DialogResult userSelection = MessageBox.Show("Do you want to delete files that have already been copied?", "Operation cancelled", MessageBoxButtons.YesNo);
                            if (userSelection == DialogResult.Yes)
                            {
                                foreach (FileInfo copiedFile in filesAlreadyCopied)
                                    copiedFile.Delete();
                                foreach (DirectoryInfo copiedDir in Enumerable.Reverse(directoriesAlreadyCopied))
                                    copiedDir.Delete();
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("An error occurred when deleting already copied files: " + exc.Message, "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (workIsCancelled)
                            WorkCompleted(WorkOutcome.CancelledByUser);
                        else
                            WorkCompleted(WorkOutcome.Aborted);
                    }
                    return;
                }
            }
            // If the program arrives here, it means that the copy process has been completed
            if (!errorsEncountered)
                WorkCompleted(WorkOutcome.Success);
            else
                WorkCompleted(WorkOutcome.CompletedWithErrors);
        }

        public void CancelWork()
        {
            workIsCancelled = true;
        }
    }
}
