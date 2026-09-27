using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型命名空间，用于访问 ObservableObject 和 ObservableValidator 等 MVVM 基类（CommunityToolkit.Mvvm.ComponentModel）

// 定义命名空间 UABEANext4.ViewModels，用于组织视图模型类（namespace UABEANext4.ViewModels）
namespace UABEANext4.ViewModels;

public class ViewModelBase : ObservableObject // 定义一个名为 ViewModelBase 的公共类，继承自 ObservableObject；用于作为所有视图模型的基础类，提供属性变更通知功能（ViewModelBase : ObservableObject）
{
} // 类体为空，表示直接继承 ObservableObject 的行为而不添加额外成员；可在此处扩展通用视图模型逻辑

public class ViewModelBaseValidator : ObservableValidator // 定义一个名为 ViewModelBaseValidator 的公共类，继承自 ObservableValidator；用于支持带验证功能的视图模型基类（ViewModelBaseValidator : ObservableValidator）
{
} // 类体为空，表示直接继承 ObservableValidator 的验证与通知功能，便于在需要数据验证的视图模型中复用
