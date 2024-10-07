using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectAndDisconnectAndroid
{
    [UnityTest]
    public IEnumerator ConnectAndDisconnectAndroidWithEnumeratorPasses()
    {
        // 特定のエラーメッセージを無視
        LogAssert.Expect(LogType.Exception, "UnityException: get_gameObject can only be called from the main thread.\nConstructors and field initializers will be executed from the loading thread when loading a scene.\nDon't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.");

        // シーンを非同期にロード
        var loadSceneOperation = SceneManager.LoadSceneAsync("multi_sendrecv");

        // シーンのロードが完了するまで待機
        while (!loadSceneOperation.isDone)
        {
            yield return null;
        }

        // シーンが正しくロードされたか確認
        Assert.AreEqual("multi_sendrecv", SceneManager.GetActiveScene().name, "Scene did not load correctly!");

        // 接続ボタンを探して押す
        Button firstButton = GameObject.Find("ButtonStart").GetComponent<Button>();
        Assert.IsNotNull(firstButton, "FirstButton was not found in the scene!");
        firstButton.onClick.Invoke();

        // 20秒待機
        yield return new WaitForSeconds(20);

        // 切断ボタンを探して押す
        Button secondButton = GameObject.Find("ButtonEnd").GetComponent<Button>();
        Assert.IsNotNull(secondButton, "SecondButton was not found in the scene!");
        secondButton.onClick.Invoke();

        yield return null;
    }
}
