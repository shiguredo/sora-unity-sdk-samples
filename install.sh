#!/bin/bash

set -ex

# SoraUnitySdk をダウンロードして SoraUnitySdkSamples にインストールする

SORAUNITYSDK_VERSION="2020.7"

# 掃除
rm -f SoraUnitySdk.zip
rm -rf SoraUnitySdk

_URL="https://github.com/shiguredo/sora-unity-sdk/releases/download/${SORAUNITYSDK_VERSION}/SoraUnitySdk.zip"

# ダウンロードと展開
curl -LO $_URL
unzip SoraUnitySdk.zip -d .

# インストール
function install_file() {
  # コピー先のディレクトリが無ければ、ディレクトリを作ってからコピーする
  _src="$1"
  _dst="$2"
  _dstdir="`dirname $_dst`"
  if [ ! -e "$_dstdir" ]; then
    mkdir -p "$_dstdir"
  fi
  cp $_src $_dst
}
function copy_file() {
  # カレントディレクトリからの相対URLを使って出力ディレクトリに install_file する
  _src="$1"
  _dstbase="$2"
  install_file "$_src" "$_dstbase/$_src"
}
pushd SoraUnitySdk
  _dstbase="../SoraUnitySdkSamples/Assets"
  copy_file SoraUnitySdk/Editor/SoraUnitySdkPostProcessor.cs $_dstbase
  copy_file SoraUnitySdk/Sora.cs $_dstbase
  copy_file Plugins/SoraUnitySdk/android/webrtc.jar $_dstbase
  copy_file Plugins/SoraUnitySdk/android/arm64-v8a/libSoraUnitySdk.so $_dstbase
  copy_file Plugins/SoraUnitySdk/ios/libwebrtc.a $_dstbase
  copy_file Plugins/SoraUnitySdk/ios/libSoraUnitySdk.a $_dstbase
  copy_file Plugins/SoraUnitySdk/windows/x86_64/SoraUnitySdk.dll $_dstbase
  mkdir -p $_dstbase/Plugins/SoraUnitySdk/macos/
  rm -rf $_dstbase/Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle/
  cp -r Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle/ $_dstbase/Plugins/SoraUnitySdk/macos/SoraUnitySdk.bundle
popd

rm -f SoraUnitySdk.zip
rm -rf SoraUnitySdk
