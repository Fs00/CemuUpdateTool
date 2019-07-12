using System.IO;

namespace CemuUpdateTool.Settings
{
    /*
     *  Custom StreamReader that holds the current number of read lines when using ReadLine()
     */
    class LineCountingStreamReader : StreamReader
    {
        public LineCountingStreamReader(string path) : base(path) {}

        public int CurrentLine { private set; get; }

        public override string ReadLine()
        {
            string result = base.ReadLine();
            if (result != null)
                CurrentLine++;
            return result;
        }
    }
}
