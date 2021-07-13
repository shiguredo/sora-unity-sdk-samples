# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと dll をこのプロジェクトにコピーする
mkdir SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64 -Force
if (Test-Path "SoraUnitySdkSamples\Assets\SoraUnitySdk") {
    Remove-Item "SoraUnitySdkSamples\Assets\SoraUnitySdk" -Force -Recurse
}
Copy-Item -Recurse ..\sora-unity-sdk\Sora\ SoraUnitySdkSamples\Assets\SoraUnitySdk\
Copy-Item ..\sora-unity-sdk\build\Release\SoraUnitySdk.dll SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\
#Copy-Item ..\sora-unity-sdk\build\Release\SoraUnitySdk.exp SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\
#Copy-Item ..\sora-unity-sdk\build\Debug\SoraUnitySdk.dll SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\
#Copy-Item ..\sora-unity-sdk\build\Debug\SoraUnitySdk.pdb SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\
#Copy-Item ..\sora-unity-sdk\build\Debug\SoraUnitySdk.exp SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\
