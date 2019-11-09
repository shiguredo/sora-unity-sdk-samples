# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと bundle をこのプロジェクトにコピーする
mkdir -p SoraUnitySdkSamples/Assets/SoraUnitySdk
cp ../sora-unity-sdk/Sora/Sora.cs SoraUnitySdkSamples/Assets/SoraUnitySdk

mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos
rm -rf SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle
cp -r ../sora-unity-sdk/build/SoraUnitySdk.bundle SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/
