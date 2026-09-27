using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UABEANext4.AssetWorkspace;

public class WorkspaceItem : INotifyPropertyChanged
{
    public string OriginalName { get; set; }
    public string Name { get; set; }
    public WorkspaceItem? Parent { get; set; }
    public List<WorkspaceItem> Children { get; set; }
    public object? Object { get; set; }
    public WorkspaceItemType ObjectType { get; }
    public int LoadIndex { get; }

    public bool Loaded => Object != null;

    public WorkspaceItem(AssetsFileInstance fileInst, int loadOrder)
    {
        Name = fileInst.name;
        OriginalName = Name;
        Parent = null;
        Children = new List<WorkspaceItem>(0);
        Object = fileInst;
        ObjectType = WorkspaceItemType.AssetsFile;
        LoadIndex = loadOrder;
    }

    public WorkspaceItem(Workspace workspace, BundleFileInstance bunInst, int loadOrder)
    {
        Name = bunInst.name;
        OriginalName = Name;
        int fileCount = bunInst.file.BlockAndDirInfo.DirectoryInfos.Count;
        Parent = null;
        Children = new List<WorkspaceItem>(fileCount);
        Object = bunInst;
        ObjectType = WorkspaceItemType.BundleFile;
        LoadIndex = loadOrder;

        // 并行加载 bundle 内部的子文件，用多核加速单个大文件的解析
        // Manager.LoadAssetsFileFromBundle 需要同步（共享 Manager 状态），但 FixupAssetsFile 可以并行
        var childItems = new WorkspaceItem[fileCount];
        var managerLock = new object();

        System.Threading.Tasks.Parallel.For(0, fileCount, i =>
        {
            AssetBundleDirectoryInfo dirInf = BundleHelper.GetDirInfo(bunInst.file, i);
            WorkspaceItemType type = ((dirInf.Flags & 0x04) != 0)
                ? WorkspaceItemType.AssetsFile
                : WorkspaceItemType.ResourceFile;

            WorkspaceItem child;
            if (type == WorkspaceItemType.AssetsFile)
            {
                // Manager 操作需要锁保护（共享内部文件列表）
                AssetsFileInstance fileInst;
                lock (managerLock)
                {
                    fileInst = workspace.Manager.LoadAssetsFileFromBundle(bunInst, i);
                    workspace.TryLoadClassDatabase(fileInst.file);
                }
                // FixupAssetsFile 是纯 CPU 计算，可以在锁外并行执行
                workspace.FixupAssetsFile(fileInst);
                child = new WorkspaceItem(dirInf.Name, fileInst, -1, WorkspaceItemType.AssetsFile);
            }
            else
            {
                child = new WorkspaceItem(dirInf.Name, dirInf, loadOrder, type);
            }

            childItems[i] = child;
        });

        // 按顺序添加子项到 UI（保持原始顺序）
        for (int i = 0; i < fileCount; i++)
        {
            if (childItems[i] != null)
            {
                var dirInf = BundleHelper.GetDirInfo(bunInst.file, i);
                workspace.AddChildItemThreadSafe(childItems[i], this, dirInf.Name);
            }
        }
    }

    public WorkspaceItem(string name, object? obj, int loadOrder, WorkspaceItemType type = WorkspaceItemType.OtherFile)
    {
        Name = name;
        OriginalName = Name;
        Parent = null;
        Children = new List<WorkspaceItem>(0);
        Object = obj;
        ObjectType = type;
        LoadIndex = loadOrder;
    }

    public static IEnumerable<WorkspaceItem> GetAssetsFileWorkspaceItems(IEnumerable<WorkspaceItem> workspaceItems)
    {
        foreach (var item in workspaceItems)
        {
            if (item.ObjectType == WorkspaceItemType.AssetsFile)
            {
                yield return item;
            }

            if (item.ObjectType == WorkspaceItemType.BundleFile)
            {
                foreach (var assetFileChild in item.Children.Where(x => x.ObjectType == WorkspaceItemType.AssetsFile))
                {
                    yield return assetFileChild;
                }
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Update(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
