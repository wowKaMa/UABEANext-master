using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using UABEANext4.ViewModels.Tools;
using UABEANext4.AssetWorkspace;
using AssetsTools.NET;

using Avalonia.Interactivity;
using Avalonia.Data.Converters;
using System.Globalization;
using System;

namespace UABEANext4.Views.Tools
{
    public partial class VrcExpParamsWindow : Window
    {
        public static IValueConverter TypeIdToNameConverter { get; } = new FuncValueConverter<int, string>(id => id switch
        {
            0 => "整数 (Int)",
            1 => "浮点 (Float)",
            2 => "布尔 (Bool)",
            _ => $"未知 ({id})"
        });

        public VrcExpParamsWindow()
        {
            InitializeComponent();
            Resources.Add("TypeIdToNameConverter", TypeIdToNameConverter);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public VrcExpParamsWindow(Workspace workspace, AssetInst asset) : this()
        {
            DataContext = new VrcExpParamsViewModel(workspace, asset);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
