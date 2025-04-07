# 変更履歴

- CHANGE
  - 下位互換のない変更
- UPDATE
  - 下位互換がある変更
- ADD
  - 下位互換がある追加
- FIX
  - バグ修正

## develop

- [CHANGE] Sora Unity SDK のバージョンを 2025.2.0-canary.1 にあげる
  - @miosakuma
- [CHANGE] Editor のバージョンを 6000.0.38f1 にあげる
  - @miosakuma
- [CHANGE] useHardwareEncoder の設定を削除する
  - @torikizi
- [UPDATE] SoraSample.cs に `VideoCodecPreference` の設定を追加
  - 従来通り、使用可能なハードウェアエンコーダーを自動で優先的に使用する挙動を維持する
  - `Sora.GetVideoCodecCapability()` で取得したコーデック情報をもとに、`Sora.VideoCodecPreference.GetHardwareAcceleratorPreference()` を使用して利用するエンコーダーとデコーダーを指定する
  - @torikizi
- [ADD] enableVideoCodecType チェックボックスを追加
  - @miosakuma
- [ADD] enableAudioCodecType チェックボックスを追加
  - @miosakuma

### misc

- [UPDATE] SoraSample.cs の改行コードを LF に統一する
  - @torikizi

## sora-unity-sdk-2025.1.0

**リリース日**: 2025-01-29

- [ADD] forwardingfilters を設定可能にする
  - @torikizi
- [ADD] forwardingfilter に name と priority を追加
  - @torikizi
- [ADD] datachannel に header を追加
  - @torikizi

## sora-unity-sdk-2024.4.0

- [UPDATE] Sora Unity SDK 2024.4.0 にあげる
  - @miosakuma

## sora-unity-sdk-2024.3.0

- [UPDATE] Sora Unity SDK 2024.3.0 にあげる
  - @miosakuma
- [UPDATE] 対応プラットフォームに Ubuntu 22.04 x86_64 を追加する
  - コード変更はなし、Ubuntu 20.04 版の Unity SDK で Ubuntu 22.04 も動作することを確認した
  - @torikizi

## sora-unity-sdk-2024.2.0

- [CHANGE] Lyra 関連の処理を削除する
  - copysdk.py から Lyra 関連ファイルのコピー処理を削除
  - SoraSample.cs から Lyra 関連の設定およびファイル読み込み処理を削除
  - lyra 関連の meta ファイルを削除
  - .gitignore ファイルから Lyra ファイルの定義を削除
  - @miosakuma
- [CHANGE] Sora Unity SDK に Sora.aar ファイルが追加された対応
  - Sora.aar.meta ファイルを追加する
  - .gitignore に Sora.aar ファイルを追加する
  - @torikizi
- [ADD] ハンズフリー切替ボタンを追加
  - multi_sendrecv シーンにハンズフリーを切り替えられるようにボタンを追加
  - 他のボタンと合わせて一つのボタンで切り替えられるようにしているが、テキストを変更することでボタンを押すとどちらに切り替えられるかを表示するようにしている
  - @torikizi

## sora-unity-sdk-2024.1.0

- [ADD] forwardingfilter に version と metadata を追加
  - @torikizi
- [FIX] forwardingfilter の action をオプションにする
  - @torikizi

## sora-unity-sdk-2023.5.2

- [UPDATE] Sora Unity SDK 2023.5.2 にあげる
  - @torikizi

## sora-unity-sdk-2023.5.1

- [FIX] テクスチャのメモリリークを解消する
  - @melpon

## sora-unity-sdk-2023.5.0

- [UPDATE] Sora Unity SDK 2023.5.0 にあげる
  - @miosakuma

## sora-unity-sdk-2023.4.0

- [CHANGE] sendonly, recvonly シーンを削除する
  - @torikizi
- [CHANGE] プロジェクトを Unity 2022.3 LTS にアップデート
  - @torikizi
- [UPDATE] iOS の Target Version を 13.0 にアップデート
  - @torikizi
- [ADD] noVideoDevice, noAudioDevice の設定を追加する
  - @torikizi
- [ADD] useHardwareEncoder の設定を追加する
  - @torikizi
- [ADD] 接続中のカメラ切り替えボタンを追加する
  - @melpon @torikizi

## sora-unity-sdk-2023.3.0

- [UPDATE] Sora Unity SDK 2023.3.0 にあげる
  - @miosakuma

## sora-unity-sdk-2023.2.0

- [ADD] CodecParams の設定を追加
  - video_vp9_params を追加
  - video_av1_params を追加
  - video_h264_params を追加
  - @torikizi
- [ADD] forwardingfilter の設定を追加
  - @torikizi
- [FIX] signalingNotifyMetadata が JSON 文字列化を再帰的に行っているのを修正
  - @melpon

## sora-unity-sdk-2023.1.0

- [UPDATE] Sora Unity SDK 2023.1.0 にあげる
  - @miosakuma
- [ADD] オーディオコーデックに Lyra を追加
  - @melpon, @torikizi
- [ADD] SignalingNotifyMetadata の設定を追加
  - @torikizi
- [ADD] AudioStreamingLanguageCode の設定を追加
  - @melpon
- [ADD] デバッグバイナリをコピーする機能を追加
  - @melpon
- [ADD] OnSetOffer コールバックを追加
  - @melpon
- [FIX] 開始ボタンを 2 回連続で押した時にクラッシュする不具合を修正
  - 開始、終了ボタンの表示制御を行う
  - @melpon

## sora-unity-sdk-2022.6.0

- [CHANGE] tobiAccessToken の設定を削除
  - @torikizi
- [ADD] 音声と映像のミュートを追加
  - @torikizi

## sora-unity-sdk-2022.5.2

- [UPDATE] Sora Unity SDK 2022.5.2 に上げる
  - @miosakuma

## sora-unity-sdk-2022.5.1

- [UPDATE] Sora Unity SDK 2022.5.1 に上げる
  - @melpon, @miosakuma
- [ADD] Video Fps の設定を追加
  - @miosakuma

## sora-unity-sdk-2022.4.0

- [UPDATE] Sora Unity SDK 2022.4.0 に上げる
  - @miosakuma

## sora-unity-sdk-2022.3.0

- [CHANGE] signalingkey を accesstoken に修正
  - @torikizi

## sora-unity-sdk-2022.2.1

- [UPDATE] copysdk.py のログ出力内容を変更する
  - @melpon
- [FIX] sora が null の時に参照してしまうケースについて修正する
  - @melpon

## sora-unity-sdk-2022.2.0

- [CHANGE] プロジェクトを Unity 2021.3 LTS にアップデート
  - @melpon
- [UPDATE] Sora Unity SDK 2022.2.0 に対応
  - @melpon
- [ADD] HTTP Proxy の設定を追加
  - @melpon
- [ADD] BundleId の設定を追加
  - @melpon
- [ADD] tobiAccessToken の設定を追加
  - @miosakuma

## sora-unity-sdk-2022.1.0-1

- [UPDATE] libboost_json.a.meta を追加する
  - iOS の推奨されるビルド初期設定を行う
  - @torikizi
- [FIX] install.sh と install.ps1 に libboost_json.a をコピーする処理を追加する
  - iOS にてサンプルが動作しない問題に対応
  - @torikizi
