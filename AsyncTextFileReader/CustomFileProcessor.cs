using System;
using System.Diagnostics;

namespace AsyncTextFileReader
{
    public class CustomFileProcessor : FileProcessor
    {
        private string Id;
        public CustomFileProcessor()
        {
            Id = Guid.NewGuid().ToString();
        }
        public override void OnError(Exception error)
        {
            //log to file or somewhere appropriate

        }

        public override void OnNext(BufferedTextFileReaderArgs value)
        {
            //processing code
            PrintResults(value.LineNo, value.Line + "-----" + Id);
        }

        public override void OnCompleted()
        {
            //clean up code here
            CleanUp();
            base.OnCompleted();
        }

        private void CleanUp()
        {
            Debug.WriteLine("clean up on Processor " + Id);
        }
    }
}