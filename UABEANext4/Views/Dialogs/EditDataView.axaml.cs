using Avalonia.Controls; // 引用 Avalonia 控件库，用于使用文本编辑器、UserControl、Control 等 UI 控件（Avalonia.Controls）

using Avalonia.Interactivity; // 引用交互事件类型，用于 RoutedEventArgs、事件处理（Avalonia.Interactivity）

using AvaloniaEdit.TextMate; // 引用 AvaloniaEdit 的 TextMate 扩展，用于语法高亮与 TextMate 集成（AvaloniaEdit.TextMate）

using TextMateSharp.Grammars; // 引用 TextMateSharp 的语法与主题支持，用于 RegistryOptions、ThemeName 等（TextMateSharp.Grammars）

// 定义命名空间：UABEANext4.Views.Dialogs（组织对话框视图相关的类）
namespace UABEANext4.Views.Dialogs;

public partial class EditDataView : UserControl // 定义部分类 EditDataView，继承自 UserControl（EditDataView : UserControl），表示一个用于编辑数据的对话框视图
{
    public EditDataView() // 构造函数：当 EditDataView 实例被创建时调用（构造函数 EditDataView）
    {
        InitializeComponent(); // 初始化组件：加载并解析与此控件关联的 .axaml（界面布局），将 XAML 中定义的控件实例化并绑定到此类（InitializeComponent）

        Loaded += EditDataView_Loaded; // 注册 Loaded 事件处理器：当控件加载完成时调用 EditDataView_Loaded（Loaded += EditDataView_Loaded）
    }

    private void EditDataView_Loaded(object? sender, RoutedEventArgs e) // Loaded 事件的处理方法：控件加载完成后执行（EditDataView_Loaded），参数为事件发送者和路由事件参数（RoutedEventArgs）
    {
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus); // 创建 RegistryOptions 实例并指定主题为 DarkPlus（RegistryOptions、ThemeName.DarkPlus），用于 TextMate 语法/主题配置

        var textMateInstallation = textEditor.InstallTextMate(registryOptions); // 在名为 textEditor 的控件上安装 TextMate 支持（InstallTextMate），返回一个 TextMate 安装实例（textMateInstallation）

        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".json").Id)); // 设置编辑器的语法（SetGrammar），这里通过扩展名 ".json" 获取语言 ID，再通过该 ID 获取对应的 scope 并应用（GetLanguageByExtension -> GetScopeByLanguageId -> SetGrammar）
    }
}
