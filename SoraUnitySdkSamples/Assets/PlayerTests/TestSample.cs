using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;


// iOS / macOS のみ。Android は別の問題で失敗するため、テストを一旦スキップ
public class TestSample
{
    SoraSample soraSample;

    [UnityTest]
    public IEnumerator TestConnectAndDisconnect()
    {
        // multi_sendrecv シーンをロードし、ロードが完了するまで待機
        SceneManager.LoadScene("multi_sendrecv");

        // シーンがロードされるまで待機
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "multi_sendrecv");

        // "Script" という名前のGameObjectにアタッチされているSoraSampleを取得
        soraSample = GameObject.Find("Script")?.GetComponent<SoraSample>();
        Assert.IsNotNull(soraSample, "SoraSample コンポーネントがアタッチされていません。");

        // 必要なパラメータを設定
        soraSample.signalingUrl = "wss://examples.com/signaling";
        soraSample.channelId = "test_channel";

        // 接続処理を実行
        soraSample.OnClickStart();

        // 接続が完了するまで少し待機
        yield return new WaitForSeconds(5);

        // プロパティを使って接続確認
        Assert.IsNotNull(soraSample.CurrentSora, "Sora が接続されていません。");
        Assert.AreEqual(SoraSample.State.Started, soraSample.CurrentState, "状態が Started ではありません。");

        // 切断処理を実行
        soraSample.OnClickEnd();

        // 切断が完了するまでより長く待機
        // State.Disconnecting から State.Init に変わるまで待つ
        float waitTime = 0f;
        while (soraSample.CurrentState != SoraSample.State.Init && waitTime < 10f)
        {
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;
        }

        // タイムアウトチェック
        Assert.Less(waitTime, 10f, "切断処理がタイムアウトしました");

        // プロパティを使って切断確認
        Assert.IsNull(soraSample.CurrentSora, "切断後に Sora インスタンスが null になっていません。");
        Assert.AreEqual(SoraSample.State.Init, soraSample.CurrentState, "状態が Init に戻っていません。");
    }
}