using System;
using System.Diagnostics;
using System.Threading;

namespace AsyncTextFileReader
{
    public abstract class FileProcessor : IObserver<BufferedTextFileReaderArgs>
    {
        private IDisposable _unsubscribe;

        public void Subscribe(IObservable<BufferedTextFileReaderArgs> provider)
        {
            _unsubscribe = provider.Subscribe(this);
        }

        public void Unsubscribe()
        {
            if (_unsubscribe != null)
            {
                _unsubscribe.Dispose();
                _unsubscribe = null;
            }
        }

        public virtual void OnNext(BufferedTextFileReaderArgs value)
        {
            throw new NotImplementedException();
        }

        public virtual void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnCompleted()
        {
            this.Unsubscribe();
        }

        protected void PrintResults(int lineNo, string line)
        {
            Debug.WriteLine(string.Format("{0}-{1} Thread ID: {2}", lineNo, line, Thread.CurrentThread.ManagedThreadId));
        }
    }
}