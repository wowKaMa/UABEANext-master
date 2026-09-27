using CommunityToolkit.Mvvm.ComponentModel;

namespace UABEANext4.AssetWorkspace;

public partial class ResourceEntry : ObservableObject
{
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _type = "";
    [ObservableProperty] private long _size;
    [ObservableProperty] private long _offset;
    [ObservableProperty] private long _pathId;
    [ObservableProperty] private string _fileName = "";

    public string DisplaySize => Size >= 1048576 
        ? $"{(double)Size / 1048576:F2} MB" 
        : $"{(double)Size / 1024:F2} KB";
}
