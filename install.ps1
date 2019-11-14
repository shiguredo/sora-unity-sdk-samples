# SoraUnitySdk をダウンロードして SoraUnitySdkSamples にインストールする

$ErrorActionPreference = 'Stop'


$SORAUNITYSDK_VERSION = "0.4.0"

# 一通り掃除
if (Test-Path "SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk") {
    Remove-Item "SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk" -Force -Recurse
}
if (Test-Path "SoraUnitySdkSamples\Assets\SoraUnitySdk") {
    Remove-Item "SoraUnitySdkSamples\Assets\SoraUnitySdk" -Force -Recurse
}
if (Test-Path "SoraUnitySdk.zip") {
    Remove-Item "SoraUnitySdk.zip" -Force
}
if (Test-Path "SoraUnitySdk") {
    Remove-Item "SoraUnitySdk" -Force -Recurse
}

if (!(Test-Path "SoraUnitySdkSamples\Assets\Plugins")) {
    New-Item -Path "SoraUnitySdkSamples\Assets\Plugins" -ItemType Directory
}

$_URL = "https://github.com/shiguredo/sora-unity-sdk/releases/download/${SORAUNITYSDK_VERSION}/SoraUnitySdk.zip"
$_FILE = "SoraUnitySdk.zip"

# ダウンロードと展開
Invoke-WebRequest -Uri $_URL -OutFile $_FILE
Expand-Archive -Path $_FILE -DestinationPath .

# インストール
Copy-Item "SoraUnitySdk\Plugins\SoraUnitySdk" "SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk" -Recurse
Copy-Item "SoraUnitySdk\SoraUnitySdk" "SoraUnitySdkSamples\Assets\SoraUnitySdk" -Recurse

Remove-Item "SoraUnitySdk.zip" -Force
Remove-Item "SoraUnitySdk" -Force -Recurse
