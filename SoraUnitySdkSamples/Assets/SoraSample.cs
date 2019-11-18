using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class SoraSample : MonoBehaviour
{
    public enum SampleType
    {
        MultiPubsub,
        MultiSub,
        Pub,
        Sub,
    }

    Sora sora;
    public SampleType sampleType;
    // 実行中に変えられたくないので実行時に固定する
    SampleType fixedSampleType;

    // 非マルチストリームで利用する
    uint trackId = 0;
    public GameObject renderTarget;

    // マルチストリームで利用する
    Dictionary<uint, GameObject> tracks = new Dictionary<uint, GameObject>();
    public GameObject scrollViewContent;
    public GameObject baseContent;

    // 以下共通
    public string signalingUrl = "";
    public string channelId = "";
    public string signalingKey = "";

    public bool captureUnityCamera;
    public Camera capturedCamera;

    public bool unityAudioInput = false;
    public AudioSource audioSourceInput;
    public bool unityAudioOutput = false;
    public AudioSource audioSourceOutput;
    bool fixedUnityAudioInput = false;
    bool fixedUnityAudioOutput = false;

    public string videoCapturerDevice = "";
    public string audioRecordingDevice = "";
    public string audioPlayoutDevice = "";

    public bool Recvonly { get { return fixedSampleType == SampleType.Sub || fixedSampleType == SampleType.MultiSub; } }
    public bool Multistream { get { return fixedSampleType == SampleType.MultiPubsub || fixedSampleType == SampleType.MultiSub; } }

    Queue<short[]> audioBuffer = new Queue<short[]>();
    int audioBufferSamples = 0;
    int audioBufferPosition = 0;

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
        fixedSampleType = sampleType;
        fixedUnityAudioInput = unityAudioInput;
        fixedUnityAudioOutput = unityAudioOutput;

        DumpDeviceInfo("video capturer devices", Sora.GetVideoCapturerDevices());
        DumpDeviceInfo("audio recording devices", Sora.GetAudioRecordingDevices());
        DumpDeviceInfo("audio playout devices", Sora.GetAudioPlayoutDevices());

        if (!Multistream)
        {
            var image = renderTarget.GetComponent<UnityEngine.UI.RawImage>();
            image.texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
        }
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
            if (sora != null && fixedUnityAudioInput)
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

    // Update is called once per frame
    void Update()
    {
        if (sora == null)
        {
            return;
        }

        sora.DispatchEvents();

        if (!Multistream)
        {
            if (trackId != 0)
            {
                var image = renderTarget.GetComponent<UnityEngine.UI.RawImage>();
                sora.RenderTrackToTexture(trackId, image.texture);
            }
        }
        else
        {
            foreach (var track in tracks)
            {
                var image = track.Value.GetComponent<UnityEngine.UI.RawImage>();
                sora.RenderTrackToTexture(track.Key, image.texture);
            }
        }
    }
    void InitSora()
    {
        DisposeSora();

        sora = new Sora();
        if (!Multistream)
        {
            sora.OnAddTrack = (trackId) =>
            {
                Debug.LogFormat("OnAddTrack: trackId={0}", trackId);
                this.trackId = trackId;
            };
            sora.OnRemoveTrack = (trackId) =>
            {
                Debug.LogFormat("OnRemoveTrack: trackId={0}", trackId);
                this.trackId = 0;
            };
        }
        else
        {
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
        sora.OnNotify = (json) =>
        {
            Debug.LogFormat("OnNotify: {0}", json);
        };
        // これは別スレッドからやってくるので注意すること
        sora.OnHandleAudio = (buf, samples, channels) =>
        {
            lock (audioBuffer)
            {
                audioBuffer.Enqueue(buf);
                audioBufferSamples += samples;
            }
        };

        if (fixedUnityAudioOutput)
        {
            var audioClip = AudioClip.Create("AudioClip", 480000, 1, 48000, true, (data) =>
            {
                lock (audioBuffer)
                {
                    if (audioBufferSamples < data.Length)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            data[i] = 0.0f;
                        }
                        return;
                    }

                    var p = audioBuffer.Peek();
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = p[audioBufferPosition] / 32768.0f;
                        ++audioBufferPosition;
                        if (audioBufferPosition >= p.Length)
                        {
                            audioBuffer.Dequeue();
                            p = audioBuffer.Peek();
                            audioBufferPosition = 0;
                        }
                    }
                    audioBufferSamples -= data.Length;
                }
            });
            audioSourceOutput.clip = audioClip;
            audioSourceOutput.Play();
        }

        if (fixedUnityAudioInput)
        {
            AudioRenderer.Start();
            audioSourceInput.Play();
        }
    }
    void DisposeSora()
    {
        if (sora != null)
        {
            sora.Dispose();
            sora = null;
            Debug.Log("Sora is Disposed");
            if (fixedUnityAudioInput)
            {
                audioSourceInput.Stop();
                AudioRenderer.Stop();
            }

            if (fixedUnityAudioOutput)
            {
                audioSourceOutput.Stop();
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
        if (signalingUrl.Length == 0 && channelId.Length == 0 && System.IO.File.Exists(".env.json"))
        {
            var settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(".env.json"));
            signalingUrl = settings.signaling_url;
            channelId = settings.channel_id;
            signalingKey = settings.signaling_key;
        }

        if (signalingUrl.Length == 0)
        {
            Debug.LogError("シグナリング URL が設定されていません");
            return;
        }
        if (channelId.Length == 0)
        {
            Debug.LogError("チャンネル ID が設定されていません");
            return;
        }
        // signalingKey がある場合はメタデータを設定する
        string metadata = "";
        if (signalingKey.Length != 0)
        {
            var md = new Metadata()
            {
                signaling_key = signalingKey
            };
            metadata = JsonUtility.ToJson(md);
        }

        sampleType = fixedSampleType;
        unityAudioInput = fixedUnityAudioInput;
        unityAudioOutput = fixedUnityAudioOutput;

        InitSora();

        var config = new Sora.Config()
        {
            SignalingUrl = signalingUrl,
            ChannelId = channelId,
            Metadata = metadata,
            Role = Recvonly ? Sora.Role.Downstream : Sora.Role.Upstream,
            Multistream = Multistream,
            UnityAudioInput = unityAudioInput,
            UnityAudioOutput = unityAudioOutput,
            VideoCapturerDevice = videoCapturerDevice,
            AudioRecordingDevice = audioRecordingDevice,
            AudioPlayoutDevice = audioPlayoutDevice,
        };
        if (captureUnityCamera && capturedCamera != null)
        {
            config.CapturerType = Sora.CapturerType.UnityCamera;
            config.UnityCamera = capturedCamera;
        }

        var success = sora.Connect(config);
        if (!success)
        {
            sora.Dispose();
            sora = null;
            Debug.LogErrorFormat("Sora.Connect failed: signalingUrl={0}, channelId={1}", signalingUrl, channelId);
            return;
        }
        Debug.LogFormat("Sora is Created: signalingUrl={0}, channelId={1}", signalingUrl, channelId);
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