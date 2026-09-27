using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保持原名 AssetsTools.NET）

using AvaloniaEdit.Document; // 引用 AvaloniaEdit 的文档类型，用于文本编辑器功能（保持原名 AvaloniaEdit.Document）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保持原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供基本类型与工具（保持原名 System）

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定（保持原名 System.Collections.ObjectModel）

using System.IO; // 引用 IO 操作命名空间，用于文件读写（保持原名 System.IO）

using System.Reflection; // 引用反射命名空间，用于获取类型的属性信息（保持原名 System.Reflection）

using System.Text; // 引用文本编码与处理命名空间（Encoding 等）（保持原名 System.Text）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（保持原名 System.Threading.Tasks）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 接口（保持原名 UABEANext4.Interfaces）

using UABEANext4.Logic.Configuration; // 引用配置相关逻辑命名空间（ConfigurationValues、ConfigurationItemBase 等）（保持原名 UABEANext4.Logic.Configuration）

using UABEANext4.Logic.ImportExport; // 引用导入导出逻辑命名空间（保持原名 UABEANext4.Logic.ImportExport）

using UABEANext4.Util; // 引用工具类命名空间（保持原名 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类

public partial class SettingsViewModel : ViewModelBase, IDialogAware // 定义部分类 SettingsViewModel，继承自 ViewModelBase 并实现 IDialogAware 接口（表示“设置”对话框的视图模型）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（会生成 ConfigItems 属性）
    private ObservableCollection<ConfigurationItemBase> _configItems = []; // 字段：ConfigItems 的后备字段，类型为 ObservableCollection<ConfigurationItemBase>，用于保存配置项集合

    public string Title => "设置 (Settings)"; // 属性：对话框标题，中文显示“设置”，括号中保留英文原名（Title）

    public int Width => 350; // 属性：对话框宽度，固定为 350 像素（Width）

    public int Height => 550; // 属性：对话框高度，固定为 550 像素（Height）

    public SettingsViewModel() // 构造函数：初始化 SettingsViewModel 实例
    {
        var properties = typeof(ConfigurationValues) // 使用反射获取 ConfigurationValues 类型的所有属性（properties）
            .GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取所有公共实例属性（BindingFlags.Public | BindingFlags.Instance）

        foreach (var property in properties) // 遍历每个属性（property）
        {
            var propType = property.PropertyType; // 获取属性的类型（propType）

            if (propType == typeof(bool)) // 如果属性类型是 bool
            {
                ConfigItems.Add(new ConfigurationBooleanItem(property)); // 创建 ConfigurationBooleanItem 并添加到 ConfigItems 集合
            }
            else if (propType == typeof(int)) // 如果属性类型是 int
            {
                ConfigItems.Add(new ConfigurationIntegerItem(property)); // 创建 ConfigurationIntegerItem 并添加到 ConfigItems 集合
            }
            else if (propType.IsEnum) // 如果属性类型是枚举（IsEnum）
            {
                ConfigItems.Add(new ConfigurationEnumItem(property)); // 创建 ConfigurationEnumItem 并添加到 ConfigItems 集合
            }
        }
    } // 构造函数结束
} // 类定义结束：SettingsViewModel 的类型声明结束
