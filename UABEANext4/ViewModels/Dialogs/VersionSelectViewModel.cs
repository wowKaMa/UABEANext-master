using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保持原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供 Action 委托等（保持原名 System）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 接口（保持原名 UABEANext4.Interfaces）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类

public partial class VersionSelectViewModel : ViewModelBase, IDialogAware<string?> // 定义部分类 VersionSelectViewModel，继承自 ViewModelBase 并实现 IDialogAware<string?> 接口（表示一个版本选择对话框的视图模型）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（会生成 Version 属性）
    public string _version = "0.0.0f0"; // 字段：Version 的后备字段，默认值为 "0.0.0f0"（Unity 版本号格式）

    public string Title => "版本选择 (Version Select)"; // 属性：对话框标题，中文显示“版本选择”，括号中保留英文原名（Title）

    public int Width => 300; // 属性：对话框宽度，固定为 300 像素（Width）

    public int Height => 140; // 属性：对话框高度，固定为 140 像素（Height）

    public event Action<string?>? RequestClose; // 事件：请求关闭对话框时触发（RequestClose），参数为 string? 表示返回的版本号或 null

    public void BtnOk_Click() // 方法：当用户点击“确定”按钮时调用（BtnOk_Click）
    {
        RequestClose?.Invoke(Version); // 触发 RequestClose 事件并传递当前 Version 属性的值（表示用户选择的版本号）
    }

    public void BtnCancel_Click() // 方法：当用户点击“取消”按钮时调用（BtnCancel_Click）
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传递 null（表示用户取消选择）
    }
} // 类定义结束：VersionSelectViewModel 的类型声明结束
