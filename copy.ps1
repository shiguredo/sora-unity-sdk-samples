# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと dll をこのプロジェクトにコピーする
mkdir SoraUnitySdkSamples\Assets\Sora\Plugins\x86_64 -Force
Copy-Item ..\sora-unity-sdk\Sora\Sora.cs SoraUnitySdkSamples\Assets\Sora\
Copy-Item ..\sora-unity-sdk\build\Release\SoraUnitySdk.dll SoraUnitySdkSamples\Assets\Sora\Plugins\x86_64\