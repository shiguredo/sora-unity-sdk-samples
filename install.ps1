# SoraUnitySdk をダウンロードして SoraUnitySdkSamples にインストールする

$ErrorActionPreference = 'Stop'


$SORAUNITYSDK_VERSION = "2022.1.0"

# 一通り掃除
if (Test-Path "SoraUnitySdk.zip") {
    Remove-Item "SoraUnitySdk.zip" -Force
}
if (Test-Path "SoraUnitySdk") {
    Remove-Item "SoraUnitySdk" -Force -Recurse
}

$_URL = "https://github.com/shiguredo/sora-unity-sdk/releases/download/${SORAUNITYSDK_VERSION}/SoraUnitySdk.zip"
$_FILE = "SoraUnitySdk.zip"

# ダウンロードと展開
Invoke-WebRequest -Uri $_URL -OutFile $_FILE
Expand-Archive -Path $_FILE -DestinationPath .

# インストール
function Install-File($src, $dst) {
    $dstdir = Split-Path -parent $dst
    New-Item $dstdir -ItemType Directory -ErrorAction SilentlyContinue
    Copy-Item $src $dst
}
function Copy-File($src, $dstbase) {
    Install-File $src "$dstbase\$src"
}

Push-Location SoraUnitySdk
    $_dstbase="..\SoraUnitySdkSamples\Assets"
    Copy-File SoraUnitySdk\Editor\SoraUnitySdkPostProcessor.cs $_dstbase
    Copy-File SoraUnitySdk\Generated\Jsonif.cs $_dstbase
    Copy-File SoraUnitySdk\Generated\SoraConf.cs $_dstbase
    Copy-File SoraUnitySdk\Generated\SoraConfInternal.cs $_dstbase
    Copy-File SoraUnitySdk\Sora.cs $_dstbase
    Copy-File Plugins\SoraUnitySdk\android\webrtc.jar $_dstbase
    Copy-File Plugins\SoraUnitySdk\android\arm64-v8a\libSoraUnitySdk.so $_dstbase
    Copy-File Plugins\SoraUnitySdk\ios\libwebrtc.a $_dstbase
    Copy-File Plugins\SoraUnitySdk\ios\libSoraUnitySdk.a $_dstbase
    Copy-File Plugins\SoraUnitySdk\ios\libboost_json.a $_dstbase
    Copy-File Plugins\SoraUnitySdk\windows\x86_64\SoraUnitySdk.dll $_dstbase
    New-Item $_dstbase\Plugins\SoraUnitySdk\macos -ItemType Directory -ErrorAction SilentlyContinue
    if (Test-Path "$_dstbase\Plugins\SoraUnitySdk\macos\SoraUnitySdk.bundle") {
        Remove-Item "$_dstbase\Plugins\SoraUnitySdk\macos\SoraUnitySdk.bundle" -Force -Recurse
    }
    Copy-Item Plugins\SoraUnitySdk\macos\SoraUnitySdk.bundle $_dstbase\Plugins\SoraUnitySdk\macos\SoraUnitySdk.bundle -Recurse
Pop-Location

Remove-Item "SoraUnitySdk.zip" -Force
Remove-Item "SoraUnitySdk" -Force -Recurse
