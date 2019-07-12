using System;

namespace CemuUpdateTool.Settings
{
    /*
     *  Custom exception for parsing errors that contains line number at which the exception has been thrown 
     */
    public class OptionsParsingException : Exception
    {
        public int CurrentLine { get; }

        public OptionsParsingException(string message, int lineCount) : base(message)
        {
            CurrentLine = lineCount;
        }

        protected OptionsParsingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
}
