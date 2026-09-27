using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace UABEANext4.Logic;

public static class UnityViewerInteractor
{
    private static string _viewerPath = string.Empty;
    public static string LastLog { get; internal set; } = "Not started";

    public static string ViewerPath
    {
        get
        {
            if (string.IsNullOrEmpty(_viewerPath))
            {
                // Check custom NewestViewer location
                string newestPath = @"d:\Antigravity\NewestViewer\Build\AssetViewer.exe";
                if (File.Exists(newestPath)) 
                {
                    _viewerPath = newestPath;
                }
                else
                {
                    // If not found, maybe log warning? 
                    // For now, keep empty or default to the expected path so user knows where to put it.
                    _viewerPath = newestPath; 
                }
            }
            return _viewerPath;
        }
        set => _viewerPath = value;
    }

    public static bool IsViewerAvailable => !string.IsNullOrEmpty(ViewerPath) && File.Exists(ViewerPath);

    public static void LaunchViewer(string vrcaPath)
    {
        LaunchEmbedded(vrcaPath, IntPtr.Zero);
    }

    public static void LaunchEmbedded(string vrcaPath, IntPtr parentHwnd)
    {
        if (!IsViewerAvailable) 
        {
            LastLog = $"Viewer not found at: {ViewerPath}";
            return;
        }

        try
        {
            LastLog = $"Attempting load...\nPath present: {!string.IsNullOrEmpty(vrcaPath)}\nHWND valid: {parentHwnd != IntPtr.Zero}";

            if (string.IsNullOrEmpty(vrcaPath)) 
            {
                LastLog += "\nSTOP: vrcaPath is empty";
                return;
            }

            // Unity supports -parentHWND <hwnd> to run as a child window
            string args = $"\"{vrcaPath}\"";
            if (parentHwnd != IntPtr.Zero)
            {
                args += $" -parentHWND {parentHwnd.ToInt64()} -popupwindow -screen-fullscreen 0";
            }
            
            LastLog = $"Launching: {ViewerPath}\nArgs: {args}\nHWND: {parentHwnd}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ViewerPath,
                Arguments = args,
                WorkingDirectory = Path.GetDirectoryName(ViewerPath),
                UseShellExecute = true
            };
            Process.Start(psi);
            LastLog += "\nProcess Started Successfully";
        }
        catch (Exception ex)
        {
            LastLog = $"Failed launch: {ex.Message}";
            Debug.WriteLine($"[UnityViewer] Failed to launch viewer: {ex.Message}");
        }
    }

    public static async Task SendSelectCommand(long pathId)
    {
        try
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync("127.0.0.1", 5555);
                using (var stream = client.GetStream())
                {
                    string msg = $"SELECT|{pathId}";
                    byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UnityViewer] IPC Error: {ex.Message}");
        }
    }

    public static async Task<bool> DecompressVrca(string vrcaPath)
    {
        if (!IsViewerAvailable) return false;

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ViewerPath,
                Arguments = $"\"{vrcaPath}\" \"cacheDecompress\"",
                WorkingDirectory = Path.GetDirectoryName(ViewerPath),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            
            var proc = Process.Start(psi);
            if (proc != null)
            {
                await proc.WaitForExitAsync();
                return proc.ExitCode == 0;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UnityViewer] Decompression failed: {ex.Message}");
        }
        return false;
    }

    public static async void SendCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync("127.0.0.1", 39215);
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(command);
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }
        catch (Exception)
        {
            // Silently fail
        }
    }

    public static void HighlightObject(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        SendCommand($"SELECT:{name}");
    }

    public static void SendResizeCommand(float width, float height)
    {
        SendCommand($"RESIZE:{width}:{height}");
    }
}
