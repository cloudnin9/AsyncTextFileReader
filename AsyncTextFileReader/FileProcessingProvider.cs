using System;
using System.Collections.Generic;
using System.Linq;

namespace AsyncTextFileReader
{
    public class FileProcessingProvider : IObservable<BufferedTextFileReaderArgs>
    {
        private readonly IList<IObserver<BufferedTextFileReaderArgs>> _observers;

        public FileProcessingProvider()
        {
            this._observers = new List<IObserver<BufferedTextFileReaderArgs>>();
        }

        public IDisposable Subscribe(IObserver<BufferedTextFileReaderArgs> observer)
        {
            //maintain a list of observers to notify
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            //a mechanism for the observer to be removed from the Observerable
            return new Unsubscriber(_observers, observer);
        }

        internal class Unsubscriber : IDisposable
        {
            private readonly IList<IObserver<BufferedTextFileReaderArgs>> _observers;
            private readonly IObserver<BufferedTextFileReaderArgs> _observer;

            public Unsubscriber(IList<IObserver<BufferedTextFileReaderArgs>> observers,
                IObserver<BufferedTextFileReaderArgs> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void ProcessLine(BufferedTextFileReaderArgs args)
        {
            _observers.AsParallel().ForAll(ob => ob.OnNext(args));
        }

        public void ProcessLine(string line, int lineNo, bool eof, double percentage = 0)
        {
            var args = new BufferedTextFileReaderArgs(line, lineNo, eof, percentage);
            _observers.AsParallel().ForAll(ob => ob.OnNext(args));
        }

        public void EndTransmission()
        {
            var arr = _observers.ToArray();//makes a copy of the collection
            foreach (var observer in arr)
            {
                if (_observers.Contains(observer))
                {
                    observer.OnCompleted();
                }
            }

            _observers.Clear();
        }
    }
}
