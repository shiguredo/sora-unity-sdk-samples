# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと bundle をこのプロジェクトにコピーする
mkdir -p SoraUnitySdkSamples/Assets/SoraUnitySdk
cp ../sora-unity-sdk/Sora/Sora.cs SoraUnitySdkSamples/Assets/SoraUnitySdk
rm -rf SoraUnitySdkSamples/Assets/SoraUnitySdk/Editor/
cp -r ../sora-unity-sdk/Sora/Editor/ SoraUnitySdkSamples/Assets/SoraUnitySdk/Editor/
rm -rf SoraUnitySdkSamples/Assets/SoraUnitySdk/Generated/
cp -r ../sora-unity-sdk/Sora/Generated/ SoraUnitySdkSamples/Assets/SoraUnitySdk/Generated/

if [ "$1" = "macos" ]; then
  mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos
  rm -rf SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle
  cp -r ../sora-unity-sdk/_build/sora-unity-sdk/macos/SoraUnitySdk.bundle SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/macos/
elif [ "$1" = "ios" ]; then
  mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/ios
  rm -rf SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/ios/libSoraUnitySdk.a
  cp -r ../sora-unity-sdk/_build/sora-unity-sdk/ios/libSoraUnitySdk.a SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/ios/
  cp -r ../sora-unity-sdk/_install/ios/webrtc/lib/libwebrtc.a SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/ios/
elif [ "$1" = "android" ]; then
  mkdir -p SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/
  rm -f SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/libSoraUnitySdk.so
  cp ../sora-unity-sdk/_build/sora-unity-sdk/android/libSoraUnitySdk.so SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/arm64-v8a/
  cp ../sora-unity-sdk/_install/android/webrtc/jar/webrtc.jar SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk/android/
else
  echo "$0 <macos|ios|android>"
  exit 1
fi
