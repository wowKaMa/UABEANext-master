using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原始英文名称）

using System; // 引用基础系统命名空间，提供 Action 等类型（保留原始英文名称）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 List<T> 等集合类型（保留原始英文名称）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 接口（保留原始英文名称）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原始英文名称）

public partial class SelectDumpViewModel : ViewModelBase, IDialogAware<SelectedDumpType?> // 定义部分类 SelectDumpViewModel，继承自 ViewModelBase 并实现 IDialogAware<SelectedDumpType?>（对话框视图模型）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（将生成 SelectedItem 属性）
    public SelectedDumpType _selectedItem; // 字段：SelectedItem 的后备字段，表示当前下拉选择的枚举值（SelectedDumpType）

    public List<string> DropdownItems { get; } // 属性：下拉菜单项的字符串列表（DropdownItems），只读

    public string Title => "批量导入 (Batch Import)"; // 属性：对话框标题，中文显示“批量导入”，括号中保留英文原名（Title）

    public int Width => 300; // 属性：对话框宽度（像素）（Width）

    public int Height => 80; // 属性：对话框高度（像素）（Height）

    public event Action<SelectedDumpType?>? RequestClose; // 事件：请求关闭对话框时触发，参数为选中的枚举或 null（RequestClose）

    public SelectDumpViewModel(bool hideAnyOption) // 构造函数：初始化 SelectDumpViewModel，参数决定是否隐藏“任意 (Any)”选项
    {
        SelectedItem = SelectedDumpType.JsonDump; // 将默认选择项设为 JsonDump（SelectedDumpType.JsonDump）

        DropdownItems = new List<string> // 初始化下拉项列表（DropdownItems），每项为中文显示并在括号中保留原始英文名称
        {
            "UABEA JSON 转储 (UABEA json dump)", // 下拉项 1：UABEA JSON 转储（保留英文）
            "UABE 文本 转储 (UABE text dump)", // 下拉项 2：UABE 文本 转储（保留英文）
            "原始 转储 (Raw dump)", // 下拉项 3：原始 转储（保留英文）
            "任意 (Any)" // 下拉项 4：任意（保留英文）
        }; // 列表初始化结束

        if (hideAnyOption) // 如果调用者要求隐藏“任意 (Any)”选项
        {
            DropdownItems.RemoveAt(DropdownItems.Count - 1); // 从列表中移除最后一项（即“任意 (Any)”）
        }
    } // 构造函数结束

    public void BtnOk_Click() // 方法：当用户点击“确定”按钮时调用（BtnOk_Click）
    {
        RequestClose?.Invoke(SelectedItem); // 触发 RequestClose 事件并传回当前选中的枚举值（SelectedItem），通知对话框宿主关闭并返回结果
    }

    public void BtnCancel_Click() // 方法：当用户点击“取消”按钮时调用（BtnCancel_Click）
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传回 null，表示用户取消操作
    }
} // 类 SelectDumpViewModel 结束

public enum SelectedDumpType // 枚举：表示可选的导出/导入转储类型（SelectedDumpType）
{
    JsonDump, // 枚举值：JSON 转储（JsonDump）
    TxtDump, // 枚举值：文本转储（TxtDump）
    RawDump, // 枚举值：原始二进制转储（RawDump）
    Any // 枚举值：任意类型（Any）
}
