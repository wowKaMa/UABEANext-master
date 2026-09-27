using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保留原始英文名称 AssetsTools.NET）

using AvaloniaEdit.Document; // 引用 AvaloniaEdit 的文档类型，用于文本编辑器（TextDocument）（保留原始英文名称 AvaloniaEdit.Document）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原始英文名称 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供基本类型与委托（保留原始英文名称 System）

using System.IO; // 引用 IO 操作命名空间，用于内存流和文件流（保留原始英文名称 System.IO）

using System.Text; // 引用文本编码与处理命名空间（Encoding 等）（保留原始英文名称 System.Text）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（保留原始英文名称 System.Threading.Tasks）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 接口（保留原始英文名称 UABEANext4.Interfaces）

using UABEANext4.Logic.ImportExport; // 引用导入导出逻辑命名空间（AssetImport、AssetExport）（保留原始英文名称 UABEANext4.Logic.ImportExport）

using UABEANext4.Util; // 引用工具类命名空间（MessageBoxUtil 等）（保留原始英文名称 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原始英文名称）

public partial class EditDataViewModel : ViewModelBase, IDialogAware<byte[]?> // 定义部分类 EditDataViewModel，继承 ViewModelBase 并实现 IDialogAware<byte[]?>（对话框返回 byte[]?）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（将生成公开属性 Document）
    private TextDocument? _document; // 私有字段：TextDocument 的后备字段（Document），用于在编辑器中显示/编辑文本

    private AssetTypeValueField _baseField; // 私有字段：保存传入的 AssetTypeValueField（反序列化后的资产字段树），用于导入/导出操作

    private RefTypeManager _refMan; // 私有字段：引用类型管理器（RefTypeManager），用于解析引用类型（如 MonoBehaviour 脚本引用）

    public string Title => "编辑数据 (Edit Data)"; // 对话框标题（UI 显示中文并在括号保留英文原名 "Edit Data"）

    public int Width => 350; // 对话框宽度（像素）

    public int Height => 550; // 对话框高度（像素）

    public event Action<byte[]?>? RequestClose; // 事件：请求关闭对话框并返回结果（byte[]? 表示导入后的原始字节或 null 表示取消）

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应在运行时直接调用（保留英文说明）
    public EditDataViewModel() // 无参构造函数（仅供设计器）
    {
        _document = new TextDocument(); // 为设计器初始化一个空的 TextDocument，便于设计器预览

        _baseField = new AssetTypeValueField(); // 为设计器创建一个空的 AssetTypeValueField 占位

        _refMan = new RefTypeManager(); // 为设计器创建一个空的 RefTypeManager 占位
    }

    public EditDataViewModel(AssetTypeValueField baseField, RefTypeManager refMan) // 运行时构造函数：接收要编辑的 baseField 与 refMan
    {
        _baseField = baseField; // 保存传入的 baseField，用于导出为 JSON 并在编辑器中显示

        _refMan = refMan; // 保存传入的引用类型管理器，用于后续导入时解析引用

        using var ms = new MemoryStream(); // 创建内存流用于将 baseField 导出为 JSON 文本

        var exporter = new AssetExport(ms); // 创建 AssetExport 实例，绑定到内存流，用于将 baseField 导出为文本格式

        exporter.DumpJsonAsset(_baseField); // 将 baseField 导出为 JSON 并写入内存流（DumpJsonAsset）

        ms.Position = 0; // 将内存流位置重置到开头，以便读取刚写入的数据

        var str = Encoding.UTF8.GetString(ms.ToArray()); // 将内存流中的字节转换为 UTF-8 字符串（JSON 文本）

        Document = new TextDocument(str); // 将生成的 JSON 文本包装为 TextDocument 并赋值给公开属性 Document，供 UI 编辑
    }

    public async Task BtnOk_Click() // 异步方法：当用户点击“确定”按钮时调用，尝试将编辑器文本导入回二进制数据并返回
    {
        var text = Document!.Text; // 读取 Document 中的文本内容（假定 Document 非空）

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text)); // 将文本按 UTF-8 编码写入内存流，作为导入器的输入

        var importer = new AssetImport(ms, _refMan); // 创建 AssetImport 实例，传入内存流与引用类型管理器，用于将 JSON 转回二进制资产数据

        var data = importer.ImportJsonAsset(_baseField.TemplateField, out string? exceptionMessage); // 调用 ImportJsonAsset 使用模板字段进行导入，返回字节数组或 null，并输出异常信息

        if (data == null) // 如果导入失败（返回 null）
        {
            await MessageBoxUtil.ShowDialog("编译错误 (Compile Error)", "导入时出现问题:\n" + exceptionMessage); // 弹出错误对话框，显示中文提示并保留英文原文 "Compile Error"，同时显示异常信息
            return; // 终止方法，不关闭对话框
        }

        RequestClose?.Invoke(data); // 导入成功，触发 RequestClose 事件并传回导入得到的字节数组，通知宿主关闭对话框并接收结果
    }

    public void BtnCancel_Click() // 方法：当用户点击“取消”按钮时调用
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传回 null，表示用户取消操作
    }
} // 类 EditDataViewModel 结束
