using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UABEANext4.Logic;

namespace UABEANext4.Controls
{
    public class UnityViewHost : NativeControlHost
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private const int GWLP_HWNDPARENT = -8;

        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_CLIPSIBLINGS = 0x04000000;
        private const int WS_CLIPCHILDREN = 0x02000000;
        private const int SW_SHOW = 5;
        private const int SW_SHOWNA = 8;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const uint SWP_NOCOPYBITS = 0x0100;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint RDW_ALLCHILDREN = 0x0080;

        public static readonly StyledProperty<string> FileNameProperty =
            AvaloniaProperty.Register<UnityViewHost, string>(nameof(FileName), string.Empty);

        public string FileName
        {
            get => GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<UnityViewHost, bool>(nameof(IsActive), false);

        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        static UnityViewHost()
        {
            FileNameProperty.Changed.AddClassHandler<UnityViewHost>((x, e) => x.OnFileNameChanged(e));
            IsActiveProperty.Changed.AddClassHandler<UnityViewHost>((x, e) => x.OnIsActiveChanged(e));
        }

        private string _vrcaPath = string.Empty;
        private IntPtr _hwnd = IntPtr.Zero;
        private IntPtr _unityHwnd = IntPtr.Zero;
        private Process? _unityProcess;
        private bool _isLaunching = false;

        private void OnFileNameChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var newPath = e.NewValue as string;
            if (!string.IsNullOrEmpty(newPath))
            {
                _vrcaPath = newPath;
                if (IsActive) TryLaunch();
            }
        }

        private void OnIsActiveChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (IsActive)
            {
                TryLaunch();
            }
            else
            {
                // Kill process if we switch away
                KillUnityProcess();
                UnityViewerInteractor.LastLog = "Stopped (Inactive)";
            }
        }

        public UnityViewHost()
        {
            this.SizeChanged += OnSizeChanged;
            this.AttachedToVisualTree += OnAttachedToVisualTree;
            this.DetachedFromVisualTree += OnDetachedFromVisualTree;
            
            // Safety net: Ensure process dies if the app crashes or exits
            AppDomain.CurrentDomain.ProcessExit += (s, e) => KillUnityProcess();
        }

        private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            // Track parent window position changes
            var window = TopLevel.GetTopLevel(this) as Avalonia.Controls.Window;
            if (window != null)
            {
                window.PositionChanged += OnWindowPositionChanged;
                // Also kill if the window explicitly closes
                window.Closing += (s, args) => KillUnityProcess();
            }
        }

        private void OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            KillUnityProcess();
        }

        private void OnWindowPositionChanged(object? sender, Avalonia.Controls.PixelPointEventArgs e)
        {
            if (IsActive) PositionUnityOverlay();
        }

        private void KillUnityProcess()
        {
            if (_unityProcess != null && !_unityProcess.HasExited)
            {
                try { _unityProcess.Kill(); } catch { }
                _unityProcess = null;
            }
            _unityHwnd = IntPtr.Zero;
        }

        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (IsActive) PositionUnityOverlay();
        }

        private void TryLaunch()
        {
            if (_hwnd == IntPtr.Zero || string.IsNullOrEmpty(_vrcaPath) || _isLaunching || !IsActive)
                return;

            // Kill previous if exists
            KillUnityProcess();

            _isLaunching = true;
            LaunchAndEmbed();
        }

        private async void LaunchAndEmbed()
        {
            var viewerPath = UnityViewerInteractor.ViewerPath;
            if (string.IsNullOrEmpty(viewerPath) || !System.IO.File.Exists(viewerPath))
            {
                UnityViewerInteractor.LastLog = $"Viewer not found: {viewerPath}";
                _isLaunching = false;
                return;
            }

            try
            {
                // Launch WITHOUT -parentHWND, we'll embed manually
                var psi = new ProcessStartInfo
                {
                    FileName = viewerPath,
                    Arguments = $"\"{_vrcaPath}\" -popupwindow -screen-fullscreen 0",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(viewerPath),
                    UseShellExecute = false
                };

                _unityProcess = Process.Start(psi);
                UnityViewerInteractor.LastLog = $"Launched PID: {_unityProcess?.Id}";

                if (_unityProcess != null)
                {
                    // Wait for the window to be created
                    await Task.Delay(1500);

                    IntPtr unityHwnd = _unityProcess.MainWindowHandle;
                    
                    // Retry a few times if handle not ready
                    for (int i = 0; i < 10 && unityHwnd == IntPtr.Zero; i++)
                    {
                        await Task.Delay(300);
                        _unityProcess.Refresh();
                        unityHwnd = _unityProcess.MainWindowHandle;
                    }

                    if (unityHwnd != IntPtr.Zero)
                    {
                        _unityHwnd = unityHwnd; // Store for resize

                        // OWNED POPUP APPROACH: Remove Unity's border and set our window as owner.
                        // This keeps Unity in front of our window while preserving its rendering and input.
                        int style = GetWindowLong(unityHwnd, GWL_STYLE);
                        // Remove borders, caption, etc.
                        style &= ~0x00C00000;    // Remove WS_CAPTION
                        style &= ~0x00040000;    // Remove WS_THICKFRAME
                        style |= WS_VISIBLE;
                        SetWindowLong(unityHwnd, GWL_STYLE, style);

                        // Set our main window as the owner of the Unity window
                        // This makes Unity stay in front of us and minimize/close with us
                        var topLevel = TopLevel.GetTopLevel(this) as Avalonia.Controls.Window;
                        var ownerHwnd = topLevel?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                        if (ownerHwnd != IntPtr.Zero)
                        {
                            SetWindowLongPtr(unityHwnd, GWLP_HWNDPARENT, ownerHwnd);
                        }

                        // Position it initially
                        PositionUnityOverlay();
                        ShowWindow(unityHwnd, SW_SHOW);
                        
                        UnityViewerInteractor.LastLog = $"Owned! PID:{_unityProcess.Id} HWND:{unityHwnd}";
                    }
                    else
                    {
                        UnityViewerInteractor.LastLog = "Failed to get Unity window handle";
                    }
                }
            }
            catch (Exception ex)
            {
                UnityViewerInteractor.LastLog = $"Launch error: {ex.Message}";
            }
            finally
            {
                _isLaunching = false;
            }
        }

        private void PositionUnityOverlay()
        {
            if (_unityHwnd == IntPtr.Zero) return;

            try
            {
                // Get screen coordinates of this control
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel == null) return;

                double scaling = topLevel.RenderScaling;
                
                // Get our position relative to the top-level window's CLIENT area
                var controlPos = this.TranslatePoint(new Point(0, 0), topLevel);
                if (!controlPos.HasValue) return;

                // Get the top-level window handle
                var topLevelHwnd = (topLevel as Avalonia.Controls.Window)?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
                if (topLevelHwnd == IntPtr.Zero) return;

                // Convert client coordinates to screen coordinates
                // Start with the control position in client coordinates (scaled to physical pixels)
                POINT clientPoint = new POINT 
                { 
                    X = (int)(controlPos.Value.X * scaling), 
                    Y = (int)(controlPos.Value.Y * scaling) 
                };
                
                // Convert to screen coordinates
                ClientToScreen(topLevelHwnd, ref clientPoint);

                int screenX = clientPoint.X;
                int screenY = clientPoint.Y;
                int w = (int)(Bounds.Width * scaling);
                int h = (int)(Bounds.Height * scaling);

                if (w > 0 && h > 0)
                {
                    // Position the Unity window on screen
                    SetWindowPos(_unityHwnd, IntPtr.Zero, screenX, screenY, w, h, 
                        SWP_NOCOPYBITS | SWP_NOZORDER | SWP_NOACTIVATE);

                    // Send resize command to Unity via IPC
                    UnityViewerInteractor.SendResizeCommand((float)w, (float)h);
                }
            }
            catch { /* Ignore positioning errors */ }
        }

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            _hwnd = handle.Handle;

            if (string.IsNullOrEmpty(_vrcaPath))
            {
                _vrcaPath = FileName;
            }

            TryLaunch();
            return handle;
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            KillUnityProcess();
            base.DestroyNativeControlCore(control);
            _hwnd = IntPtr.Zero;
        }
    }
}
