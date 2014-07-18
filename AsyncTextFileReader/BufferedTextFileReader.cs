using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTextFileReader
{
    public class BufferedTextFileReader : IDisposable
    {
        private bool _disposed = false;
        private readonly FileProcessingProvider _processor = null;
        public StreamReader sr { get; set; }

        public BufferedTextFileReader(string path)
        {
            var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bs = new BufferedStream(fs);
            sr = new StreamReader(bs);
        }

        public BufferedTextFileReader(FileProcessingProvider processor, string path)
        {
            var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bs = new BufferedStream(fs);
            sr = new StreamReader(bs);
            this._processor = processor;
        }

        public async Task ReadLineAsync(IProgress<BufferedTextFileReaderArgs> progress = null)
        {
            Debug.WriteLine("Begin ReadFileAsync: " + Thread.CurrentThread.ManagedThreadId);
            var line = "";
            var count = 0;
            var sb = new StringBuilder();
            while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                Debug.WriteLine("After Await ReadFileAsync: " + Thread.CurrentThread.ManagedThreadId);
                var l = line;
                sb.AppendLine(l);
                var lineNo = count++;
                if (_processor != null)
                {
                    _processor.ProcessLine(new BufferedTextFileReaderArgs(l, lineNo, false, 0));
                }
                if (progress != null && sb.Length > 0)
                {
                    progress.Report(new BufferedTextFileReaderArgs(sb.ToString(), lineNo, false, lineNo));
                    sb.Clear();
                }
            }
            if (_processor != null)
            {
                _processor.EndTransmission();
            }
            if (progress != null)
            {
                progress.Report(new BufferedTextFileReaderArgs(sb.ToString(), -1, true, 100));
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
            }
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            try
            {
                sr.Close();
                sr.Dispose();
                sr = null;
            }
            catch (Exception)
            {
            }
        }
    }
}