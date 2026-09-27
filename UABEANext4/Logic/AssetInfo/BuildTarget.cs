namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo；用于组织与资产信息相关的类型（命名空间原名：UABEANext4.Logic.AssetInfo）

public enum BuildTarget
// 定义公共枚举 BuildTarget；表示 Unity 构建目标的平台枚举（枚举原名：BuildTarget）
{
    // 枚举体开始

    StandaloneOSX = 2,
    // 枚举成员 StandaloneOSX（值 = 2）：表示 macOS 独立构建（原名：StandaloneOSX）；显式从 2 开始以匹配 Unity 内部编号

    StandaloneOSXUniversal,
    // 枚举成员 StandaloneOSXUniversal：表示 macOS 通用二进制（原名：StandaloneOSXUniversal）；值为上一个成员值 +1

    StandaloneOSXIntel,
    // 枚举成员 StandaloneOSXIntel：表示仅 Intel 架构的 macOS 构建（原名：StandaloneOSXIntel）

    StandaloneWindows,
    // 枚举成员 StandaloneWindows：表示 Windows 独立构建（32 位）（原名：StandaloneWindows）

    WebPlayer,
    // 枚举成员 WebPlayer：表示旧版 Unity Web Player 平台（原名：WebPlayer）

    WebPlayerStreamed,
    // 枚举成员 WebPlayerStreamed：表示流式 Web Player（原名：WebPlayerStreamed）

    Wii,
    // 枚举成员 Wii：表示任天堂 Wii 平台（原名：Wii）

    iOS,
    // 枚举成员 iOS：表示苹果 iOS 平台（原名：iOS）

    PS3,
    // 枚举成员 PS3：表示索尼 PlayStation 3 平台（原名：PS3）

    XBOX360,
    // 枚举成员 XBOX360：表示微软 Xbox 360 平台（原名：XBOX360）

    StandaloneBroadcom,
    // 枚举成员 StandaloneBroadcom：表示 Broadcom 平台的独立构建（原名：StandaloneBroadcom）；较少见的目标

    Android,
    // 枚举成员 Android：表示 Android 平台（原名：Android）

    StandaloneGLESEmu,
    // 枚举成员 StandaloneGLESEmu：表示 GLES 模拟器的独立构建（原名：StandaloneGLESEmu）

    StandaloneGLES20Emu,
    // 枚举成员 StandaloneGLES20Emu：表示 GLES2.0 模拟器的独立构建（原名：StandaloneGLES20Emu）

    NaCl,
    // 枚举成员 NaCl：表示 Google Native Client 平台（原名：NaCl）

    StandaloneLinux,
    // 枚举成员 StandaloneLinux：表示 Linux 独立构建（原名：StandaloneLinux）

    Flash,
    // 枚举成员 Flash：表示 Adobe Flash 平台（原名：Flash）；历史遗留平台

    StandaloneWindows64,
    // 枚举成员 StandaloneWindows64：表示 Windows 64 位独立构建（原名：StandaloneWindows64）

    WebGL,
    // 枚举成员 WebGL：表示基于浏览器的 WebGL 构建（原名：WebGL）

    WSAPlayer,
    // 枚举成员 WSAPlayer：表示 Windows Store Apps（通用 Windows 平台）播放器（原名：WSAPlayer）

    WSAPlayerX64,
    // 枚举成员 WSAPlayerX64：表示 64 位 WSA 平台（原名：WSAPlayerX64）

    WSAPlayerARM,
    // 枚举成员 WSAPlayerARM：表示 ARM 架构的 WSA 平台（原名：WSAPlayerARM）

    StandaloneLinux64,
    // 枚举成员 StandaloneLinux64：表示 Linux 64 位独立构建（原名：StandaloneLinux64）

    StandaloneLinuxUniversal,
    // 枚举成员 StandaloneLinuxUniversal：表示 Linux 通用构建（原名：StandaloneLinuxUniversal）

    WP8Player,
    // 枚举成员 WP8Player：表示 Windows Phone 8 平台（原名：WP8Player）

    StandaloneOSXIntel64,
    // 枚举成员 StandaloneOSXIntel64：表示仅 Intel 64 位的 macOS 构建（原名：StandaloneOSXIntel64）

    BlackBerry,
    // 枚举成员 BlackBerry：表示 BlackBerry 平台（原名：BlackBerry）

    Tizen,
    // 枚举成员 Tizen：表示三星 Tizen 平台（原名：Tizen）

    PSP2,
    // 枚举成员 PSP2：表示索尼 PlayStation Vita / PS Vita（原名：PSP2）

    PS4,
    // 枚举成员 PS4：表示索尼 PlayStation 4 平台（原名：PS4）

    PSM,
    // 枚举成员 PSM：表示 PlayStation Mobile（原名：PSM）

    XboxOne,
    // 枚举成员 XboxOne：表示微软 Xbox One 平台（原名：XboxOne）

    SamsungTV,
    // 枚举成员 SamsungTV：表示三星智能电视平台（原名：SamsungTV）

    N3DS,
    // 枚举成员 N3DS：表示任天堂 3DS 平台（原名：N3DS）

    WiiU,
    // 枚举成员 WiiU：表示任天堂 Wii U 平台（原名：WiiU）

    tvOS,
    // 枚举成员 tvOS：表示苹果 tvOS 平台（原名：tvOS）

    Switch,
    // 枚举成员 Switch：表示任天堂 Switch 平台（原名：Switch）

    Lumin,
    // 枚举成员 Lumin：表示 Magic Leap Lumin 平台（原名：Lumin）

    Stadia,
    // 枚举成员 Stadia：表示 Google Stadia 云游戏平台（原名：Stadia）

    CloudRendering,
    // 枚举成员 CloudRendering：表示云渲染目标（原名：CloudRendering）

    GameCoreXboxSeries,
    // 枚举成员 GameCoreXboxSeries：表示 Xbox Series 的 GameCore 平台（原名：GameCoreXboxSeries）

    GameCoreXboxOne,
    // 枚举成员 GameCoreXboxOne：表示 Xbox One 的 GameCore 平台（原名：GameCoreXboxOne）

    PS5,
    // 枚举成员 PS5：表示索尼 PlayStation 5 平台（原名：PS5）

    EmbeddedLinux,
    // 枚举成员 EmbeddedLinux：表示嵌入式 Linux 平台（原名：EmbeddedLinux）

    QNX,
    // 枚举成员 QNX：表示 QNX 实时操作系统平台（原名：QNX）

    Bratwurst
    // 枚举成员 Bratwurst：表示一个名为 Bratwurst 的目标（原名：Bratwurst）；可能是内部/占位或特定平台代号
}
// 枚举体结束（BuildTarget）
