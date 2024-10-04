using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectAndDisconnect
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ConnectAndDisconnectWithEnumeratorPasses()
    {
        // シーンを非同期にロード
        var loadSceneOperation = SceneManager.LoadSceneAsync("multi_sendrecv");

        // シーンのロードが完了するまで待機
        while (!loadSceneOperation.isDone)
        {
            yield return null; // 次のフレームまで待機
        }

        // シーンが正しくロードされたか確認
        Assert.AreEqual("multi_sendrecv", SceneManager.GetActiveScene().name, "Scene did not load correctly!");

        // 1つ目のボタンを探して押す
        Button firstButton = GameObject.Find("ButtonStart").GetComponent<Button>();
        Assert.IsNotNull(firstButton, "FirstButton was not found in the scene!");

        // 1つ目のボタンをクリック（シミュレート）
        firstButton.onClick.Invoke();

        // 数秒待機 (例えば20秒)
        yield return new WaitForSeconds(20);

        // 2つ目のボタンを探して押す
        Button secondButton = GameObject.Find("ButtonEnd").GetComponent<Button>();
        Assert.IsNotNull(secondButton, "SecondButton was not found in the scene!");

        // 2つ目のボタンをクリック（シミュレート）
        secondButton.onClick.Invoke();

        // 追加の確認やボタンを押した後の状態確認
        // 必要に応じてアサーションをここに追加
        // Assert for checking after second button press logic

        yield return null; // フレームをまたいでテストを終了    }
    }
}
