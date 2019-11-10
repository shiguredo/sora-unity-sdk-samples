# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと dll をこのプロジェクトにコピーする
mkdir SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64 -Force
mkdir SoraUnitySdkSamples\Assets\SoraUnitySdk -Force
Copy-Item ..\sora-unity-sdk\Sora\Sora.cs SoraUnitySdkSamples\Assets\SoraUnitySdk\
Copy-Item ..\sora-unity-sdk\build\Release\SoraUnitySdk.dll SoraUnitySdkSamples\Assets\Plugins\SoraUnitySdk\windows\x86_64\