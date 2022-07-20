# Sora Unity SDK サンプル集

このリポジトリには、 [Sora Unity SDK](https://github.com/shiguredo/sora-unity-sdk) を利用したサンプルアプリを掲載しています。実際の利用シーンに即したサンプルをご用意しておりますので、目的に応じた Sora Unity SDK の使い方を簡単に学ぶことができます。

## About Support

We check PRs or Issues only when written in JAPANESE.
In other languages, we won't be able to deal with them. Thank you for your understanding.

## 時雨堂のオープンソースソフトウェアについて

利用前に https://github.com/shiguredo/oss をお読みください。

## 対応 Unity バージョン

- Unity 2021.3 (LTS)

## 対応 Sora バージョン

- Sora 2022.1.1

## 対応 プラットフォーム

- Windows 10 1809 x86_64 以降
- macOS 12.4 M1 以降
- Android 7 以降
- iOS 12 以降
- Ubuntu 20.04 x86_64 以降

## Sora Unity SDK サンプル集を使ってみる

### 準備するもの

- Unity 開発環境 ([対応 Unity バージョン](#対応-unity-バージョン))
- サーバー (Sora, Tobi, Sora Labo) への接続情報  ([対応 Sora バージョン](#対応-sora-バージョン))
- Python 3

### sora-unity-sdk のインストール

1. [master ブランチ](https://github.com/shiguredo/sora-unity-sdk-samples/tree/master) をダウンロード、もしくはクローンして利用してください。
    develop ブランチは開発ブランチであり、正常に動作しないことがあります。

2. ダウンロードされたディレクトリに移動して、以下コマンドを実行して下さい。
    各種サンプルを実行するために必要になる sora-unity-sdk をインストールできます。[^1] 利用している sora-unity-sdk のバージョンは install.py に定義されています。

```
python3 install.py
```

[^1]: `--sdk-path` で `SoraUnitySdk` ディレクトリを指定することでローカルにダウンロードした sora-unity-sdk をインストールすることができます。

### Unity Editor 上でサンプルを実行する

1. Unity の開発環境で、 `SoraUnitySdkSamples` を指定してプロジェクトを開きます。

2. `Assets > Scene` から実行するサンプルシーンを選択します。

   - `sendonly`: シングルストリーム送信
   - `recvonly`: シングルストリーム受信
   - `multi_sendrecv`: マルチストリーム送受信
   - `multi_sendonly`: マルチストリーム送信
   - `multi_recvonly`: マルチストリーム受信

3. Script オブジェクトのインスペクターにサーバーに送信する情報を設定します
    `Signaling Url` と `Channel Id` は必須です。それ以外の設定は各環境に応じて設定してください。
    設定内容については [Sora のドキュメント](https://sora-doc.shiguredo.jp/SIGNALING) も参考にしてください。

   - `Signaling Url`: シグナリング URL
   - `Channel Id`: チャネル ID
   - `Tobi Access Token`: Tobi 向けのアクセストークン
   - `Signaling Key`: シグナリングキー

4. プレイモードを実行し、ゲームビュー内に表示される「開始」ボタンを押すとサーバーに接続します。
    映像が描画されない場合は、シグナリング URL やコンソールの出力を確認してみてください。

### 諸注意

現在 Android 向けにビルドを行った際に Sora との接続がエラーになる現象が発生しています。
Development Build を選択してビルドを行うと事象を回避できることが確認されています。
サンプルを Android で動作させる場合は Development Build を選択してビルドを行なってください。


## 動作例

- [Sora Unity SDK サンプル集を動かしてみた - torikiziのブログ](https://torikizi.hatenablog.jp/entry/2019/12/03/101411)

[Sora Labo](https://sora-labo.shiguredo.jp/) を使って Sora Unity SDK サンプル集を動かしています。

## ライセンス

Apache License 2.0

```
Copyright 2019-2022, Wandbox LLC (Original Author)
Copyright 2019-2022, Shiguredo Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

