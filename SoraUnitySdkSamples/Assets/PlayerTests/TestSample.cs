using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine.SceneManagement;

public class TestSample
{
    SoraSample soraSample;
    FieldInfo soraField;
    MethodInfo disposeSoraMethod;

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

        // Reflectionを使ってprivateな'sora'フィールドにアクセス
        soraField = typeof(SoraSample).GetField("sora", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(soraField, "'sora'フィールドが見つかりません。");

        // Reflectionを使ってDisposeSoraメソッドにアクセス
        disposeSoraMethod = typeof(SoraSample).GetMethod("DisposeSora", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(disposeSoraMethod, "'DisposeSora'メソッドが見つかりません。");

        // 必要なパラメータを設定
        soraSample.signalingUrl = "wss://example.com/signaling";
        soraSample.channelId = "test_channel";

        // 接続処理を実行
        soraSample.OnClickStart();

        // 接続が完了するまで少し待機
        yield return new WaitForSeconds(5);

        // Reflectionで'sora'フィールドの値を取得して接続確認
        var soraInstance = soraField.GetValue(soraSample);
        Assert.IsNotNull(soraInstance, "Sora が接続されていません。");

        // 切断処理を実行
        soraSample.OnClickEnd();

        // ReflectionでDisposeSoraメソッドを呼び出してリソースを解放
        disposeSoraMethod.Invoke(soraSample, null);

        // 切断が完了するまで待機
        yield return new WaitForSeconds(5);

        // Reflectionで'sora'フィールドの値がnullになっているか確認
        soraInstance = soraField.GetValue(soraSample);
        Assert.IsNull(soraInstance, "切断後に Sora インスタンスが null になっていません。");
    }
}