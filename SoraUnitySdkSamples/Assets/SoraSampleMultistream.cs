using System.Collections;
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
    public string SignalingKey;
    public bool Recvonly;

    public bool captureUnityCamera;
    public Camera capturedCamera;

    public bool UnityAudioInput = false;
    public AudioSource audioSource;

    public string VideoCapturerDevice = "";
    public string AudioRecordingDevice = "";
    public string AudioPlayoutDevice = "";

    void DumpDeviceInfo(string name, Sora.DeviceInfo[] infos)
    {
        Debug.LogFormat("------------ {0} --------------", name);
        foreach (var info in infos)
        {
            Debug.LogFormat("DeviceName={0} UniqueName={1}", info.DeviceName, info.UniqueName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DumpDeviceInfo("video capturer devices", Sora.GetVideoCapturerDevices());
        DumpDeviceInfo("audio recording devices", Sora.GetAudioRecordingDevices());
        DumpDeviceInfo("audio playout devices", Sora.GetAudioPlayoutDevices());

        StartCoroutine(Render());
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
        sora.OnNotify = (json) =>
        {
            Debug.LogFormat("OnNotify: {0}", json);
        };
        AudioRenderer.Start();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    void DisposeSora()
    {
        if (sora != null)
        {
            sora.Dispose();
            sora = null;
            Debug.Log("Sora is Disposed");
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            AudioRenderer.Stop();
        }
        foreach (var track in tracks)
        {
            GameObject.Destroy(track.Value);
        }
        tracks.Clear();
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
            if (sora != null)
            {
                var samples = AudioRenderer.GetSampleCountForCaptureFrame();
                if (AudioSettings.speakerMode == AudioSpeakerMode.Stereo)
                {
                    using (var buf = new Unity.Collections.NativeArray<float>(samples * 2, Unity.Collections.Allocator.Temp))
                    {
                        AudioRenderer.Render(buf);
                        sora.ProcessAudio(buf.ToArray(), 0, samples);
                    }
                }
            }
        }
    }

    [Serializable]
    class Settings
    {
        public string signaling_url = "";
        public string channel_id = "";
        public string signaling_key = "";
    }

    [Serializable]
    class Metadata
    {
        public string signaling_key;
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
            SignalingKey = settings.signaling_key;
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
        // SignalingKey がある場合はメタデータを設定する
        string metadata = "";
        if (SignalingKey.Length != 0)
        {
            var md = new Metadata()
            {
                signaling_key = SignalingKey
            };
            metadata = JsonUtility.ToJson(md);
        }

        InitSora();

        var config = new Sora.Config()
        {
            SignalingUrl = SignalingUrl,
            ChannelId = ChannelId,
            Metadata = metadata,
            Role = Recvonly ? Sora.Role.Downstream : Sora.Role.Upstream,
            Multistream = true,
            UnityAudioInput = UnityAudioInput,
            VideoCapturerDevice = VideoCapturerDevice,
            AudioRecordingDevice = AudioRecordingDevice,
            AudioPlayoutDevice = AudioPlayoutDevice,
        };
        if (captureUnityCamera && capturedCamera != null)
        {
            config.SetUnityCamera(capturedCamera, 640, 480);
        }

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
