﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class SoraSampleMultistream : MonoBehaviour
{
    Sora sora;
    Dictionary<uint, GameObject> tracks = new Dictionary<uint, GameObject>();
    public GameObject scrollViewContent;
    public GameObject baseContent;
    public string SignalingUrl;
    public string ChannelId;
    public bool Recvonly;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (sora == null)
        {
            return;
        }

        sora.DispatchEvents();

        foreach (var track in tracks)
        {
            var image = track.Value.GetComponent<UnityEngine.UI.RawImage>();
            sora.RenderTrackToTexture(track.Key, image.texture);
        }
    }
    void InitSora()
    {
        DisposeSora();

        sora = new Sora();
        sora.OnAddTrack = (trackId) =>
        {
            Debug.LogFormat("OnAddTrack: trackId={0}", trackId);
            var obj = GameObject.Instantiate(baseContent, Vector3.zero, Quaternion.identity);
            obj.name = string.Format("track {0}", trackId);
            obj.transform.SetParent(scrollViewContent.transform);
            obj.SetActive(true);
            var image = obj.GetComponent<UnityEngine.UI.RawImage>();
            image.texture = new Texture2D(320, 240, TextureFormat.RGBA32, false);
            tracks.Add(trackId, obj);
        };
        sora.OnRemoveTrack = (trackId) =>
        {
            Debug.LogFormat("OnRemoveTrack: trackId={0}", trackId);
            if (tracks.ContainsKey(trackId))
            {
                GameObject.Destroy(tracks[trackId]);
                tracks.Remove(trackId);
            }
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
        foreach (var track in tracks)
        {
            GameObject.Destroy(track.Value);
        }
        tracks.Clear();
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
            Mode = Recvonly ? Sora.Mode.Multistream_Recvonly : Sora.Mode.Multistream_Sendrecv,
        };
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
