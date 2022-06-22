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

- [CHANGE] プロジェクトを Unity 2021.3 LTS にアップデート
    - @melpon
- [UPDATE] Sora Unity SDK 2022.2.0 に対応
    - @melpon
- [ADD] HTTP Proxy の設定を追加
    - @melpon
- [ADD] BundleId の設定を追加
    - @melpon

## sora-unity-sdk-2022.1.0-1

- [UPDATE] libboost_json.a.metaを追加する
    - iOS の推奨されるビルド初期設定を行う
    - @torikizi
- [FIX] install.sh と install.ps1 に libboost_json.a をコピーする処理を追加する
    - iOS にてサンプルが動作しない問題に対応
    - @torikizi
