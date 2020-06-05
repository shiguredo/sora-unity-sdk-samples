#!/bin/bash

set -ex

# SoraUnitySdk をダウンロードして SoraUnitySdkSamples にインストールする

SORAUNITYSDK_VERSION="2020.4"

# 一通り掃除
rm -rf SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk
rm -rf SoraUnitySdkSamples/Assets/SoraUnitySdk
rm -f SoraUnitySdk.zip
rm -rf SoraUnitySdk

mkdir -p SoraUnitySdkSamples/Assets/Plugins

_URL="https://github.com/shiguredo/sora-unity-sdk/releases/download/${SORAUNITYSDK_VERSION}/SoraUnitySdk.zip"

# ダウンロードと展開
curl -LO $_URL
unzip SoraUnitySdk.zip -d .

# インストール
cp -r "SoraUnitySdk/Plugins/SoraUnitySdk" "SoraUnitySdkSamples/Assets/Plugins/SoraUnitySdk"
cp -r "SoraUnitySdk/SoraUnitySdk" "SoraUnitySdkSamples/Assets/SoraUnitySdk"

rm -f SoraUnitySdk.zip
rm -rf SoraUnitySdk
