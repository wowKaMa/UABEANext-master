// 引用 Avalonia 控件库，包含 Control、TextBlock、Window 等 UI 控件类型
using Avalonia.Controls;

// 引用 Avalonia 的模板接口定义，用于实现 IDataTemplate 等模板相关接口
using Avalonia.Controls.Templates;

// 引用 CommunityToolkit MVVM 的基础类型（例如 ObservableObject）
using CommunityToolkit.Mvvm.ComponentModel;

// 引用 Dock.Model.Core，包含 IDockable 等停靠布局相关接口
using Dock.Model.Core;

// 引用系统基础命名空间，包含 Exception、Activator 等类型
using System;

// 定义当前代码所属的命名空间，与项目结构保持一致
namespace UABEANext4;

// 定义 ViewLocator 类，实现 IDataTemplate 接口，用于将 ViewModel 映射为 View
public class ViewLocator : IDataTemplate
{
    // Build 方法：根据传入的数据（通常是 ViewModel 实例）构建并返回对应的 Control（View）
    public Control Build(object? data)
    {
        // 如果传入的数据为 null，则返回一个显示提示的 TextBlock（中文并保留英文原文）
        if (data == null)
        {
            return new TextBlock { Text = "空的视图模型 (Null view model)" };
        }

        // 获取传入对象的运行时类型（例如 UABEANext4.ViewModels.MainViewModel）
        var dataType = data.GetType();

        // 通过命名约定将类型名中的 "ViewModel" 替换为 "View"（例如 MainViewModel -> MainView）
        var name = dataType.FullName!.Replace("ViewModel", "View");

        // 在该类型所属的程序集（Assembly）中查找替换后名称对应的类型（View）
        var type = dataType.Assembly.GetType(name);

        // 如果找到了对应的 View 类型
        if (type != null)
        {
            // 使用反射动态创建该 View 类型的实例（Activator.CreateInstance）
            var instance = (Control)Activator.CreateInstance(type)!;

            // 如果实例创建成功，则返回该 Control，框架会将其显示为对应的视图
            if (instance != null)
            {
                return instance;
            }
            else
            {
                // 如果实例创建失败，返回一个包含错误信息的 TextBlock（中文并保留英文原文）
                return new TextBlock { Text = "创建实例失败 (Create Instance Failed): " + type.FullName };
            }
        }
        else
        {
            // 如果未找到对应的 View 类型，返回一个提示信息的 TextBlock（中文并保留英文原文）
            return new TextBlock { Text = "未找到 (Not Found): " + name };
        }
    }

    // Match 方法：告诉框架哪些数据类型应由此 IDataTemplate 处理（返回 true 则使用 Build 创建视图）
    public bool Match(object? data)
    {
        // 如果数据是 ObservableObject（MVVM 基类）或实现了 IDockable（停靠项），则匹配成功
        return data is ObservableObject or IDockable;
    }
}
