using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.Interfaces;
using UABEANext4.Logic.Hierarchy;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class TransferReportViewModel : ObservableObject, IDialogAware
    {
        public string Title => "转移报告 (Transfer Report)";
        public int Width => 700;
        public int Height => 550;

        private readonly List<TransferRecord> _allRecords;

        [ObservableProperty]
        private ObservableCollection<TransferGroup> _groups = new();

        [ObservableProperty]
        private string _searchText = "";

        public TransferReportViewModel(IEnumerable<TransferRecord> records)
        {
            _allRecords = records.ToList();
            UpdateGroups();
        }

        partial void OnSearchTextChanged(string value)
        {
            UpdateGroups();
        }

        private void UpdateGroups()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allRecords
                : _allRecords.Where(r => 
                    r.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.Type.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    r.SourcePathId.ToString().Contains(SearchText) ||
                    r.TargetPathId.ToString().Contains(SearchText)
                ).ToList();

            var newGroups = filtered
                .GroupBy(r => r.Type)
                .Select(g => new TransferGroup(g.Key, g.ToList()))
                .OrderBy(g => g.Type)
                .ToList();

            Groups = new ObservableCollection<TransferGroup>(newGroups);
        }

        [RelayCommand]
        public void Close()
        {
            // IDialogAware doesn't strictly require a Close command, 
            // but we can trigger it if we implement IDialogAware<object>
        }
    }

    public class TransferGroup
    {
        public string Type { get; }
        public List<TransferRecord> Records { get; }
        public string DisplayHeader => $"{Type} ({Records.Count})";

        public TransferGroup(string type, List<TransferRecord> records)
        {
            Type = type;
            Records = records;
        }
    }
}
