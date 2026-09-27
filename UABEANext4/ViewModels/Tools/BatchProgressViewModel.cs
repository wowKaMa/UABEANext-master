using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace UABEANext4.ViewModels.Tools
{
    public partial class BatchFileItemViewModel : ObservableObject
    {
        [ObservableProperty] private string _fileName = string.Empty;
        [ObservableProperty] private double _progress;
        [ObservableProperty] private string _status = "等待中 (Pending)";
        [ObservableProperty] private string _message = string.Empty;
        [ObservableProperty] private string _memoryInfo = "-";
        
        public string FullPath { get; set; } = string.Empty;
    }

    public partial class BatchProgressViewModel : ViewModelBase
    {
        [ObservableProperty] private double _totalProgress;
        [ObservableProperty] private string _totalProgressText = string.Empty;
        [ObservableProperty] private string _statusText = "准备就绪 (Ready)";
        
        [ObservableProperty] 
        private ObservableCollection<BatchFileItemViewModel> _items = new ObservableCollection<BatchFileItemViewModel>();
        
        public CancellationTokenSource Cts { get; } = new CancellationTokenSource();
        public bool IsCancelled => Cts.IsCancellationRequested;

        public BatchProgressViewModel()
        {
        }

        [RelayCommand]
        public void Cancel()
        {
            Cts.Cancel();
            StatusText = "正在取消 (Cancelling)...";
        }

        public void AddLog(string log) 
        {
            // Legacy/Global log if needed, or remove
        }
    }
}
