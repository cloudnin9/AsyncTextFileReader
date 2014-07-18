using System;

namespace AsyncTextFileReader
{
    public class BufferedTextFileReaderArgs : EventArgs
    {
        public string Line { get; private set; }

        public int LineNo { get; private set; }

        public bool EndOfFile { get; private set; }
        public double ProgressPercentage { get; set; }

        public BufferedTextFileReaderArgs(string line, int lineNo, bool endOfFile, double percentage)
        {
            EndOfFile = endOfFile;
            Line = line;
            LineNo = lineNo;
            ProgressPercentage = percentage;
        }
    }
}