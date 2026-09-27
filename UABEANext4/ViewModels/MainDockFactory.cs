using CommunityToolkit.Mvvm.Input; // 引用命令支持 (RelayCommand)

using Dock.Avalonia.Controls; // 引用 Avalonia 停靠窗口控件

using Dock.Model.Controls; // 引用停靠接口

using Dock.Model.Core; // 引用核心停靠布局接口

using Dock.Model.Mvvm; // 引用 MVVM 工厂基类

using Dock.Model.Mvvm.Controls; // 引用工具与文档停靠控件

using System; // 引用基础系统空间

using System.Collections.Generic; // 引用字典与集合

using UABEANext4.AssetWorkspace; // 引用资产工作区

using UABEANext4.Logic.Documents; // 引用文档逻辑

using UABEANext4.ViewModels.Documents; // 引用文档视图模型

using UABEANext4.ViewModels.Tools; // 引用工具视图模型

namespace UABEANext4.ViewModels; // 定义命名空间

internal class MainDockFactory : Factory // 定义主停靠工厂类 (MainDockFactory)
{
    public ProportionalDock? MainPane; // 主面板 (MainPane)

    public DocumentManager DocMan; // 文档管理器 (DocMan)

    private IRootDock? _rootDock; // 根布局 (IRootDock)

    private IDocumentDock? _fileDocumentDock; // 文档停靠区 (_fileDocumentDock)

    private WorkspaceExplorerToolViewModel? _workspaceExplorerTool; // 资源管理器工具

    private InspectorToolViewModel? _inspectorToolViewModel; // 检查器工具

    private PreviewerToolViewModel? _previewerToolViewModel; // 预览器工具

    private HierarchyToolViewModel? _hierarchyToolViewModel; // 层级工具

    private UnityToolComponentsViewModel? _unityToolComponentsViewModel;
    private VrcaPreviewerToolViewModel? _vrcaPreviewerToolViewModel;

    // [新增] 声明 UnityToolComponents 工具的 ViewModel 私有字段

    private Workspace _workspace; // 工作区实例 (_workspace)

    public MainDockFactory(Workspace workspace) // 运行时构造函数
    {
        _workspace = workspace; // 注入外部工作区

        DocMan = new(); // 初始化文档管理器
    }

    public override IRootDock CreateLayout() // 核心方法：构建界面布局 (CreateLayout)
    {
        // 1. 初始化所有工具的视图模型 (ViewModel)

        _workspaceExplorerTool = new WorkspaceExplorerToolViewModel(_workspace);

        _inspectorToolViewModel = new InspectorToolViewModel(_workspace);

        _previewerToolViewModel = new PreviewerToolViewModel(_workspace);

        _hierarchyToolViewModel = new HierarchyToolViewModel(_workspace);

        _unityToolComponentsViewModel = new UnityToolComponentsViewModel(_workspace);
        _vrcaPreviewerToolViewModel = new VrcaPreviewerToolViewModel(_workspace);

        // [新增] 实例化您的组件汉化工具模型，并传入工作区数据

        // 2. 创建文档区域 (中间部分)

        var assetDocumentDock = new BlankDocumentViewModel();

        var documentDock = _fileDocumentDock = new DocumentDock
        {
            ActiveDockable = assetDocumentDock,

            VisibleDockables = CreateList<IDockable>(assetDocumentDock),

            CanCreateDocument = true,

            CreateDocument = new RelayCommand(AddNewBlankDocument)
        };

        // 3. Create Left Bottom Dock (Explorer & Hierarchy)
        var leftBottomDock = new ToolDock
        {
            ActiveDockable = _workspaceExplorerTool,
            VisibleDockables = CreateList<IDockable>(_workspaceExplorerTool, _hierarchyToolViewModel),
            Alignment = Alignment.Left,
            GripMode = GripMode.Visible,
            Proportion = 0.2
        };

        // 4. Create Right Bottom Dock (Previewer & Components)
        var rightBottomDock = new ToolDock
        {
            ActiveDockable = _previewerToolViewModel,
            VisibleDockables = CreateList<IDockable>(
                _previewerToolViewModel,
                _unityToolComponentsViewModel
            ),
            Alignment = Alignment.Right,
            GripMode = GripMode.Visible,
            Proportion = 0.3
        };

        // 5. Create Bottom Panel (Horizontal: Left, Middle(Doc), Right)
        var bottomPane = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>(
                leftBottomDock,
                new ProportionalDockSplitter(),
                documentDock,
                new ProportionalDockSplitter(),
                rightBottomDock
            ),
            Proportion = 0.4
        };

        // 6. Create Top Left Dock (VRCA Previewer)
        var topLeftDock = new ToolDock
        {
            ActiveDockable = _vrcaPreviewerToolViewModel,
            VisibleDockables = CreateList<IDockable>(_vrcaPreviewerToolViewModel),
            Alignment = Alignment.Left,
            Proportion = 0.75
        };

        // 7. Create Top Right Dock (Inspector)
        var topRightDock = new ToolDock
        {
            ActiveDockable = _inspectorToolViewModel,
            VisibleDockables = CreateList<IDockable>(_inspectorToolViewModel),
            Alignment = Alignment.Right,
            GripMode = GripMode.Visible,
            Proportion = 0.25
        };

        // 8. Create Top Panel (Horizontal: VRCA Previewer, Previewer)
        var topPane = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>(
                topLeftDock,
                new ProportionalDockSplitter(),
                topRightDock
            ),
            Proportion = 0.6
        };

        // 9. Combine into Main Pane (Vertical Split)
        MainPane = new ProportionalDock
        {
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>(
                topPane,
                new ProportionalDockSplitter(),
                bottomPane
            )
        };

        // 10. Configure Root Layout
        var windowLayout = CreateRootDock();
        windowLayout.VisibleDockables = CreateList<IDockable>(MainPane);
        windowLayout.ActiveDockable = MainPane;

        _rootDock = CreateRootDock();
        _rootDock.VisibleDockables = CreateList<IDockable>(windowLayout);
        _rootDock.ActiveDockable = windowLayout;

        return _rootDock;
    }

    public override void InitLayout(IDockable layout) // 初始化停靠定位器 (InitLayout)
    {
        DockableLocator = new Dictionary<string, Func<IDockable?>>
        {
            ["Root"] = () => _rootDock,

            ["WorkspaceExplorer"] = () => _workspaceExplorerTool,

            ["Hierarchy"] = () => _hierarchyToolViewModel,

            ["Files"] = () => _fileDocumentDock,

            ["Inspector"] = () => _inspectorToolViewModel,

            ["UnityToolComponents"] = () => _unityToolComponentsViewModel,
            ["VrcaPreviewer"] = () => _vrcaPreviewerToolViewModel,

            ["Previewer"] = () => _previewerToolViewModel,

        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }

    private void AddNewBlankDocument() // 添加空白文档方法
    {
        if (_fileDocumentDock is not null)
        {
            var newDoc = new BlankDocumentViewModel();

            AddDockable(_fileDocumentDock, newDoc);

            SetActiveDockable(newDoc);
        }
    }
}