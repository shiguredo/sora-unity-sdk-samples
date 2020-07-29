# Sora Unity SDK サンプル集

このリポジトリには、 [Sora Unity SDK](https://github.com/shiguredo/sora-unity-sdk) を利用したサンプルアプリを掲載しています。実際の利用シーンに即したサンプルをご用意しておりますので、目的に応じた Sora Unity SDK の使い方を簡単に学ぶことができます。

## About Support

We check PRs or Issues only when written in JAPANESE.
In other languages, we won't be able to deal with them. Thank you for your understanding.

## Discord

https://discord.gg/pFPQ5pS

Sora Unity SDK サンプルに関する質問・要望などの報告は Disocrd へお願いします。

バグに関してもまずは Discord へお願いします。
ただし、 Sora のライセンス契約の有無に関わらず、 応答時間と問題の解決を保証しませんのでご了承ください。

Sora Unity SDK サンプルに対する有償のサポートについては提供しておりません。

## システム条件

- Unity 2019.1
- Unity 2019.2
- Unity 2019.3
- Unity 2019.4
- Unity 2020.1

## sora-unity-sdk のインストール

Windows の場合は `install.bat` を、macOS の場合は `install.sh` を実行して下さい。
これで各種サンプルを実行するために必要になる sora-unity-sdk をインストールできます。

## サンプルと実行方法

各サンプルはシーンとして用意しています。
実行したいサンプルを "Assets > Scene" から選んでください。

- `sendonly`: シングルストリーム送信
- `recvonly`: シングルストリーム受信
- `multi_sendrecv`: マルチストリーム送受信
- `multi_sendonly`: マルチストリーム送信
- `multi_recvonly`: マルチストリーム受信

プレイモードを実行し、ゲームビュー内に表示される「開始」ボタンを押すと Sora サーバーに接続します。
映像が描画されない場合は、シグナリング URL やコンソールの出力を確認してみてください。

## 接続先の設定

各サンプルのシーンの "Script" オブジェクトのインスペクタで接続先を指定できます。

- `Signaling Url`: シグナリング URL
- `Channel Id`: チャネル ID
- `Signaling Key`: シグナリングキー

## 動作例

- [Sora Unity SDK サンプル集を動かしてみた - torikiziのブログ](https://torikizi.hatenablog.jp/entry/2019/12/03/101411)

[Sora Labo](https://sora-labo.shiguredo.jp/) を使って Sora Unity SDK サンプル集を動かしています。

## ライセンス

Apache License 2.0

```
Copyright 2019-2020, Wandbox LLC (Original Author)
Copyright 2019-2020, Shiguredo Inc

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

