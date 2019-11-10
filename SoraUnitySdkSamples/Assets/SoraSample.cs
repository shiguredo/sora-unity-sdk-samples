using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class SoraSample : MonoBehaviour
{
    Sora sora;
    uint trackId = 0;
    public GameObject renderTarget;
    public string SignalingUrl;
    public string ChannelId;
    public bool Recvonly;

    public Camera capturedCamera;

    // Start is called before the first frame update
    void Start()
    {
        var image = renderTarget.GetComponent<UnityEngine.UI.RawImage>();
        image.texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
        StartCoroutine(Render());
    }

    IEnumerator Render()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (sora != null)
            {
                sora.OnRender();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sora == null)
        {
            return;
        }

        sora.DispatchEvents();

        if (trackId != 0)
        {
            var image = renderTarget.GetComponent<UnityEngine.UI.RawImage>();
            sora.RenderTrackToTexture(trackId, image.texture);
        }
    }
    void InitSora()
    {
        DisposeSora();

        sora = new Sora();
        sora.OnAddTrack = (trackId) =>
        {
            Debug.LogFormat("OnAddTrack: trackId={0}", trackId);
            this.trackId = trackId;
        };
        sora.OnRemoveTrack = (trackId) =>
        {
            Debug.LogFormat("OnRemoveTrack: trackId={0}", trackId);
        };
    }
    void DisposeSora()
    {
        if (sora != null)
        {
            sora.Dispose();
            sora = null;
            Debug.Log("Sora is Disposed");
        }
    }

    [Serializable]
    class Settings
    {
        public string signaling_url = "";
        public string channel_id = "";
    }

    public void OnClickStart()
    {
        // 開発用の機能。
        // .env.json ファイルがあったら、それを読んでシグナリングURLとチャンネルIDを設定する。
        if (SignalingUrl.Length == 0 && ChannelId.Length == 0 && System.IO.File.Exists(".env.json"))
        {
            var settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(".env.json"));
            SignalingUrl = settings.signaling_url;
            ChannelId = settings.channel_id;
        }

        if (SignalingUrl.Length == 0)
        {
            Debug.LogError("シグナリング URL が設定されていません");
            return;
        }
        if (ChannelId.Length == 0)
        {
            Debug.LogError("チャンネル ID が設定されていません");
            return;
        }

        InitSora();

        var config = new Sora.Config()
        {
            SignalingUrl = SignalingUrl,
            ChannelId = ChannelId,
            Role = Recvonly ? Sora.Role.Downstream : Sora.Role.Upstream,
            Multistream = false,
        };
        config.SetUnityCamera(capturedCamera, 640, 480);

        var success = sora.Connect(config);
        if (!success)
        {
            sora.Dispose();
            sora = null;
            Debug.LogErrorFormat("Sora.Connect failed: SignalingUrl={0}, ChannelId={1}", SignalingUrl, ChannelId);
            return;
        }
        Debug.LogFormat("Sora is Created: SignalingUrl={0}, ChannelId={1}", SignalingUrl, ChannelId);
    }
    public void OnClickEnd()
    {
        DisposeSora();
    }

    void OnApplicationQuit()
    {
        DisposeSora();
    }
}