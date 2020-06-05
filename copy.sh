# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと bundle をこのプロジェクトにコピーする
mkdir -p SoraUnitySdkSamples/Assets/SoraUnitySdk
cp ../sora-unity-sdk/Sora/Sora.cs SoraUnitySdkSamples/Assets/SoraUnitySdk

if [ "$1" = "macos" ]; then
  mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos
  rm -rf SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle
  cp -r ../sora-unity-sdk/build/SoraUnitySdk.bundle SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/
elif [ "$1" = "android" ]; then
  mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/
  rm -f SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/libSoraUnitySdk.so
  cp ../sora-unity-sdk/build/android/libSoraUnitySdk.so SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/
  cp ../sora-unity-sdk/_install/android/webrtc/jar/webrtc.jar SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/
else
  echo "$0 <macos|android>"
  exit 1
fi
