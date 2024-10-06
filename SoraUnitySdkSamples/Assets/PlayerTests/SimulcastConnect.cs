using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimulcastConnect
{
    [UnityTest]
    public IEnumerator SimulcastConnectWithEnumeratorPasses()
    {
        // multi_sendrecv シーンを非同期にロード
        var loadSceneOperation = SceneManager.LoadSceneAsync("multi_sendrecv");

        // シーンのロードが完了するまで待機
        while (!loadSceneOperation.isDone)
        {
            yield return null; // 次のフレームまで待機
        }

        // シーンが正しくロードされたか確認
        Assert.AreEqual("multi_sendrecv", SceneManager.GetActiveScene().name, "Scene did not load correctly!");

        // SoraSample スクリプトを探して simulcast を有効にする
        SoraSample script = GameObject.Find("Script").GetComponent<SoraSample>();
        if (script != null)
        {
            // simulcast を有効にする
            script.simulcast = true;
        }

        // 接続ボタンを探して押す
        Button firstButton = GameObject.Find("ButtonStart").GetComponent<Button>();
        Assert.IsNotNull(firstButton, "FirstButton was not found in the scene!");

        // ボタンをクリック（シミュレート）
        firstButton.onClick.Invoke();

        // 20 秒待機
        yield return new WaitForSeconds(20);

        // 切断ボタンを探して押す
        Button secondButton = GameObject.Find("ButtonEnd").GetComponent<Button>();
        Assert.IsNotNull(secondButton, "SecondButton was not found in the scene!");

        // ボタンをクリック（シミュレート）
        secondButton.onClick.Invoke();

        // 追加の確認やボタンを押した後の状態確認
        // 必要に応じてアサーションをここに追加
        // Assert for checking after second button press logic

        yield return null; // テストを終了
    }
}
