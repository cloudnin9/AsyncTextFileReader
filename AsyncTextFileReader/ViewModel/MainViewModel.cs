using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncTextFileReader.Annotations;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace AsyncTextFileReader.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ICommand _readFileCommand;
        private double _progressValue;
        private string _readText;
        private int _counter;
        private string _fileName;

        public MainViewModel()
        {
            ReadFileCommand = new RelayCommand(ReadFileHandler, () => true);
        }

        private void  ReadFileHandler()
        {
            ReadText = "Reading... " + (_counter++) + Environment.NewLine;

            var provider = new FileProcessingProvider();
            var processor = new CustomFileProcessor();
            processor.Subscribe(provider);

            var progress = new Progress<BufferedTextFileReaderArgs>();
            Debug.WriteLine("Add Progress Handler: " + Thread.CurrentThread.ManagedThreadId);
            progress.ProgressChanged += OnProgressChanged;

            ReadFileAsync(progress, provider).ContinueWith(antecedent =>
            {
                Debug.WriteLine("Continue With: " + Thread.CurrentThread.ManagedThreadId);
                DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        Debug.WriteLine("Remove Progress Handler: " + Thread.CurrentThread.ManagedThreadId);
                        progress.ProgressChanged -= OnProgressChanged;
                    });
            });
        }
        
        public ICommand ReadFileCommand
        {
            get { return _readFileCommand; }
            set
            {
                _readFileCommand = value;
                RaisePropertyChanged(() => ReadFileCommand);
            }
        }

        public Double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged(() => ProgressValue);
            }
        }

        public string ReadText
        {
            get { return _readText; }
            set
            {
                _readText = value;
                RaisePropertyChanged(() => ReadText);
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        public async Task ReadFileAsync(IProgress<BufferedTextFileReaderArgs> progress, FileProcessingProvider provider)
        {            
            using (var fr = new BufferedTextFileReader(provider, FileName??@"c:\Temp\trTask_InsertUpdate.sql"))
            {
                await fr.ReadLineAsync(progress);                
            }
        }

        void OnProgressChanged(object sender, BufferedTextFileReaderArgs e)
        {
            ProgressValue = e.ProgressPercentage;
            ReadText += e.Line + Environment.NewLine;
            ReadText += e.EndOfFile ? "The whole file is read" : "";
        }
    }
}