using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET.Extra 库，提供处理 Unity 资产文件的扩展功能（保留原始英文名称 AssetsTools.NET.Extra）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原始英文名称 CommunityToolkit.Mvvm.ComponentModel）

using System;
// 引用基础系统命名空间，提供基本类型与工具（保留原始英文名称 System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T> 等集合类型（保留原始英文名称 System.Collections.Generic）

using System.Linq;
// 引用 LINQ 扩展方法，用于集合查询与转换（保留原始英文名称 System.Linq）

using UABEANext4.AssetWorkspace;
// 引用项目的资产工作区命名空间，包含 Workspace、WorkspaceItem、AssetsFileInstance 等（保留原始英文名称 UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces;
// 引用项目接口命名空间，包含 IDialogAware 等接口（保留原始英文名称 UABEANext4.Interfaces）

using UABEANext4.Logic.AssetInfo;
// 引用资产信息相关逻辑命名空间，包含 GeneralInfo、TypeTreeInfo、ExternalInfo、ScriptInfo 等（保留原始英文名称 UABEANext4.Logic.AssetInfo）

namespace UABEANext4.ViewModels.Dialogs;
// 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原始英文名称）

public partial class AssetInfoViewModel : ViewModelBase, IDialogAware
// 定义部分类 AssetInfoViewModel，继承自 ViewModelBase 并实现 IDialogAware（表示一个显示资产信息的对话框视图模型）
{
    public List<WorkspaceItem> Items { get; }
    // 公共只读属性 Items：保存传入或用于显示的 WorkspaceItem 列表（用于 UI 下拉或选择）

    private WorkspaceItem? _selectedItem;
    // 私有字段 _selectedItem：保存当前选中的 WorkspaceItem（可为空）

    public WorkspaceItem? SelectedItem
    // 公共属性 SelectedItem：对外暴露选中项，设置时会触发相关信息面板的更新
    {
        get => _selectedItem;
        // getter：返回当前私有字段 _selectedItem 的值

        set
        // setter：当外部设置 SelectedItem 时执行以下逻辑以更新显示内容
        {
            _selectedItem = value;
            // 将传入的值保存到私有字段 _selectedItem

            if (_selectedItem is not { Object: AssetsFileInstance inst })
            // 判断选中项是否存在且其 Object 字段是否为 AssetsFileInstance；若不是则清空显示信息
            {
                GeneralInfo = GeneralInfo.Empty;
                // 将 GeneralInfo 设为空占位（表示没有可显示的一般信息）

                TypeTreeInfo = TypeTreeInfo.Empty;
                // 将 TypeTreeInfo 设为空占位（表示没有类型树信息）

                ExternalsInfo = ExternalInfo.Empty;
                // 将 ExternalsInfo 设为空占位（表示没有外部引用信息）

                ScriptsInfo = ScriptInfo.Empty;
                // 将 ScriptsInfo 设为空占位（表示没有脚本信息）

                return;
                // 结束 setter，因选中项无效无需继续处理
            }

            GeneralInfo = new GeneralInfo(inst);
            // 如果选中项有效，使用该文件实例 inst 构造 GeneralInfo 并赋值（显示一般信息）

            TypeTreeInfo = new TypeTreeInfo(_workspace, inst);
            // 使用工作区与文件实例构造 TypeTreeInfo（显示类型树信息）

            ExternalsInfo = new ExternalInfo(_workspace, inst);
            // 使用工作区与文件实例构造 ExternalInfo（显示外部引用信息）

            ScriptsInfo = new ScriptInfo(_workspace, inst);
            // 使用工作区与文件实例构造 ScriptInfo（显示脚本相关信息）
        }
    }

    [ObservableProperty]
    private GeneralInfo? _generalInfo;
    // 使用 ObservableProperty 特性生成公开属性 GeneralInfo 的后备字段 _generalInfo（用于绑定并在改变时通知 UI）

    [ObservableProperty]
    private TypeTreeInfo? _typeTreeInfo;
    // 使用 ObservableProperty 特性生成公开属性 TypeTreeInfo 的后备字段 _typeTreeInfo（用于绑定并在改变时通知 UI）

    [ObservableProperty]
    private ExternalInfo? _externalsInfo;
    // 使用 ObservableProperty 特性生成公开属性 ExternalsInfo 的后备字段 _externalsInfo（用于绑定并在改变时通知 UI）

    [ObservableProperty]
    private ScriptInfo? _scriptsInfo;
    // 使用 ObservableProperty 特性生成公开属性 ScriptsInfo 的后备字段 _scriptsInfo（用于绑定并在改变时通知 UI）

    private readonly Workspace _workspace;
    // 私有只读字段 _workspace：保存传入的 Workspace 实例，用于在构造信息对象时访问管理器与文件查找

    public string Title => "资产信息 (Asset Info)";
    // 对话框标题（Title）：显示为中文“资产信息”，括号内保留英文原名 (Asset Info) 以便用户识别

    public int Width => 850;
    // 对话框宽度（像素），用于 UI 布局

    public int Height => 500;
    // 对话框高度（像素），用于 UI 布局

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)]
    public AssetInfoViewModel()
    // 无参构造函数（仅供设计器使用），标记为 Obsolete 以避免在运行时误用
    {
        Items = new List<WorkspaceItem>();
        // 为设计器初始化 Items 为一个空列表，避免 null 引用

        _workspace = new Workspace();
        // 为设计器创建一个新的 Workspace 占位（仅用于设计器预览）
    }

    public AssetInfoViewModel(Workspace workspace, IEnumerable<WorkspaceItem> items)
    // 运行时构造函数：接收 Workspace 与要显示的 WorkspaceItem 集合
    {
        Items = items.ToList();
        // 将传入的 items 转为列表并赋值给 Items 属性，便于 UI 绑定与索引访问

        _workspace = workspace;
        // 保存传入的 Workspace 实例到私有字段 _workspace，供后续信息构造使用
    }
}
// 类 AssetInfoViewModel 定义结束
