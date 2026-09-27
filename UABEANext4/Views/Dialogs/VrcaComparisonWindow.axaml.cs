using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using System.Collections.Generic;
using System.Linq;
using UABEANext4.ViewModels.Dialogs;

namespace UABEANext4.Views.Dialogs
{
    public partial class VrcaComparisonWindow : Window
    {
        public VrcaComparisonWindow()
        {
            InitializeComponent();
        }

        public VrcaComparisonWindow(VrcaComparisonViewModel viewModel) : this()
        {
            DataContext = viewModel;
            
            // Wait for text to be loaded then apply highlighters
            this.Opened += (s, e) => ApplyHighlighters(viewModel);
        }

        private void ApplyHighlighters(VrcaComparisonViewModel vm)
        {
            var sourceEditor = this.FindControl<TextEditor>("sourceEditor");
            var targetEditor = this.FindControl<TextEditor>("targetEditor");
            var resultEditor = this.FindControl<TextEditor>("resultEditor");

            if (sourceEditor != null) sourceEditor.TextArea.TextView.LineTransformers.Add(new DiffHighlighter(vm.SourceDiffLines, Color.Parse("#40569CD6")));
            if (targetEditor != null) targetEditor.TextArea.TextView.LineTransformers.Add(new DiffHighlighter(vm.TargetDiffLines, Color.Parse("#404EC9B0")));
            if (resultEditor != null) resultEditor.TextArea.TextView.LineTransformers.Add(new DiffHighlighter(vm.ResultDiffLines, Color.Parse("#40DCDCAA")));
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private class DiffHighlighter : DocumentColorizingTransformer
        {
            private readonly HashSet<int> _diffLines;
            private readonly IBrush _backgroundBrush;

            public DiffHighlighter(HashSet<int> diffLines, Color backgroundColor)
            {
                _diffLines = diffLines;
                _backgroundBrush = new SolidColorBrush(backgroundColor);
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (_diffLines.Contains(line.LineNumber))
                {
                    ChangeLinePart(
                        line.Offset,
                        line.EndOffset,
                        (VisualLineElement element) =>
                        {
                            element.TextRunProperties.SetBackgroundBrush(_backgroundBrush);
                        });
                }
            }
        }
    }
}
