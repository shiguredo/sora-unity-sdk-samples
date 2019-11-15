# Sora Unity SDK サンプル集

このリポジトリには、 [Sora Unity SDK](https://github.com/shiguredo/sora-unity-sdk) を利用したサンプルアプリを掲載しています。実際の利用シーンに即したサンプルをご用意しておりますので、目的に応じた Sora Unity SDK の使い方を簡単に学ぶことができます。

## About Support

We check PRs or Issues only when written in JAPANESE.
In other languages, we won't be able to deal with them. Thank you for your understanding.

## サポートについて

Sora Uinty SDK サンプル集に関する質問・要望・バグなどの報告は Issues の利用をお願いします。
ただし、 Sora のライセンス契約の有無に関わらず、 Issue への応答時間と問題の解決を保証しませんのでご了承ください。

Sora Unity SDK サンプル集に対する有償のサポートについては現在提供しておりません。

## システム条件

- Unity 2019.1

## sora-unity-sdk のインストール

`install.bat` を実行して下さい。
これで各種サンプルを実行するために必要になる sora-unity-sdk をインストールできます。

## サンプルと実行方法

各サンプルはシーンとして用意しています。
実行したいサンプルを "Assets > Scene" から選んでください。

- `pub`: シングルストリーム送信
- `sub`: シングルストリーム受信
- `multi_pubsub`: マルチストリーム送受信
- `multi_sub`: マルチストリーム受信

プレイモードを実行し、ゲームビュー内に表示される「開始」ボタンを押すと Sora サーバーに接続します。
映像が描画されない場合は、シグナリング URL やコンソールの出力を確認してみてください。

## 接続先の設定

`SoraUnitySdkSamples/.env.json` を編集してください。

- `signaling_url`: シグナリング URL
- `channel_id`: チャネル ID

## ライセンス

Apache License 2.0

```
Copyright 2019, Shiguredo Inc, melpon and kdxu

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

