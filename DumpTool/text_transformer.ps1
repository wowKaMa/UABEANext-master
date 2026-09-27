# ==========================================
# 1. 初始化本地参数与终端流捕获
# ==========================================
$secureA = Read-Host "请输入参数 A" -AsSecureString
$bstrA = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureA)
$passwordA = [Runtime.InteropServices.Marshal]::PtrToStringAuto($bstrA)
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstrA)

$secureB = Read-Host "请输入参数 B" -AsSecureString
$bstrB = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureB)
$passwordB = [Runtime.InteropServices.Marshal]::PtrToStringAuto($bstrB)
[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstrB)

Clear-Host 

# ==========================================
# 2. 定义向量转换与矩阵编解码管道
# ==========================================
function Get-MD5Hex {
    param([string]$inputString)
    $utf8 = [System.Text.Encoding]::UTF8
    $bytes = $utf8.GetBytes($inputString)
    $md5 = [System.Security.Cryptography.MD5]::Create()
    $md5Bytes = $md5.ComputeHash($bytes)
    $md5.Dispose()
    $hex = -join ($md5Bytes | ForEach-Object { $_.ToString("x2") })
    [Array]::Clear($bytes, 0, $bytes.Length)
    [Array]::Clear($md5Bytes, 0, $md5Bytes.Length)
    return $hex
}

function Get-SHA256Hex {
    param([string]$inputString)
    $utf8 = [System.Text.Encoding]::UTF8
    $bytes = $utf8.GetBytes($inputString)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $hashBytes = $sha256.ComputeHash($bytes)
    $sha256.Dispose()
    $hex = -join ($hashBytes | ForEach-Object { $_.ToString("x2") })
    [Array]::Clear($bytes, 0, $bytes.Length)
    [Array]::Clear($hashBytes, 0, $hashBytes.Length)
    return $hex
}

function Get-SHA512Hex {
    param([string]$inputString)
    $utf8 = [System.Text.Encoding]::UTF8
    $bytes = $utf8.GetBytes($inputString)
    $sha512 = [System.Security.Cryptography.SHA512]::Create()
    $hashBytes = $sha512.ComputeHash($bytes)
    $sha512.Dispose()
    $hex = -join ($hashBytes | ForEach-Object { $_.ToString("x2") })
    [Array]::Clear($bytes, 0, $bytes.Length)
    [Array]::Clear($hashBytes, 0, $hashBytes.Length)
    return $hex
}

# ==========================================
# 3. 执行序列映射与多维矩阵对齐计算
# ==========================================
# 参数 A 序列矩阵映射
$md5A = Get-MD5Hex $passwordA
$finalA = Get-SHA256Hex $md5A

# 参数 B 序列矩阵映射
$md5B = Get-MD5Hex $passwordB
$finalB = Get-SHA256Hex $md5B

# 综合原数据流矩阵特征提取
$sha512Raw = Get-SHA512Hex ($passwordA + $passwordB)

# 反向索引复合绑定与特征矩阵合并
$BA_AB = ($finalB + $finalA) + $sha512Raw

# 递归迭代复合特征矩阵求解终态
$md5BA_AB = Get-MD5Hex $BA_AB
$ultimateResult = Get-SHA256Hex $md5BA_AB

# ==========================================
# 4. 输出各阶段分块数据验证矩阵
# ==========================================
Write-Host ""
Write-Host "================ [ 诊断校验与状态对照面板 ] ================" -ForegroundColor Yellow
Write-Host "1. 节点 A 索引校验位 (MD5)   : $md5A" -ForegroundColor Cyan
Write-Host "2. 节点 A 向量映射态         : $finalA (长度: $($finalA.Length))" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. 节点 B 索引校验位 (MD5)   : $md5B" -ForegroundColor Cyan
Write-Host "4. 节点 B 向量映射态         : $finalB (长度: $($finalB.Length))" -ForegroundColor Cyan
Write-Host ""
Write-Host "5. 复合数据流基底映射态      : $sha512Raw (长度: $($sha512Raw.Length))" -ForegroundColor Magenta
Write-Host ""
Write-Host "6. 矩阵综合重组标识 [BA(AB)] : $BA_AB (长度: $($BA_AB.Length))" -ForegroundColor DarkMagenta
Write-Host ""
Write-Host "7. 终态节点校验位 (MD5)      : $md5BA_AB" -ForegroundColor Green
Write-Host "8. 矩阵解析终态目标值        : $ultimateResult (长度: $($ultimateResult.Length))" -ForegroundColor Green
Write-Host "=============================================================" -ForegroundColor Yellow
Write-Host ""

# ==========================================
# 5. 交互式流控分流选择
# ==========================================
Write-Host "【控制流模式选择】" -ForegroundColor Cyan
Write-Host "  - 输入 [yes]    : 输出终端解析终态目标值" -ForegroundColor White
Write-Host "  - 输入 [BA(AB)] : 输出矩阵综合重组标识" -ForegroundColor White
$modeChoice = Read-Host "请指定通道配置类型 (yes / BA(AB))"

if ($modeChoice -ne "yes" -and $modeChoice -ne "BA(AB)") {
    Write-Host "配置参数无效，流控自动中断退出。" -ForegroundColor Red
    exit
}

# ==========================================
# 6. 同步挂起与事件触发准备
# ==========================================
Add-Type -AssemblyName System.Windows.Forms
Write-Host "已锁定通道 [$modeChoice]。请在 5 秒内将输入焦点移至目标控件..." -ForegroundColor Yellow
for ($i = 5; $i -gt 0; $i--) {
    Write-Host "$i..." -NoNewline -ForegroundColor Cyan
    Start-Sleep -Milliseconds 1000
}
Write-Host "`n正在向活动缓冲区投递渲染流..." -ForegroundColor Green

# ==========================================
# 7. 渲染流输出与缓冲区映射
# ==========================================
$targetText = if ($modeChoice -eq "yes") { $ultimateResult } else { $BA_AB }

[System.Windows.Forms.SendKeys]::SendWait($targetText)
Start-Sleep -Milliseconds 200

# ==========================================
# 8. 执行内存垃圾回收与会话残留清理
# ==========================================
[System.Windows.Forms.Clipboard]::Clear()

try {
    $null = [Windows.ApplicationModel.DataTransfer.Clipboard, Windows, ContentType = WindowsRuntime]::ClearHistory()
} catch {
    Set-Clipboard "" 2>$null
}

Remove-Variable passwordA, passwordB, secureA, secureB, bstrA, bstrB, md5A, finalA, md5B, finalB, sha512Raw, BA_AB, md5BA_AB, ultimateResult, modeChoice, targetText -ErrorAction SilentlyContinue
[System.GC]::Collect()
[System.GC]::WaitForPendingFinalizers()

Clear-History -ErrorAction SilentlyContinue
$historyPath = (Get-PSReadLineOption -ErrorAction SilentlyContinue).HistorySavePath
if ($historyPath -and (Test-Path $historyPath)) {
    Clear-Content $historyPath -Force -ErrorAction SilentlyContinue
}

Write-Host "管线任务圆满结束。动态缓冲区与会话中间态已执行安全释放。" -ForegroundColor DarkGray