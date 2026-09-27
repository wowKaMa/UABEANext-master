using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型命名空间，提供 ObservableProperty 等 MVVM 特性（保留原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供 Action 等委托类型（保留原名 System）

using UABEANext4.Interfaces; // 引用项目内的接口命名空间，包含 IDialogAware 接口（保留原名 UABEANext4.Interfaces）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原名）

public partial class RenameFileViewModel : ViewModelBase, IDialogAware<string?> // 定义部分类 RenameFileViewModel，继承自 ViewModelBase 并实现 IDialogAware<string?>（对话框会返回 string? 类型结果）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与属性变更通知（会生成公开属性 NewName 并在设置时触发通知）
    public string _newName; // 字段：NewName 的后备字段，保存用户输入的新文件名（生成的属性名为 NewName，保留英文原名）

    public string Title => "重命名文件 (Rename File)"; // 属性：对话框标题，显示为中文并在括号中保留英文原名（Title）

    public int Width => 350; // 属性：对话框宽度（像素），用于 UI 布局（Width）

    public int Height => 80; // 属性：对话框高度（像素），用于 UI 布局（Height）

    public event Action<string?>? RequestClose; // 事件：请求关闭对话框时触发，参数为 string?（返回的新名称或 null 表示取消），外部订阅以接收结果（RequestClose）

    public RenameFileViewModel(string originalName) // 构造函数：接收原始文件名并初始化视图模型
    {
        NewName = originalName; // 将传入的 originalName 赋值给生成的属性 NewName，作为对话框初始输入值
    }

    public void BtnOk_Click() // 方法：当用户点击“确定”按钮时调用（BtnOk_Click）
    {
        RequestClose?.Invoke(NewName); // 触发 RequestClose 事件并传回当前 NewName（表示用户确认并返回新名称）
    }

    public void BtnCancel_Click() // 方法：当用户点击“取消”按钮时调用（BtnCancel_Click）
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传回 null（表示用户取消操作）
    }
} // 类 RenameFileViewModel 结束
