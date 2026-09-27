using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.AssetWorkspace;
using CommunityToolkit.Mvvm.Messaging;
using UABEANext4.Logic;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Avalonia.Platform.Storage;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AssetsTools.NET.Texture;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace UABEANext4.ViewModels.Tools
{
    public enum BruteForceMode
    {
        Numeric,    // 纯数字
        ModelName,  // 模型名组合
        Date,       // 日期
        Dictionary  // 字典
    }

    public partial class OscToolViewModel : ViewModelBase
    {
        [ObservableProperty] private string _password = "";
        [ObservableProperty] private int _keyIndex = 0; // 0=4, 1=8, 2=12, 3=16
        [ObservableProperty] private int _port = 9000;
        [ObservableProperty] private bool _isMultiplexing = false;
        [ObservableProperty] private int _refreshRate = 10;
        [ObservableProperty] private bool _isRunning = false;
        [ObservableProperty] private bool _saveSettings = false;
        [ObservableProperty] private bool _startHidden = false; // "Start & Hide window on start" - for future use?

        // Brute Force Properties
        [ObservableProperty] private bool _isBruteForcing = false;
        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(IsNumericMode))]
        [NotifyPropertyChangedFor(nameof(IsModelNameMode))]
        [NotifyPropertyChangedFor(nameof(IsDateMode))]
        [NotifyPropertyChangedFor(nameof(IsDictionaryMode))]
        private BruteForceMode _selectedBruteForceMode = BruteForceMode.Numeric;
        
        partial void OnSelectedBruteForceModeChanged(BruteForceMode value)
        {
            OnPropertyChanged(nameof(SelectedBruteForceModeIndex));
        }

        public bool IsNumericMode => SelectedBruteForceMode == BruteForceMode.Numeric;
        public bool IsModelNameMode => SelectedBruteForceMode == BruteForceMode.ModelName;
        public bool IsDateMode => SelectedBruteForceMode == BruteForceMode.Date;
        public bool IsDictionaryMode => SelectedBruteForceMode == BruteForceMode.Dictionary;

        public int SelectedBruteForceModeIndex
        {
            get => (int)SelectedBruteForceMode;
            set
            {
                if (Enum.IsDefined(typeof(BruteForceMode), value))
                {
                    SelectedBruteForceMode = (BruteForceMode)value;
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty] private string _bruteForceModelName = "Kokoa";
        [ObservableProperty] private int _bruteForceMaxDigits = 4;
        [ObservableProperty] private string _dictionaryFolderPath = "";

        public List<BruteForceMode> BruteForceModes { get; } = Enum.GetValues(typeof(BruteForceMode)).Cast<BruteForceMode>().ToList();

        [ObservableProperty] private int _nonce0 = 0;
        [ObservableProperty] private int _nonce1 = 0;
        [ObservableProperty] private int _nonce2 = 0;

        // Key length logic: 4 * (KeyIndex + 1) -> 4, 8, 12, 16
        public int KeyLength => 4 * (KeyIndex + 1);
        public List<string> KeyLengths { get; } = new() { "4", "8", "12", "16" };
        public ObservableCollection<string> Logs { get; } = new();

        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _bruteForceCts;
        private UdpClient? _udpClient;
        private Workspace _workspace = null!;
        private string? _lastLoggedPassword;
        private static byte[]? _lastDecryptedPalette;
        private bool _isInternalSelectionChange = false;

        public OscToolViewModel(Workspace workspace = null!)
        {
            _workspace = workspace;
            if (_workspace != null)
            {
                WeakReferenceMessenger.Default.Register<AssetsSelectedMessage>(this, (r, m) => OnAssetsSelected(m));
                WeakReferenceMessenger.Default.Register<SelectedWorkspaceItemChangedMessage>(this, (r, m) => OnWorkspaceItemsChanged(m));
            }
        }

        [RelayCommand]
        private void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _udpClient = new UdpClient();
            
            AddLog($"OSC 已在端口 {Port} 启动 (OSC Started on Port {Port})");
            _lastLoggedPassword = null;
            
            // Start background loop
            Task.Run(() => RunOscLoop(_cancellationTokenSource.Token));
        }

        [RelayCommand]
        private void Stop()
        {
            if (!IsRunning) return;

            // Stop Brute Force if running
            if (IsBruteForcing)
            {
                StopBruteForce();
            }

            IsRunning = false;
            _cancellationTokenSource?.Cancel();
            _udpClient?.Close();
            _udpClient = null;
            AddLog("OSC 已停止 (OSC Stopped)");
        }

        [RelayCommand]
        private void StartBruteForce()
        {
            if (IsBruteForcing) return;
            if (!IsRunning)
            {
                Start(); // Ensure OSC is running
            }

            
            IsBruteForcing = true;
            _bruteForceCts = new CancellationTokenSource();
            
            IsBruteForcing = true;
            _bruteForceCts = new CancellationTokenSource();
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog($"开始暴力破解: {SelectedBruteForceMode} (Starting Brute Force)"));
            Task.Run(() => RunBruteForceLoop(_bruteForceCts.Token));
        }

        [RelayCommand]
        private void StopBruteForce()
        {
            if (!IsBruteForcing) return;
            IsBruteForcing = false;
            _bruteForceCts?.Cancel();
            AddLog("暴力破解已停止 (Brute Force Stopped)");
        }

        [RelayCommand]
        private async Task SelectDictionaryFolder()
        {
            var storageProvider = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop 
                ? desktop.MainWindow?.StorageProvider 
                : null;

            if (storageProvider == null) return;

            var result = await storageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
            {
                Title = "选择字典文件夹 (Select Dictionary Folder)",
                AllowMultiple = false
            });

            if (result.Count > 0)
            {
                DictionaryFolderPath = result[0].Path.LocalPath;
            }
        }

        [RelayCommand]
        private void ClearLogs()
        {
            Logs.Clear();
        }

        private async Task RunOscLoop(CancellationToken token)
        {
            try
            {
                float factor = (float)Math.Pow(10, 4);

                while (!token.IsCancellationRequested)
                {
                    // Re-calculate Digest per loop to support changing passwords (e.g. Brute Force)
                    // Prepare Password Buffer (mimic ImGui truncation)
                    string effectivePassword = Password;
                    // Use current KeyLength
                    int currentKeyLength = KeyLength; 
                    
                    if (_lastLoggedPassword != effectivePassword)
                    {
                        _lastLoggedPassword = effectivePassword;
                        LogKeyBytes(effectivePassword);
                    }
                    
                    if (effectivePassword.Length > currentKeyLength)
                        effectivePassword = effectivePassword.Substring(0, currentKeyLength);

                    byte[] inputBytes = Encoding.UTF8.GetBytes(effectivePassword);
                    
                    // Password Buffer for XOR (Padded)
                    byte[] passwordBytes = new byte[currentKeyLength];
                    Array.Copy(inputBytes, passwordBytes, Math.Min(inputBytes.Length, currentKeyLength));

                    // SHA256 Digest
                    using var sha256 = SHA256.Create();
                    byte[] digest = sha256.ComputeHash(inputBytes);

                    for (int i = 0; i < currentKeyLength; i++)
                    {
                        if (token.IsCancellationRequested) break;

                        if (IsMultiplexing)
                        {
                            // Multiplexing requires sequential steps with small delays for VRChat to sync
                            SendOsc("/avatar/parameters/encrypt_lock", true);
                            SendOsc("/avatar/parameters/encrypt_switch0", (i & 1) != 0);
                            SendOsc("/avatar/parameters/encrypt_switch1", (i & 2) != 0);
                            SendOsc("/avatar/parameters/encrypt_switch2", (i & 4) != 0);
                            SendOsc("/avatar/parameters/encrypt_switch3", (i & 8) != 0);

                            byte pChar = passwordBytes[i];
                            byte dChar = i < digest.Length ? digest[i] : (byte)0;
                            byte var = (byte)(pChar ^ dChar);
                            float pwd = 1.0f - (var / 128.0f);
                            pwd = -(float)(Math.Round(pwd * factor) / factor);

                            SendOsc("/avatar/parameters/pkey", pwd);
                            
                            // User requested fastest: Reduce wait to minimum RefreshRate
                            await Task.Delay(RefreshRate, token);
                            
                            SendOsc("/avatar/parameters/encrypt_lock", false);
                            await Task.Delay(RefreshRate, token); 
                        }
                        else
                        {
                            // In non-multiplexing mode, we can burst all parameters instantly
                            byte pChar = passwordBytes[i];
                            byte dChar = i < digest.Length ? digest[i] : (byte)0;
                            byte var = (byte)(pChar ^ dChar);
                            float pwd = 1.0f - (var / 128.0f);
                            pwd = -(float)(Math.Round(pwd * factor) / factor);

                            string addr = $"/avatar/parameters/pkey{i}";
                            SendOsc(addr, pwd);
                            // No delay between keys in non-multiplex mode for "Fastest"
                        }
                    }

                    // Cycle Delay: Reduced from 1000ms to RefreshRate for responsive updates
                    await Task.Delay(RefreshRate, token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                AddLog($"错误: {ex.Message} (Error: {ex.Message})");
                IsRunning = false;
            }
        }

        private void SendOsc(string address, object value)
        {
            if (_udpClient == null) return;
            try
            {
                byte[] packet = BuildOscPacket(address, value);
                _udpClient.Send(packet, packet.Length, "127.0.0.1", Port);
            }
            catch (Exception)
            {
                // AddLog($"Send Error: {ex.Message}");
            }
        }

        private byte[] BuildOscPacket(string address, object value)
        {
            List<byte> data = new List<byte>();

            // Address String (padded to 4 bytes)
            data.AddRange(Encoding.UTF8.GetBytes(address));
            data.Add(0); // Null terminator
            while (data.Count % 4 != 0) data.Add(0);

            // Type Tag String (padded to 4 bytes)
            data.Add((byte)',');
            
            if (value is float) data.Add((byte)'f');
            else if (value is int) data.Add((byte)'i'); // C++ treats bool as 'T'/'F' or 'i'?
            else if (value is bool b) data.Add(b ? (byte)'T' : (byte)'F');
            
            data.Add(0); // Null terminator
            while (data.Count % 4 != 0) data.Add(0);

            // Arguments
            if (value is float f)
            {
                byte[] bytes = BitConverter.GetBytes(f);
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                data.AddRange(bytes);
            }
            else if (value is int i)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                data.AddRange(bytes);
            }
            // Bool has no data payload in OSC 1.0 (True/False tags are sufficient)

            return data.ToArray();
        }

        private void AddLog(string message)
        {
            // Use Post instead of Invoke to avoid blocking the caller (especially background threads)
            // and reduce the chance of UI starvation leading to PlatformImpl errors.
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (Logs.Count > 100) Logs.RemoveAt(0);
            });
        }

        private void LogKeyBytes(string pwd)
        {
            byte[] key = new byte[16];
            byte[] fixedKeyBytes = Encoding.ASCII.GetBytes("password");
            byte[] userKeyBytes = Encoding.UTF8.GetBytes(pwd);
            byte[] hash;
            using (SHA256 sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(userKeyBytes);
            }

            for (int i = 0; i < fixedKeyBytes.Length && i < 16; ++i)
                key[i] = fixedKeyBytes[i];

            int currentKeyLength = KeyLength;
            if (currentKeyLength > 0)
            {
                for (int i = (16 - currentKeyLength), j = 0; i < key.Length; ++i, ++j)
                {
                    if (j < userKeyBytes.Length)
                        key[i] = (byte)(userKeyBytes[j] ^ hash[j]);
                    else
                        key[i] = hash[j];
                }
            }
            string keyStr = string.Join("\n", key.Select((b, index) => $"pkey{index}={b}"));
            AddLog($"当前尝试密码: {pwd}\n{keyStr}");
        }
        private async Task RunBruteForceLoop(CancellationToken token)
        {
            try
            {
                IEnumerable<string> generator = SelectedBruteForceMode switch
                {
                    BruteForceMode.Numeric => GenerateNumericPasswords(),
                    BruteForceMode.ModelName => GenerateModelNamePasswords(),
                    BruteForceMode.Date => GenerateDatePasswords(),
                    BruteForceMode.Dictionary => GenerateDictionaryPasswords(),
                    _ => Enumerable.Empty<string>()
                };

                foreach (var pwd in generator)
                {
                    if (token.IsCancellationRequested) break;

                    // Update Password Logic
                    // We need to update the UI thread's bound property
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => Password = pwd);

                    // Wait for OSC loop to pick it up and transmit at least once
                    // Since we optimized RunOscLoop, the cycle duration is now much shorter:
                    int cycleDuration;
                    if (IsMultiplexing)
                    {
                        // 2 delays (lock/unlock) * KeyLength * RefreshRate
                        cycleDuration = (KeyLength * 2 * RefreshRate) + RefreshRate;
                    }
                    else
                    {
                        // Instant burst + RefreshRate delay
                        cycleDuration = RefreshRate + 10; 
                    }
                    
                    await Task.Delay(cycleDuration, token);
                }

                AddLog("暴力破解完成 (Brute Force Completed)");
                Avalonia.Threading.Dispatcher.UIThread.Post(() => IsBruteForcing = false);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                AddLog($"暴力破解错误: {ex.Message}");
                Avalonia.Threading.Dispatcher.UIThread.Post(() => IsBruteForcing = false);
            }
        }

        private IEnumerable<string> GenerateNumericPasswords()
        {
            // 1 to 10 digits? User said "1 to 10 bits" (digits)
            // If MaxDigits is 4, do we try 1, 2, 3 digits first? 
            // User said "try 1 to 10 digits pure numeric".
            // Let's iterate length 1 to MaxDigits.
            
            for (int len = 1; len <= BruteForceMaxDigits; len++)
            {
                long max = (long)Math.Pow(10, len);
                for (long i = 0; i < max; i++)
                {
                    // Yield raw number string (e.g. "123") vs padded ("0123")?
                    // Usually brute force tries specific formats.
                    // Let's yield the zero-padded version for that length.
                    yield return i.ToString($"D{len}"); // "0005"
                    
                    // If we want raw unpadded (e.g. "5"), it is covered by len=1 loop
                }
            }
        }

        private IEnumerable<string> GenerateModelNamePasswords()
        {
            // ModelName + 0...9999 (User example: Kokoa000-Kokoa9999)
            // Assuming 3 digits to 4 digits suffix?
            // "Kokoa000" implies 3 digits.
            // Let's try 0 to 9999, formatted flexible.
            
            string baseName = BruteForceModelName;
            
            // Try 1 digit suffix
            for (int i = 0; i < 10; i++) yield return $"{baseName}{i}";
            
            // Try 2 digits
            for (int i = 0; i < 100; i++) yield return $"{baseName}{i:D2}";
            
            // Try 3 digits
            for (int i = 0; i < 1000; i++) yield return $"{baseName}{i:D3}";
            
            // Try 4 digits
            for (int i = 0; i < 10000; i++) yield return $"{baseName}{i:D4}";
        }

        private IEnumerable<string> GenerateDatePasswords()
        {
            // 2010 to 2026 YYYYMMDD
            DateTime start = new DateTime(2010, 1, 1);
            DateTime end = new DateTime(2026, 12, 31);
            
            for (var dt = start; dt <= end; dt = dt.AddDays(1))
            {
                yield return dt.ToString("yyyyMMdd");
            }
        }

        private IEnumerable<string> GenerateDictionaryPasswords()
        {
            if (string.IsNullOrEmpty(DictionaryFolderPath) || !System.IO.Directory.Exists(DictionaryFolderPath))
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog("字典文件夹无效 (Invalid Dictionary Folder)"));
                yield break;
            }

            var files = System.IO.Directory.GetFiles(DictionaryFolderPath, "*.txt", System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog($"加载字典: {System.IO.Path.GetFileName(file)}"));
                foreach (var line in System.IO.File.ReadLines(file))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        yield return line.Trim();
                    }
                }
            }
        }
        // Offline Decryption
        [ObservableProperty] private string _selectedAssetName = "未选择 (None)";
        [ObservableProperty] private bool _canDecrypt = false;
        [ObservableProperty] private int _selectedGammaModeIndex = 0; // 0=None, 1=2.2, 2=0.45
        [ObservableProperty] private string _channelOrder = "RGBA";
        [ObservableProperty] private int _encryptionRounds = 20; // Default rounds in ShellProtector 2.x
        [ObservableProperty] private string _selectedAssetFormat = "未知 (Unknown)";
        [ObservableProperty] private string _selectedAssetChannels = "未知 (Unknown)";
        
        public List<string> EncryptionModes { get; } = new() { "XXTEA", "ChaCha20" };
        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(IsXXTEAMode))]
        [NotifyPropertyChangedFor(nameof(IsChaChaMode))]
        private int _selectedEncryptionModeIndex = 0;

        public bool IsXXTEAMode => SelectedEncryptionModeIndex == 0;
        public bool IsChaChaMode => SelectedEncryptionModeIndex == 1;

        public List<string> GammaModes { get; } = new() { "不进行校正 (None)", "伽马 (Gamma 2.2 -> Linear)", "线性 (Linear -> Gamma 2.2)" };
        
        // New: Texture List for File Selection
        public ObservableCollection<AssetInst> TextureList { get; } = new();
        [ObservableProperty] private AssetInst? _selectedAsset;
        [ObservableProperty] private AssetInst? _targetAsset;

        partial void OnTargetAssetChanged(AssetInst? value)
        {
            if (_isInternalSelectionChange) return;
            if (value != null && SelectedEncryptionModeIndex == 1) // ChaCha20
            {
                TryFindNoncesForTexture(value);
            }
        }

        // Material List for Nonce Selection
        public ObservableCollection<AssetInst> MaterialList { get; } = new();
        [ObservableProperty] private AssetInst? _selectedMaterial;

        partial void OnSelectedMaterialChanged(AssetInst? value)
        {
            if (value == null || _workspace == null) return;
            TryExtractNoncesFromMaterial(value);
        }

        private void TryExtractNoncesFromMaterial(AssetInst materialAsset)
        {
            try
            {
                var baseField = _workspace.GetBaseField(materialAsset);
                if (baseField == null) return;

                var savedProps = baseField["m_SavedProperties"];
                if (savedProps == null || savedProps.IsDummy)
                {
                    AddLog($"[材质选择] 警告: {materialAsset.AssetName} 没有 m_SavedProperties");
                    return;
                }

                int n0 = Nonce0, n1 = Nonce1, n2 = Nonce2;
                bool found = false;

                // Helper to process a field that might be m_Floats or m_Ints
                void ProcessEntries(AssetTypeValueField field)
                {
                    if (field == null || field.IsDummy) return;
                    var arr = field["Array"];
                    if (arr.IsDummy) arr = field;
                    if (arr.Children == null) return;

                    foreach (var f in arr.Children)
                    {
                        var first = f["first"];
                        var second = f["second"];
                        if (first.IsDummy || second.IsDummy) continue;

                        string propName = first.AsString;
                        long val = second.AsLong;
                        
                        if (propName == "_Nonce0") { n0 = (int)val; found = true; }
                        else if (propName == "_Nonce1") { n1 = (int)val; found = true; }
                        else if (propName == "_Nonce2") { n2 = (int)val; found = true; }
                    }
                }

                ProcessEntries(savedProps["m_Floats"]);
                ProcessEntries(savedProps["m_Ints"]);

                if (found)
                {
                    Nonce0 = n0;
                    Nonce1 = n1;
                    Nonce2 = n2;
                    AddLog($"[材质选择] 已从 {materialAsset.AssetName} 提取 Nonce: {Nonce0}, {Nonce1}, {Nonce2}");
                }
                else
                {
                    AddLog($"[材质选择] 在 {materialAsset.AssetName} 中未找到 _Nonce0/1/2");
                }
            }
            catch (Exception ex)
            {
                AddLog($"[错误] 提取材质 Nonce 失败: {ex.Message}");
            }
        }

        partial void OnSelectedAssetChanged(AssetInst? value)
        {
            if (_isInternalSelectionChange) return;
            if (value != null)
            {
                SelectedAssetName = $"{value.AssetName} (PathID: {value.PathId})";
                CanDecrypt = true;

                try
                {
                    var baseField = _workspace.GetBaseField(value);
                    if (baseField != null)
                    {
                        var texFile = TextureFile.ReadTextureFile(baseField);
                        if (texFile == null) return;
                        var format = (TextureFormat)texFile.m_TextureFormat;
                        SelectedAssetFormat = $"格式: {format} (ID: {texFile.m_TextureFormat}) | 分辨率: {texFile.m_Width}x{texFile.m_Height}";

                        // Determine channels
                        string channels = "未知 (Unknown)";
                        switch (texFile.m_TextureFormat)
                        {
                            case 3: channels = "RGB (3通道)"; break;
                            case 4: channels = "RGBA (4通道)"; break;
                            case 5: channels = "ARGB (4通道)"; break;
                            case 10: channels = "DXT1 (BC1 - RGB)"; break;
                            case 12: channels = "DXT5 (BC3 - RGBA)"; break;
                            case 28: channels = "DXT1 Crunched (RGB)"; break;
                            case 29: channels = "DXT5 Crunched (RGBA)"; break;
                            default: channels = $"{format}"; break;
                        }
                        SelectedAssetChannels = $"通道 (Channels): {channels}";

                        // Heuristics for ShellProtector
                        string assetName = (value?.AssetName ?? "").ToLower();
                        bool isMainEncrypted = assetName.Contains("_encrypt") && !assetName.Contains("_encrypt2") && !assetName.Contains("_encrypttex");
                        bool isPaletteEncrypted = assetName.Contains("_encrypt2") || assetName.Contains("_encrypttex") || assetName.Contains("_encode");

                        if (isPaletteEncrypted)
                        {
                            // User confirmed: Palettes also use 20 rounds.
                            EncryptionRounds = 20;
                            SelectedGammaModeIndex = 0; // None
                            ChannelOrder = "RGBA";

                            if (SelectedEncryptionModeIndex == 1) // ChaCha20
                            {
                                TryFindNoncesForTexture(value);
                            }

                            // Automatic Pairing Logic (Stronger Search)
                            // 1. Extract the numeric ID part (everything before the first underscore)
                            string idPrefix = "";
                            int underscoreIndex = assetName.IndexOf('_');
                            if (underscoreIndex > 0)
                            {
                                idPrefix = assetName.Substring(0, underscoreIndex);
                            }
                            else
                            {
                                // Fallback to current prefix extraction
                                if (assetName.Contains("_encrypt2")) idPrefix = assetName.Substring(0, assetName.IndexOf("_encrypt2"));
                                else if (assetName.Contains("_encrypttex")) idPrefix = assetName.Substring(0, assetName.IndexOf("_encrypttex"));
                                else if (assetName.Contains("_encode")) idPrefix = assetName.Substring(0, assetName.IndexOf("_encode"));
                            }

                            if (!string.IsNullOrEmpty(idPrefix))
                            {
                                int listCount = TextureList?.Count ?? 0;
                                AddLog($"[诊断] 正在扫描 {listCount} 个资源，寻找主贴图 ID: {idPrefix}...");

                                // Priority 1: Exact {ID}_encrypt
                                string seekName = idPrefix + "_encrypt";
                                TargetAsset = TextureList?.FirstOrDefault(t => 
                                    t != null && t.AssetName != null && 
                                    t.AssetName.Equals(seekName, StringComparison.OrdinalIgnoreCase));
                                
                                // Priority 2: Starts with ID, contains _encrypt, but is not a known palette suffix
                                if (TargetAsset == null)
                                {
                                    TargetAsset = TextureList?.FirstOrDefault(t => 
                                        t != null && t.AssetName != null && 
                                        t.AssetName.StartsWith(idPrefix, StringComparison.OrdinalIgnoreCase) && 
                                        t.AssetName.Contains("_encrypt") && 
                                        !t.AssetName.Contains("_encrypt2") &&
                                        !t.AssetName.Contains("_encrypttex") &&
                                        !t.AssetName.Contains("_encode") &&
                                        t.PathId != (value?.PathId ?? 0));
                                }

                                if (TargetAsset != null && TargetAsset.AssetName != null)
                                {
                                    AddLog($"[自动关联] 成功匹配主贴图: {TargetAsset.AssetName}");
                                }
                                else
                                {
                                    AddLog($"[警告] 未能自动匹配主贴图 (ID: {idPrefix}).");
                                }
                            }
                        }
                        else if (isMainEncrypted)
                        {
                            EncryptionRounds = 20;
                            SelectedGammaModeIndex = 0; // None

                            if (SelectedEncryptionModeIndex == 1) // ChaCha20
                            {
                                TryFindNoncesForTexture(value);
                            }

                            // Bidirectional Pairing: If user selected main, try to find the palette and swap
                            string idPrefix = "";
                            int underscoreIndex = assetName.IndexOf('_');
                            if (underscoreIndex > 0) idPrefix = assetName.Substring(0, underscoreIndex);

                            if (!string.IsNullOrEmpty(idPrefix))
                            {
                                AddLog($"[诊断] 正在寻找关联副贴图 (ID: {idPrefix})...");
                                var palette = TextureList?.FirstOrDefault(t => 
                                    t != null && t.AssetName != null && 
                                    t.AssetName.StartsWith(idPrefix, StringComparison.OrdinalIgnoreCase) && 
                                    (t.AssetName.Contains("_encrypt2") || t.AssetName.Contains("_encrypttex") || t.AssetName.Contains("_encode")));

                                if (palette != null)
                                {
                                    AddLog($"[双向对位] 发现副贴图 {palette.AssetName}，正在自动重排...");
                                    _isInternalSelectionChange = true;
                                    SelectedAsset = palette; // Switch primary selection to palette
                                    TargetAsset = value;    // Put main texture in second box
                                    _isInternalSelectionChange = false;
                                    return; // Current call ends here
                                }
                                else
                                {
                                    TargetAsset = null; // No palette found
                                }
                            }
                        }

                        // Automatic Nonce Detection for ChaCha20
                        if (SelectedEncryptionModeIndex == 1) // ChaCha20
                        {
                            TryFindNoncesForTexture(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddLog($"[错误] 处理资源 {value.AssetName} 时发生异常: {ex.Message}");
                }
            }
            else
            {
                SelectedAssetName = "未选择 (None) - 请选择 Texture2D";
                SelectedAssetFormat = "";
                SelectedAssetChannels = "";
                CanDecrypt = false;
            }
        }

        private void TryFindNoncesForTexture(AssetInst textureAsset)
        {
            if (textureAsset == null || _workspace == null) return;

            // Collect all assets file instances in the workspace
            List<AssetsFileInstance> allFiles = new List<AssetsFileInstance>();
            
            void FindFiles(WorkspaceItem item)
            {
                if (item.ObjectType == WorkspaceItemType.AssetsFile && item.Object is AssetsFileInstance afi)
                {
                    allFiles.Add(afi);
                }
                foreach (var child in item.Children) FindFiles(child);
            }
            foreach (var root in _workspace.RootItems) FindFiles(root);

            foreach (var fileInst in allFiles)
            {
                try
                {
                    var materials = fileInst.file.GetAssetsOfType(21); // Material
                    foreach (var info in materials)
                    {
                        var asset = _workspace.GetAssetInst(fileInst, 0, info.PathId);
                        if (asset == null) continue;

                        string name = asset.AssetName ?? "";
                        if (name.Contains("_duplicated", StringComparison.OrdinalIgnoreCase))
                        {
                            var baseField = _workspace.GetBaseField(asset);
                            if (baseField == null) continue;

                            var savedProps = baseField["m_SavedProperties"];
                            if (savedProps == null || savedProps.IsDummy) continue;

                            bool matched = false;
                                // Handle m_TexEnvs with "Array"
                                var texEnvsField = savedProps["m_TexEnvs"];
                                var texEnvsArr = texEnvsField["Array"];
                                if (texEnvsArr.IsDummy) texEnvsArr = texEnvsField;

                                foreach (var texEnv in texEnvsArr.Children)
                                {
                                    var first = texEnv["first"];
                                    var second = texEnv["second"];
                                    if (first.IsDummy || second.IsDummy) continue;

                                    string propName = first.AsString;
                                    if (propName == "_EncryptTex0" || propName == "_EncryptTex1")
                                    {
                                        var texPtr = second["m_Texture"];
                                        if (texPtr.IsDummy) continue;
                                        
                                        long pathId = texPtr["m_PathID"].AsLong;

                                        // Match PathID against either selected or target texture
                                        if (pathId != 0 && (pathId == textureAsset.PathId || 
                                                           (TargetAsset != null && pathId == TargetAsset.PathId)))
                                        {
                                            matched = true;
                                            break;
                                        }
                                    }
                                }

                                if (matched)
                                {
                                    AddLog($"[自动探测] 发现匹配材质球: {name}");
                                    
                                    // Ensure it's in the MaterialList so the UI can select it
                                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                                        if (!MaterialList.Contains(asset))
                                            MaterialList.Add(asset);
                                        
                                        _isInternalSelectionChange = true;
                                        SelectedMaterial = asset;
                                        _isInternalSelectionChange = false;
                                    });
                                    
                                    // Extract Nonces using the updated logic (checking both m_Floats and m_Ints)
                                    int n0 = Nonce0, n1 = Nonce1, n2 = Nonce2;
                                    bool found = false;

                                    void ExtractFrom(AssetTypeValueField field)
                                    {
                                        if (field == null || field.IsDummy) return;
                                        var arr = field["Array"];
                                        if (arr.IsDummy) arr = field;
                                        if (arr.Children == null) return;
                                        foreach (var f in arr.Children)
                                        {
                                            var first = f["first"];
                                            var second = f["second"];
                                            if (first.IsDummy || second.IsDummy) continue;
                                            string propName = first.AsString;
                                            long val = second.AsLong;
                                            if (propName == "_Nonce0") { n0 = (int)val; found = true; }
                                            else if (propName == "_Nonce1") { n1 = (int)val; found = true; }
                                            else if (propName == "_Nonce2") { n2 = (int)val; found = true; }
                                        }
                                    }

                                    ExtractFrom(savedProps["m_Floats"]);
                                    ExtractFrom(savedProps["m_Ints"]);

                                    if (found)
                                    {
                                        Nonce0 = n0;
                                        Nonce1 = n1;
                                        Nonce2 = n2;
                                        AddLog($"[自动探测] 已自动填入 Nonce: {Nonce0}, {Nonce1}, {Nonce2}");
                                    }
                                    return;
                                }
                        }
                    }
                }
                catch
                {
                    // Ignore errors for individual files
                }
            }
        }

        private void OnWorkspaceItemsChanged(SelectedWorkspaceItemChangedMessage message)
        {
            if (message.Value == null || message.Value.Count == 0) return;
            
            TextureList.Clear();
            MaterialList.Clear();
            SelectedAsset = null;
            SelectedMaterial = null;
            SelectedAssetName = "正在扫描... (Scanning...)";
            CanDecrypt = false;

            Task.Run(() =>
            {
                var foundTextures = new List<AssetInst>();
                var foundMaterials = new List<AssetInst>();

                void ScanItem(WorkspaceItem item, int depth = 0)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog($"扫描 (Scan): {item.Name} [{item.ObjectType}] (Depth: {depth})"));

                    // Case 1: Direct AssetsFile
                    if (item.ObjectType == WorkspaceItemType.AssetsFile && item.Object is AssetsFileInstance fileInst)
                    {
                        try
                        {
                            var maxNameLen = Logic.Configuration.ConfigurationManager.Settings.ListingNameLength;
                            
                            // Scan Textures
                            var textures = fileInst.file.GetAssetsOfType((int)AssetClassID.Texture2D);
                            foreach (var info in textures)
                            {
                                var assetInst = _workspace.GetAssetInst(fileInst, 0, info.PathId);
                                if (assetInst == null) continue;
                                if (string.IsNullOrEmpty(assetInst.AssetName) || assetInst.AssetName == "Unnamed asset")
                                    assetInst.AssetName = _workspace.Namer.GetAssetName(assetInst, true, maxNameLen);
                                foundTextures.Add(assetInst);
                            }

                            // Scan Materials
                            var materials = fileInst.file.GetAssetsOfType(21); // Material
                            foreach (var info in materials)
                            {
                                var assetInst = _workspace.GetAssetInst(fileInst, 0, info.PathId);
                                if (assetInst == null) continue;
                                if (string.IsNullOrEmpty(assetInst.AssetName) || assetInst.AssetName == "Unnamed asset")
                                    assetInst.AssetName = _workspace.Namer.GetAssetName(assetInst, true, maxNameLen);
                                foundMaterials.Add(assetInst);
                            }
                        }
                        catch (Exception ex) 
                        {
                             Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog($"  - Error scanning file: {ex.Message}"));
                        }
                    }
                    
                    // Case 2: Recursion (Bundle contents or Directories)
                    if (item.Children.Count > 0)
                    {
                        foreach (var child in item.Children)
                        {
                            ScanItem(child, depth + 1);
                        }
                    }
                }

                if (message.Value.Count == 0)
                {
                     Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog("警告: 未选择文件 (No file selected)"));
                }

                foreach (var item in message.Value)
                {
                    ScanItem(item);
                }

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    TextureList.Clear();
                    foreach (var tex in foundTextures) TextureList.Add(tex);
                    
                    MaterialList.Clear();
                    foreach (var mat in foundMaterials) MaterialList.Add(mat);

                    if (TextureList.Count > 0)
                    {
                        SelectedAsset = TextureList[0];
                        SelectedAssetName = $"已扫描到 {TextureList.Count} 个纹理, {MaterialList.Count} 个材质";
                    }
                    else
                    {
                        SelectedAssetName = "选中文件中无纹理 (No textures in selected files)";
                    }
                });
            });
        }

        private void OnAssetsSelected(AssetsSelectedMessage message)
        {
            if (message.Value == null || message.Value.Count == 0 || _workspace == null) return;

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var selected = message.Value[0];
                if (selected.TypeId != (int)AssetClassID.Texture2D)
                {
                    SelectedAsset = null;
                    SelectedAssetName = "未选择 (None) - 请选择 Texture2D";
                    CanDecrypt = false;
                    return;
                }

                TextureList.Clear();
                var fileInst = selected.FileInstance;
                var maxNameLen = Logic.Configuration.ConfigurationManager.Settings.ListingNameLength;
                
                try
                {
                    var textures = fileInst.file.GetAssetsOfType((int)AssetClassID.Texture2D);
                    foreach (var info in textures)
                    {
                        var assetInst = _workspace.GetAssetInst(fileInst, 0, info.PathId);
                        if (assetInst == null) continue;

                        if (string.IsNullOrEmpty(assetInst.AssetName) || assetInst.AssetName == "Unnamed asset")
                        {
                            assetInst.AssetName = _workspace.Namer.GetAssetName(assetInst, true, maxNameLen);
                        }
                        TextureList.Add(assetInst);
                    }

                    // Select the one the user actually clicked on
                    SelectedAsset = TextureList.FirstOrDefault(t => t.PathId == selected.PathId) ?? TextureList.FirstOrDefault();
                    AddLog($"[扫描] 已从文件 {fileInst.name} 加载 {TextureList.Count} 个纹理。");
                }
                catch (Exception ex)
                {
                    AddLog($"[错误] 扫描文件纹理失败: {ex.Message}");
                    // Fallback to just the selected one
                    TextureList.Add(selected);
                    SelectedAsset = selected;
                }
            });
        }

        [RelayCommand]
        private async Task OfflineDecrypt()
        {
            if (SelectedAsset == null || string.IsNullOrEmpty(Password)) return;
            if (_workspace == null)
            {
                AddLog("错误:工作区未连接 (Error: Workspace not connected)");
                return;
            }

            try
            {
                // Sequence: If we have a target asset and current is a palette, do both.
                string assetName = SelectedAsset.AssetName?.ToLower() ?? "";
                bool isPalette = assetName.Contains("_encrypt2") || assetName.Contains("_encrypttex") || assetName.Contains("_encode");

                if (isPalette && TargetAsset != null)
                {
                    AddLog($"检测到关联对: 自动开始顺序解密 (Auto-pairing detected: sequential decryption starting)...");
                    await DecryptAsset(SelectedAsset);
                    await DecryptAsset(TargetAsset);
                    AddLog("顺序解密任务完成 (Sequential decryption task completed).");
                }
                else
                {
                    await DecryptAsset(SelectedAsset);
                }
            }
            catch (Exception ex)
            {
                AddLog($"异常 (Exception): {ex.Message}");
            }
        }

        private async Task DecryptAsset(AssetInst asset)
        {
            if (asset == null || asset.AssetName == null) return;
            try
            {
                AddLog($"开始解密 (Decrypting): {asset.AssetName}...");
                
                // Get raw encrypted texture data (Inline logic from TextureHelper)
                var textureTemp = _workspace.GetTemplateField(asset);
                var image_data = textureTemp.Children.FirstOrDefault(f => f.Name == "image data");
                if (image_data == null)
                {
                    AddLog("错误: 无法找到 image data 字段 (Error: image data field missing)");
                    return;
                }
                
                // Force ByteArray for reading
                image_data.ValueType = AssetValueType.ByteArray;
                var m_PlatformBlob = textureTemp.Children.FirstOrDefault(f => f.Name == "m_PlatformBlob");
                if (m_PlatformBlob != null)
                {
                    var m_PlatformBlob_Array = m_PlatformBlob.Children[0];
                    m_PlatformBlob_Array.ValueType = AssetValueType.ByteArray;
                }

                AssetTypeValueField baseField;
                lock (asset.FileInstance.LockReader)
                {
                    baseField = textureTemp.MakeValue(asset.FileReader, asset.AbsoluteByteStart);
                }

                var texFile = TextureFile.ReadTextureFile(baseField);

                // Read bytes
                byte[]? encryptedBytes = null;
                var imageDataField = baseField["image data"];
                if (imageDataField != null && !imageDataField.IsDummy && imageDataField.AsByteArray.Length > 0)
                {
                     encryptedBytes = imageDataField.AsByteArray;
                }
                else
                {
                    // Try stream data
                    if (texFile.m_StreamData.size != 0 && !string.IsNullOrEmpty(texFile.m_StreamData.path))
                    {
                        string rootPath = Path.GetDirectoryName(asset.FileInstance.path) ?? "";
                        string fixedStreamPath = texFile.m_StreamData.path;
                        if (asset.FileInstance.parentBundle == null && fixedStreamPath.StartsWith("archive:/"))
                        {
                            fixedStreamPath = Path.GetFileName(fixedStreamPath);
                        }
                        if (!Path.IsPathRooted(fixedStreamPath) && !string.IsNullOrEmpty(rootPath))
                        {
                            fixedStreamPath = Path.Combine(rootPath, fixedStreamPath);
                        }
                        
                        if (File.Exists(fixedStreamPath))
                        {
                            using (var fs = File.OpenRead(fixedStreamPath))
                            {
                                fs.Position = (long)texFile.m_StreamData.offset;
                                encryptedBytes = new byte[texFile.m_StreamData.size];
                                await fs.ReadAsync(encryptedBytes, 0, (int)texFile.m_StreamData.size);
                            }
                        }
                    }
                }

                if (encryptedBytes == null || encryptedBytes.Length == 0)
                {
                    AddLog("错误: 纹理数据为空 (Error: Texture data is empty)");
                    return;
                }

                AddLog($"读取到 ({encryptedBytes.Length} bytes) 加密数据. 纹理格式: {texFile.m_TextureFormat}");

                // Key Generation (ShellProtector KeyGenerator Logic)
                byte[] key = new byte[16];
                byte[] fixedKeyBytes = Encoding.ASCII.GetBytes("password");
                byte[] userKeyBytes = Encoding.ASCII.GetBytes(Password);
                byte[] hash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    hash = sha256.ComputeHash(userKeyBytes);
                }

                for (int i = 0; i < fixedKeyBytes.Length && i < 16; ++i)
                    key[i] = fixedKeyBytes[i];

                int currentKeyLength = KeyLength; // 4, 8, 12, 16 
                if (currentKeyLength > 0)
                {
                    for (int i = (16 - currentKeyLength), j = 0; i < key.Length; ++i, ++j)
                    {
                        if (j < userKeyBytes.Length)
                            key[i] = (byte)(userKeyBytes[j] ^ hash[j]);
                        else
                            key[i] = hash[j];
                    }
                }

                // string keyStr = string.Join("\n", key.Select((b, index) => $"pkey{index}={b}"));
                // AddLog($"生成密钥 (Key Bytes):\n{keyStr}");
                
                AddLog("正在解密 (Decrypting)...");
                byte[] decryptedBytes = new byte[encryptedBytes.Length];
                Array.Copy(encryptedBytes, decryptedBytes, encryptedBytes.Length);
                bool unsupportedFormat = false;

                await Task.Run(() =>
                {
                    uint[] key_uint = new uint[4];
                    key_uint[0] = (uint)(key[0] | (key[1] << 8) | (key[2] << 16) | (key[3] << 24));
                    key_uint[1] = (uint)(key[4] | (key[5] << 8) | (key[6] << 16) | (key[7] << 24));
                    key_uint[2] = (uint)(key[8] | (key[9] << 8) | (key[10] << 16) | (key[11] << 24));
                    key_uint[3] = 0;

                    if (texFile.m_TextureFormat == 4 || texFile.m_TextureFormat == 5) // TextureFormat.RGBA32 / ARGB32
                    {
                        for (int i = 0; i < decryptedBytes.Length / 4; i += 2)
                        {
                            if (i * 4 + 7 >= decryptedBytes.Length) break;
                            
                            uint[] key_block = (uint[])key_uint.Clone();
                            key_block[3] = (uint)(key[12] | (key[13] << 8) | (key[14] << 16) | (key[15] << 24));
                            key_block[3] ^= (uint)i;

                            uint[] data = new uint[2];
                            data[0] = BitConverter.ToUInt32(decryptedBytes, i * 4);
                            data[1] = BitConverter.ToUInt32(decryptedBytes, (i + 1) * 4);

                            uint[] data_dec;
                            if (SelectedEncryptionModeIndex == 1) // ChaCha20
                                data_dec = ChaCha20DecryptSingleBlock(data, key_block, (uint)Nonce0, (uint)Nonce1, (uint)Nonce2);
                            else
                                data_dec = XXTEA.Decrypt(data, key_block, EncryptionRounds);

                            Array.Copy(BitConverter.GetBytes(data_dec[0]), 0, decryptedBytes, i * 4, 4);
                            Array.Copy(BitConverter.GetBytes(data_dec[1]), 0, decryptedBytes, (i + 1) * 4, 4);
                        }
                        
                        _lastDecryptedPalette = (byte[])decryptedBytes.Clone();
                    }
                    else if (texFile.m_TextureFormat == 3) // TextureFormat.RGB24
                    {
                        for (int i = 0; i < decryptedBytes.Length / 4; i += 3)
                        {
                            if (i * 4 + 11 >= decryptedBytes.Length) break;

                            uint[] key_block = (uint[])key_uint.Clone();
                            key_block[3] = (uint)(key[12] | (key[13] << 8) | (key[14] << 16) | (key[15] << 24));
                            key_block[3] ^= (uint)(i * 4 / 3); // Map uint index back to pixel index (4 pixels = 3 uints)

                            uint[] data = new uint[3];
                            data[0] = BitConverter.ToUInt32(decryptedBytes, i * 4);
                            data[1] = BitConverter.ToUInt32(decryptedBytes, (i + 1) * 4);
                            data[2] = BitConverter.ToUInt32(decryptedBytes, (i + 2) * 4);

                            uint[] data_dec;
                            if (SelectedEncryptionModeIndex == 1) // ChaCha20
                                data_dec = ChaCha20DecryptSingleBlock(data, key_block, (uint)Nonce0, (uint)Nonce1, (uint)Nonce2);
                            else
                                data_dec = XXTEA.Decrypt(data, key_block, EncryptionRounds);

                            Array.Copy(BitConverter.GetBytes(data_dec[0]), 0, decryptedBytes, i * 4, 4);
                            Array.Copy(BitConverter.GetBytes(data_dec[1]), 0, decryptedBytes, (i + 1) * 4, 4);
                            Array.Copy(BitConverter.GetBytes(data_dec[2]), 0, decryptedBytes, (i + 2) * 4, 4);
                        }

                        _lastDecryptedPalette = (byte[])decryptedBytes.Clone();
                    }
                    else if (texFile.m_TextureFormat == 10) // TextureFormat.DXT1
                    {
                        if (_lastDecryptedPalette == null)
                        {
                            unsupportedFormat = true;
                            Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog("错误: 解密 DXT1 贴图之前，请先选中并解密包含原色彩补丁的副贴图."));
                        }
                        else
                        {
                            for (int i = 0; i < decryptedBytes.Length / 8; i++)
                            {
                                int dxtOffset = i * 8;
                                int palOffset = i * 4;
                                if (palOffset + 3 >= _lastDecryptedPalette.Length) break;
                                
                                decryptedBytes[dxtOffset + 0] = _lastDecryptedPalette[palOffset + 0];
                                decryptedBytes[dxtOffset + 1] = _lastDecryptedPalette[palOffset + 1];
                                decryptedBytes[dxtOffset + 2] = _lastDecryptedPalette[palOffset + 2];
                                decryptedBytes[dxtOffset + 3] = _lastDecryptedPalette[palOffset + 3];
                            }
                        }
                    }
                    else if (texFile.m_TextureFormat == 12) // TextureFormat.DXT5
                    {
                        if (_lastDecryptedPalette == null)
                        {
                            unsupportedFormat = true;
                            Avalonia.Threading.Dispatcher.UIThread.Post(() => AddLog("错误: 解密 DXT5 贴图之前，请先选中并解密包含原色彩补丁的副贴图."));
                        }
                        else
                        {
                            for (int i = 0; i < decryptedBytes.Length / 16; i++)
                            {
                                int dxtOffset = i * 16;
                                int palOffset = i * 4;
                                if (palOffset + 3 >= _lastDecryptedPalette.Length) break;
                                
                                decryptedBytes[dxtOffset + 8] = _lastDecryptedPalette[palOffset + 0];
                                decryptedBytes[dxtOffset + 9] = _lastDecryptedPalette[palOffset + 1];
                                decryptedBytes[dxtOffset + 10] = _lastDecryptedPalette[palOffset + 2];
                                decryptedBytes[dxtOffset + 11] = _lastDecryptedPalette[palOffset + 3];
                            }
                        }
                    }
                    else
                    {
                        unsupportedFormat = true;
                    }

                    if (!unsupportedFormat)
                    {
                        int pixelSize = (texFile.m_TextureFormat == 4 || texFile.m_TextureFormat == 5) ? 4 : (texFile.m_TextureFormat == 3 ? 3 : 0);
                        if (pixelSize > 0)
                        {
                            SwizzlePixelData(decryptedBytes, pixelSize, ChannelOrder);
                        }
                    }
                });
                
                if (unsupportedFormat)
                {
                     AddLog($"警告: 当前纹理格式 {(TextureFormat)texFile.m_TextureFormat} 不受支持。");
                }

                var imgField = baseField["image data"];
                if (imgField != null && !imgField.IsDummy && imgField.AsByteArray.Length > 0)
                {
                    imgField.AsByteArray = decryptedBytes;
                }
                else
                {
                    var streamData = baseField["m_StreamData"];
                    if (streamData != null && !streamData.IsDummy)
                    {
                        streamData["offset"].AsLong = 0;
                        streamData["size"].AsLong = 0;
                        streamData["path"].AsString = "";
                    }
                    if (imgField != null && !imgField.IsDummy)
                    {
                        imgField.TemplateField.ValueType = AssetValueType.ByteArray;
                        imgField.AsByteArray = decryptedBytes;
                    }
                }
                
                asset.UpdateAssetDataAndRow(_workspace, baseField);
                AddLog($"[成功] {asset.AssetName} 解密完成.");
            }
            catch (Exception ex)
            {
                AddLog($"[异常] {asset.AssetName}: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ExportPNG()
        {
            if (SelectedAsset == null) return;
            if (_workspace == null)
            {
                AddLog("错误:工作区未连接 (Error: Workspace not connected)");
                return;
            }

            try
            {
                AddLog("正在导出 PNG (Exporting to PNG)...");

                var textureTemp = _workspace.GetTemplateField(SelectedAsset);
                var image_data = textureTemp.Children.FirstOrDefault(f => f.Name == "image data");
                if (image_data != null) image_data.ValueType = AssetValueType.ByteArray;
                
                var m_PlatformBlob = textureTemp.Children.FirstOrDefault(f => f.Name == "m_PlatformBlob");
                if (m_PlatformBlob != null)
                {
                    var m_PlatformBlob_Array = m_PlatformBlob.Children[0];
                    m_PlatformBlob_Array.ValueType = AssetValueType.ByteArray;
                }

                AssetTypeValueField baseField;
                lock (SelectedAsset.FileInstance.LockReader)
                {
                    baseField = textureTemp.MakeValue(SelectedAsset.FileReader, SelectedAsset.AbsoluteByteStart);
                }

                byte[]? textureBytes = null;
                var imageDataField = baseField["image data"];
                if (imageDataField != null && !imageDataField.IsDummy && imageDataField.AsByteArray.Length > 0)
                {
                     textureBytes = imageDataField.AsByteArray;
                }
                else
                {
                    var texFileDummy = TextureFile.ReadTextureFile(baseField);
                    if (texFileDummy.m_StreamData.size != 0 && !string.IsNullOrEmpty(texFileDummy.m_StreamData.path))
                    {
                        string rootPath = Path.GetDirectoryName(SelectedAsset.FileInstance.path) ?? "";
                        string fixedStreamPath = texFileDummy.m_StreamData.path;
                        if (SelectedAsset.FileInstance.parentBundle == null && fixedStreamPath.StartsWith("archive:/"))
                        {
                            fixedStreamPath = Path.GetFileName(fixedStreamPath);
                        }
                        if (!Path.IsPathRooted(fixedStreamPath) && !string.IsNullOrEmpty(rootPath))
                        {
                            fixedStreamPath = Path.Combine(rootPath, fixedStreamPath);
                        }
                        
                        if (File.Exists(fixedStreamPath))
                        {
                            using (var fs = File.OpenRead(fixedStreamPath))
                            {
                                fs.Position = (long)texFileDummy.m_StreamData.offset;
                                textureBytes = new byte[texFileDummy.m_StreamData.size];
                                await fs.ReadAsync(textureBytes, 0, (int)texFileDummy.m_StreamData.size);
                            }
                        }
                    }
                }

                if (textureBytes == null || textureBytes.Length == 0)
                {
                    AddLog("错误: 纹理数据为空 (Error: Texture data is empty)");
                    return;
                }

                TextureFile tf = TextureFile.ReadTextureFile(baseField);
                // Decode as raw (usually BGRA from AssetsTools)
                byte[] decodedRaw = tf.DecodeTextureRaw(textureBytes, false);

                if (decodedRaw != null && decodedRaw.Length > 0)
                {
                    // Apply custom channel order swizzling
                    SwizzlePixelData(decodedRaw, 4, ChannelOrder);
                }

                if (decodedRaw == null || decodedRaw.Length == 0)
                {
                    AddLog("错误: 无法解码纹理 (Error: Cannot decode texture)");
                    return;
                }

                using (var image = SixLabors.ImageSharp.Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(decodedRaw, tf.m_Width, tf.m_Height))
                {
                    if (SelectedGammaModeIndex > 0)
                    {
                        float gamma = SelectedGammaModeIndex == 1 ? 2.2f : 0.4545f;
                        // Offload heavy math to avoid hanging the Avalonia UI thread
                        await Task.Run(() =>
                        {
                            image.Mutate(ctx => ctx.ProcessPixelRowsAsVector4(row =>
                            {
                                for (int i = 0; i < row.Length; i++)
                                {
                                    var p = row[i];
                                    float r = (float)Math.Pow(p.X, gamma);
                                    float g = (float)Math.Pow(p.Y, gamma);
                                    float b = (float)Math.Pow(p.Z, gamma);
                                    row[i] = new Vector4(r, g, b, p.W);
                                }
                            }));
                        });
                    }

                    // To memory stream to get byte array
                    using (var ms = new MemoryStream())
                    {
                        await image.SaveAsPngAsync(ms);
                        await SaveDecryptedFile(ms.ToArray(), $"{SelectedAsset.AssetName}_{DateTime.Now:yyyyMMdd_HHmmss}", ".png");
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog($"异常 (Exception): {ex.Message}");
            }
        }

        private async Task SaveDecryptedFile(byte[] data, string defaultName, string extension)
        {
             await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
             {
                var storageProvider = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop 
                    ? desktop.MainWindow?.StorageProvider 
                    : null;

                if (storageProvider == null)
                {
                     // Fallback to desktop if no UI provider
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string outputFolder = Path.Combine(desktopPath, "UABEA_Decrypted");
                    Directory.CreateDirectory(outputFolder);
                    string outputPath = Path.Combine(outputFolder, defaultName + extension);
                    await File.WriteAllBytesAsync(outputPath, data);
                    AddLog($"已保存到 (Saved to Desktop): {outputPath}");
                    return;
                }

                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "保存解密纹理 (Save Decrypted Texture)",
                    SuggestedFileName = defaultName + extension,
                    DefaultExtension = extension.TrimStart('.'),
                    FileTypeChoices = new List<FilePickerFileType>
                    {
                        new FilePickerFileType($"Image/Binary ({extension})") { Patterns = new[] { "*" + extension } },
                        new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                    }
                });

                if (file != null)
                {
                    using var stream = await file.OpenWriteAsync();
                    await stream.WriteAsync(data, 0, data.Length);
                    AddLog($"已保存 (Saved): {file.Path}");
                }
                else
                {
                    AddLog("保存已取消 (Save Cancelled)");
                }
             });
        }

        private void SwizzlePixelData(byte[] data, int pixelSize, string order)
        {
            if (string.IsNullOrEmpty(order) || order.Length < pixelSize) return;
            
            // "RGBA" -> no change if pixelSize is 4
            // "GRBA" -> swap index 0 and 1
            
            byte[] temp = new byte[pixelSize];
            string upperOrder = order.ToUpper();
            
            for (int i = 0; i < data.Length; i += pixelSize)
            {
                if (i + pixelSize > data.Length) break;
                
                Array.Copy(data, i, temp, 0, pixelSize);
                
                for (int j = 0; j < pixelSize; j++)
                {
                    char c = upperOrder[j];
                    int targetIdx = -1;
                    if (c == 'R') targetIdx = 0;
                    else if (c == 'G') targetIdx = 1;
                    else if (c == 'B') targetIdx = 2;
                    else if (c == 'A') targetIdx = 3;
                    
                    if (targetIdx != -1 && targetIdx < pixelSize)
                    {
                        data[i + targetIdx] = temp[j];
                    }
                }
            }
        }

        public static class XXTEA
        {
            public static byte[] Decrypt(byte[] data, byte[] key)
            {
                if (data.Length == 0) return data;
                return ToByteArray(Decrypt(ToUInt32Array(data, false), ToUInt32Array(key, false)), false);
            }

            public static uint[] Decrypt(uint[] v, uint[] k, int rounds = 20)
            {
                int n = v.Length;
                if (n < 2) return v;
                uint z, y = v[0], sum, e;
                int p, q = rounds; // Adjustable rounds
                sum = unchecked((uint)(q * 0x9E3779B9));
                while (sum != 0)
                {
                    e = (sum >> 2) & 3;
                    for (p = n - 1; p > 0; p--)
                    {
                        z = v[p - 1];
                        y = v[p] = unchecked(v[p] - (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (k[(p & 3) ^ e] ^ z))));
                    }
                    z = v[n - 1];
                    y = v[0] = unchecked(v[0] - (((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (k[(p & 3) ^ e] ^ z))));
                    sum = unchecked(sum - 0x9E3779B9);
                }
                return v;
            }

            private static uint[] ToUInt32Array(byte[] data, bool includeLength)
            {
                int length = data.Length;
                int n = (((length & 3) == 0) ? (length >> 2) : ((length >> 2) + 1));
                uint[] result;
                if (includeLength)
                {
                    result = new uint[n + 1];
                    result[n] = (uint)length;
                }
                else
                {
                    result = new uint[n];
                }
                for (int i = 0; i < length; i++)
                {
                    result[i >> 2] |= (uint)data[i] << ((i & 3) << 3);
                }
                return result;
            }

            private static byte[] ToByteArray(uint[] data, bool includeLength)
            {
                int n = data.Length << 2;
                if (includeLength)
                {
                    int m = (int)data[data.Length - 1];
                    n -= 4;
                    if ((m < n - 3) || (m > n))
                    {
                        return Array.Empty<byte>();
                    }
                    n = m;
                }
                byte[] result = new byte[n];
                for (int i = 0; i < n; i++)
                {
                    result[i] = (byte)((data[i >> 2] >> ((i & 3) << 3)) & 0xff);
                }
                return result;
            }
        }

        #region ChaCha20 Algorithm (Ported from ShellProtector)

        private uint[] ChaCha20DecryptSingleBlock(uint[] data, uint[] key_uint4, uint n0, uint n1, uint n2)
        {
            uint[] s = new uint[16];
            uint[] block = new uint[16];

            // Chacha20Init
            s[0] = 0x61707865;
            s[1] = 0x3320646e;
            s[2] = 0x79622d32;
            s[3] = 0x6b206574;

            for (int i = 0; i < 4; i++)
            {
                s[4 + i] = key_uint4[i];
                s[8 + i] = s[4 + i];
            }

            s[12] = 1; // counter = 1
            s[13] = n0;
            s[14] = n1;
            s[15] = n2;

            Array.Copy(s, 0, block, 0, 16);

            for (int i = 8; i > 0; i -= 2) // ShellProtector uses 8 rounds fixed
            {
                ChaCha20QuarterRound(block, 0, 4, 8, 12);
                ChaCha20QuarterRound(block, 1, 5, 9, 13);
                ChaCha20QuarterRound(block, 2, 6, 10, 14);
                ChaCha20QuarterRound(block, 3, 7, 11, 15);
                ChaCha20QuarterRound(block, 0, 5, 10, 15);
                ChaCha20QuarterRound(block, 1, 6, 11, 12);
                ChaCha20QuarterRound(block, 2, 7, 8, 13);
                ChaCha20QuarterRound(block, 3, 4, 9, 14);
            }

            for (int i = 0; i < 16; i++) block[i] += s[i];

            uint[] result = new uint[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i] ^ block[i];
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChaCha20QuarterRound(uint[] x, int a, int b, int c, int d)
        {
            x[a] += x[b]; x[d] = Rotl32(x[d] ^ x[a], 16);
            x[c] += x[d]; x[b] = Rotl32(x[b] ^ x[c], 12);
            x[a] += x[b]; x[d] = Rotl32(x[d] ^ x[a], 8);
            x[c] += x[d]; x[b] = Rotl32(x[b] ^ x[c], 7);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint Rotl32(uint x, int n) => x << n | (x >> (-n & 31));

        #endregion
    }
}