using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class SoraSample : MonoBehaviour
{
    public enum SampleType
    {
        MultiSendrecv,
        MultiRecvonly,
        MultiSendonly,
    }

    Sora sora;
    Sora.IAudioOutputHelper audioOutputHelper;
    enum State
    {
        Init,
        Started,
        Disconnecting,
    }
    State state;
    public UnityEngine.UI.Button buttonStart;
    public UnityEngine.UI.Button buttonEnd;
    public UnityEngine.UI.Button buttonSend;
    public UnityEngine.UI.Button buttonVideoMute;
    public UnityEngine.UI.Button buttonAudioMute;
    public UnityEngine.UI.Button buttonSwitchHandsfree;

    public SampleType sampleType;
    // 実行中に変えられたくないので実行時に固定する
    SampleType fixedSampleType;

    // Sendonly で利用する
    // 受信を行わないので、自身のカメラを表示するための trackId だけ保持しておく
    uint trackId = 0;
    public GameObject renderTarget;

    // Recvonly, Sendrecv で利用する
    Dictionary<uint, GameObject> tracks = new Dictionary<uint, GameObject>();
    public GameObject scrollViewContent;
    public GameObject baseContent;

    // 以下は Sendonly, Recvonly, Sendrecv 共通
    public string signalingUrl = "";
    public string[] signalingUrlCandidate = new string[0];

    public bool insecure = false;
    public string channelId = "";
    public string clientId = "";
    public string bundleId = "";
    public string signalingNotifyMetadataMessage = "";
    public string metadataAccessToken = "";

    public bool captureUnityCamera;
    public Camera capturedCamera;

    // enableVideoVp9Params や enableOrdered といった enable<FieldName> という名前のフィールドは、
    // enable<FieldName> == false の場合は Sora に <FieldName> の JSON オブジェクトを送信しないことを意味する。
    // そのため <FieldName> のフィールドがどのような値であっても無視される。
    // enable<FieldName> == true の場合は Sora に <FieldName> の JSON オブジェクトを送信することを意味する。
    // そのため <FieldName> に適切な値を設定する必要がある。

    public bool video = true;
    public bool noVideoDevice = false;
    public new bool audio = true;
    public bool noAudioDevice = false;
    public bool useHardwareEncoder = true;
    public Sora.VideoCodecType videoCodecType = Sora.VideoCodecType.VP9;
    public bool enableVideoVp9Params = false;
    public int videoVp9ParamsProfileId;
    public bool enableVideoAv1Params = false;
    public int videoAv1ParamsProfile;
    public bool enableVideoH264Params = false;
    public string videoH264ParamsProfileLevelId = "";
    public Sora.AudioCodecType audioCodecType = Sora.AudioCodecType.OPUS;
    public string audioStreamingLanguageCode = "";

    public bool unityAudioInput = false;
    public AudioSource audioSourceInput;
    public bool unityAudioOutput = false;
    public AudioSource audioSourceOutput;

    public string videoCapturerDevice = "";
    public string audioRecordingDevice = "";
    public string audioPlayoutDevice = "";

    public bool spotlight = false;
    public int spotlightNumber = 0;
    public bool enableSpotlightFocusRid = false;
    public Sora.SpotlightFocusRidType spotlightFocusRid = Sora.SpotlightFocusRidType.None;
    public bool enableSpotlightUnfocusRid = false;
    public Sora.SpotlightFocusRidType spotlightUnfocusRid = Sora.SpotlightFocusRidType.None;
    public bool simulcast = false;
    public bool enableSimulcastRid = false;
    public Sora.SimulcastRidType simulcastRid = Sora.SimulcastRidType.R0;

    public int videoBitRate = 0;
    public int videoFps = 30;
    public enum VideoSize
    {
        QVGA,
        VGA,
        HD,
        FHD,
        _4K,
    }
    public VideoSize videoSize = VideoSize.VGA;

    [System.Serializable]
    public class Rule
    {
        public string field;
        public string op;
        public string[] values;
    }
    [System.Serializable]
    public class RuleList
    {
        public Rule[] data;
    }

    [Header("ForwardingFilter の設定")]
    public bool enableForwardingFilterAction = false;
    public string forwardingFilterAction;
    public bool enableForwardingFilterName = false;
    public string forwardingFilterName;
    public bool enableForwardingFilterPriority = false;
    public int forwardingFilterPriority;
    public RuleList[] forwardingFilter;
    public bool enableForwardingFilterVersion = false;
    public string forwardingFilterVersion = "";
    public bool enableForwardingFilterMetadata = false;
    public string forwardingFilterMetadataMessage = "";

    [Header("DataChannel シグナリングの設定")]
    public bool dataChannelSignaling = false;
    public int dataChannelSignalingTimeout = 30;
    public bool ignoreDisconnectWebsocket = false;
    public int disconnectWaitTimeout = 5;

    [System.Serializable]
    public class DataChannel
    {
        public string label = "";
        public Sora.Direction direction = Sora.Direction.Sendrecv;
        public bool enableOrdered;
        public bool ordered;
        public bool enableMaxPacketLifeTime;
        public int maxPacketLifeTime;
        public bool enableMaxRetransmits;
        public int maxRetransmits;
        public bool enableProtocol;
        public string protocol;
        public bool enableCompress;
        public bool compress;
    }

    [Header("DataChannel メッセージングの設定")]
    public DataChannel[] dataChannels;
    string[] fixedDataChannelLabels;

    [Header("HTTP Proxy の設定")]
    public string proxyUrl;
    public string proxyUsername;
    public string proxyPassword;

    // Recvonly → 受信のみ
    // !Recvonly → 送信のみ、あるいは送受信のどちらか
    public bool Recvonly { get { return fixedSampleType == SampleType.MultiRecvonly; } }
    // Sendonly → 送信のみ
    // !Sendonly → 受信のみ、あるいは送受信のどちらか
    public bool Sendonly { get { return fixedSampleType == SampleType.MultiSendonly; } }

    public Sora.Role Role
    {
        get
        {
            return
                fixedSampleType == SampleType.MultiSendonly ? Sora.Role.Sendonly :
                fixedSampleType == SampleType.MultiRecvonly ? Sora.Role.Recvonly : Sora.Role.Sendrecv;
        }
    }

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
#if !UNITY_EDITOR && UNITY_ANDROID
        // Unity for Android は、C# コード中に WebCamTexture や Microphone にアクセスすることで
        // 自動的に必要な権限を AndroidManifest.xml に追加してくれるため、ここで適当にアクセスしておく
        var x = WebCamTexture.devices;
        var y = Microphone.devices;
#endif
        fixedSampleType = sampleType;

        DumpDeviceInfo("video capturer devices", Sora.GetVideoCapturerDevices());
        DumpDeviceInfo("audio recording devices", Sora.GetAudioRecordingDevices());
        DumpDeviceInfo("audio playout devices", Sora.GetAudioPlayoutDevices());

        // 送信のみの場合はトラック追加の度にテクスチャを追加したりする必要は無いので、ここで初期化しておく。
        // 受信がある場合は表示する数が動的に変わるのでここで初期化することはできない。
        if (Sendonly)
        {
            var image = renderTarget.GetComponent<UnityEngine.UI.RawImage>();
            image.texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
        }
        SetState(State.Init);
        StartCoroutine(Render());
        StartCoroutine(GetStats());
    }

    IEnumerator Render()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // Unity カメラの映像をキャプチャする
            // 必ず yield return new WaitForEndOfFrame() の後に呼び出すこと
            if (state == State.Started)
            {
                sora.OnRender();
            }
            // Unity から出力された音を録音データとして Sora に渡す
            if (state == State.Started && unityAudioInput && !Recvonly)
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
    IEnumerator GetStats()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            if (state != State.Started)
            {
                continue;
            }

            sora.GetStats((stats) =>
            {
                // ここは Unity スレッドとは別のスレッドで呼び出されるので注意すること
                Debug.LogFormat("GetStats: {0}", stats);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sora == null)
        {
            return;
        }

        // 各種コールバックの呼び出し
        sora.DispatchEvents();

        // DispatchEvents で OnDisconnect → DisposeSora と呼ばれて sora が null になることがある
        if (sora == null)
        {
            return;
        }

        // 送信してるカメラの映像と、受信した他人の映像をテクスチャにレンダリングする
        if (Sendonly)
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
    void OnChangeRoute()
    {
        if (audioOutputHelper == null)
        {
            return;
        }
        Debug.LogFormat("OnChangeRoute : " + (audioOutputHelper.IsHandsfree() ? "ハンズフリー OFF" : "ハンズフリー ON"));
        // ボタンのラベルを変更する
        buttonSwitchHandsfree.GetComponentInChildren<Text>().text =
        audioOutputHelper.IsHandsfree() ? "ハンズフリー ON" : "ハンズフリー OFF";
    }
    void InitSora()
    {
        DisposeSora();

        // 送信のみの場合は音声出力を行わないので、AudioOutputHelper を作成しない
        if (!Sendonly)
        {
            audioOutputHelper = Sora.AudioOutputHelperFactory.Create(OnChangeRoute);
        }

        sora = new Sora();
        if (Sendonly)
        {
            // 送信のみなので、表示するのは自分のカメラだけになる
            sora.OnAddTrack = (trackId, connectionId) =>
            {
                Debug.LogFormat("OnAddTrack: trackId={0}, connectionId={1}", trackId, connectionId);
                this.trackId = trackId;
            };
            sora.OnRemoveTrack = (trackId, connectionId) =>
            {
                Debug.LogFormat("OnRemoveTrack: trackId={0}, connectionId={1}", trackId, connectionId);
                this.trackId = 0;
            };
        }
        else
        {
            // 受信する数は動的に増減するので、トラックが追加されるたびに
            // 動的に GameObject とテクスチャを追加して設定しておく
            sora.OnAddTrack = (trackId, connectionId) =>
            {
                // connectionId == "" だったら送信者のカメラ映像用のトラックになるが、このサンプルでは区別する必要が無いので
                // どちらの場合でも気にせず GameObject を作成する
                Debug.LogFormat("OnAddTrack: trackId={0}, connectionId={1}", trackId, connectionId);
                var obj = GameObject.Instantiate(baseContent, Vector3.zero, Quaternion.identity);
                obj.name = string.Format("track {0}", trackId);
                obj.transform.SetParent(scrollViewContent.transform);
                obj.SetActive(true);
                var image = obj.GetComponent<UnityEngine.UI.RawImage>();
                image.texture = new Texture2D(320, 240, TextureFormat.RGBA32, false);
                tracks.Add(trackId, obj);
            };
            sora.OnRemoveTrack = (trackId, connectionId) =>
            {
                Debug.LogFormat("OnRemoveTrack: trackId={0}, connectionId={1}", trackId, connectionId);
                if (tracks.ContainsKey(trackId))
                {
                    var obj = tracks[trackId];
                    var image = obj.GetComponent<UnityEngine.UI.RawImage>();
                    GameObject.Destroy(image.texture);
                    GameObject.Destroy(obj);
                    tracks.Remove(trackId);
                }
            };
        }
        sora.OnSetOffer = (json) =>
        {
            Debug.LogFormat("OnSetOffer: {0}", json);
        };
        sora.OnNotify = (json) =>
        {
            Debug.LogFormat("OnNotify: {0}", json);
        };
        sora.OnPush = (json) =>
        {
            Debug.LogFormat("OnPush: {0}", json);
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
        sora.OnMessage = (label, data) =>
        {
            Debug.LogFormat("OnMessage: label={0} data={1}", label, System.Text.Encoding.UTF8.GetString(data));
        };
        // 切断時のコールバック
        // sora.Connect() を呼び出した後、エラー時や切断時に１回だけ OnDisconnect が呼ばれる。
        sora.OnDisconnect = (code, message) =>
        {
            Debug.LogFormat("OnDisconnect: code={0} message={1}", code.ToString(), message);
            // sora.Dispose() は必ず OnDisconnect を受信してから行うこと。
            DisposeSora();
        };
        sora.OnDataChannel = (label) =>
        {
            Debug.LogFormat("OnDataChannel: label={0}", label);
        };
        // カメラデバイスから取得したフレーム情報を受け取るコールバック。
        // これは別スレッドからやってくるので注意すること。
        // また、このコールバックの範囲外ではポインタは無効になるので必要に応じてデータをコピーして利用すること。
        sora.OnCapturerFrame = (frame) =>
        {
            // var vfb = frame.video_frame_buffer;
            // Debug.LogFormat("OnCapturerFrame: type={0} width={1} height={2}", vfb.type, vfb.width, vfb.height);

            // // I420 の映像データを byte[] にコピーする
            // if (vfb.type == SoraConf.VideoFrameBuffer.Type.kI420)
            // {
            //     var size_y = vfb.i420_stride_y * vfb.height;
            //     var data_y = new byte[size_y];
            //     Marshal.Copy((IntPtr)vfb.i420_data_y, data_y, 0, size_y);

            //     var size_u = vfb.i420_stride_u * ((vfb.height + 1) / 2);
            //     var data_u = new byte[size_u];
            //     Marshal.Copy((IntPtr)vfb.i420_data_u, data_u, 0, size_u);

            //     var size_v = vfb.i420_stride_v * ((vfb.height + 1) / 2);
            //     var data_v = new byte[size_v];
            //     Marshal.Copy((IntPtr)vfb.i420_data_v, data_v, 0, size_v);
            // }
        };

        if (unityAudioOutput)
        {
            // 再生デバイスから再生する代わりに、AudioSource を利用して再生する
            var audioClip = AudioClip.Create("AudioClip", 480000, 1, 48000, true, (data) =>
            {
                lock (audioBuffer)
                {
                    if (audioBuffer.Count == 0 || audioBufferSamples < data.Length)
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
                        while (audioBufferPosition >= p.Length)
                        {
                            audioBuffer.Dequeue();
                            p = audioBuffer.Peek();
                            audioBufferPosition = 0;
                        }
                        data[i] = p[audioBufferPosition] / 32768.0f;
                        ++audioBufferPosition;
                    }
                    audioBufferSamples -= data.Length;
                }
            });
            audioSourceOutput.clip = audioClip;
            audioSourceOutput.Play();
        }

        if (unityAudioInput && !Recvonly)
        {
            // Unity で再生する音を録音データとしてキャプチャできるようにする
            AudioRenderer.Start();
            // Unity 内で音を鳴らさないと無音をキャプチャし続けることになって正しく動作してるか分からないので、
            // 適当な音を無限ループで鳴らしておく
            audioSourceInput.Play();
        }
    }
    // Sora を切断する。
    // sora.Disconnect() は非同期処理であるため、この時点ではまだ sora.Dispose() を呼び出してはいけない。
    void DisconnectSora()
    {
        if (sora == null)
        {
            return;
        }
        sora.Disconnect();
        DestroyComponents();
        SetState(State.Disconnecting);
    }
    void DestroyComponents()
    {
        if (state != State.Started)
        {
            return;
        }

        if (!Sendonly)
        {
            foreach (var track in tracks)
            {
                var obj = track.Value;
                var image = obj.GetComponent<UnityEngine.UI.RawImage>();
                GameObject.Destroy(image.texture);
                GameObject.Destroy(obj);
            }
            tracks.Clear();
        }
        if (unityAudioInput && !Recvonly)
        {
            audioSourceInput.Stop();
            AudioRenderer.Stop();
        }

        if (unityAudioOutput)
        {
            audioSourceOutput.Stop();
        }
    }
    void DisposeSora()
    {
        if (sora == null)
        {
            return;
        }
        sora.Dispose();
        sora = null;
        if (audioOutputHelper != null)
        {
            audioOutputHelper.Dispose();
            audioOutputHelper = null;
        }
        Debug.Log("Sora is Disposed");
        DestroyComponents();
        SetState(State.Init);
    }

    void SetState(State state)
    {
        if (state == State.Init)
        {
            buttonStart.interactable = true;
            buttonEnd.interactable = false;
            buttonSend.interactable = false;
            if (buttonVideoMute != null) buttonVideoMute.interactable = false;
            if (buttonAudioMute != null) buttonAudioMute.interactable = false;
        }
        if (state == State.Started)
        {
            buttonStart.interactable = false;
            buttonEnd.interactable = true;
            buttonSend.interactable = true;
            if (buttonVideoMute != null) buttonVideoMute.interactable = true;
            if (buttonAudioMute != null) buttonAudioMute.interactable = true;
        }
        if (state == State.Disconnecting)
        {
            buttonStart.interactable = false;
            buttonEnd.interactable = false;
            buttonSend.interactable = false;
            if (buttonVideoMute != null) buttonVideoMute.interactable = false;
            if (buttonAudioMute != null) buttonAudioMute.interactable = false;
        }
        this.state = state;
    }

    [Serializable]
    class Settings
    {
        public string signaling_url = "";
        public string[] signaling_url_candidate = new string[0];
        public string channel_id = "";
        public string access_token = "";
    }

    [Serializable]
    class SignalingNotifyMetadata
    {
        public string message;
    }

    [Serializable]
    class Metadata
    {
        public string access_token;
    }
    [Serializable]
    class VideoVp9Params
    {
        public int profile_id;
    }
    [Serializable]
    class VideoAv1Params
    {
        public int profile;
    }
    [Serializable]
    class VideoH264Params
    {
        public string profile_level_id;
    }

    [Serializable]
    class ForwardingFilterMetadata
    {
        public string forwarding_filter_metadata;
    }

    public void OnClickStart()
    {

        // 開発用の機能。
        // .env.json ファイルがあったら、それを読んでシグナリングURLとチャンネルIDを設定する。
        if (signalingUrl.Length == 0 && channelId.Length == 0 && System.IO.File.Exists(".env.json"))
        {
            var settings = JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(".env.json"));
            signalingUrl = settings.signaling_url;
            signalingUrlCandidate = settings.signaling_url_candidate;
            channelId = settings.channel_id;
            metadataAccessToken = settings.access_token;
        }

        if (signalingUrl.Length == 0 && signalingUrlCandidate.Length == 0)
        {
            Debug.LogError("シグナリング URL が設定されていません");
            return;
        }
        if (channelId.Length == 0)
        {
            Debug.LogError("チャンネル ID が設定されていません");
            return;
        }
        // signalingNotifyMetadataMessage がある場合はメタデータを設定する
        string signalingNotifyMetadata = "";
        if (signalingNotifyMetadataMessage.Length != 0)
        {
            var snmd = new SignalingNotifyMetadata()
            {
                message = signalingNotifyMetadataMessage
            };
            signalingNotifyMetadata = JsonUtility.ToJson(snmd);
        }
        // metadataAccessToken がある場合はメタデータを設定する
        string metadata = "";
        if (metadataAccessToken.Length != 0)
        {
            var md = new Metadata()
            {
                access_token = metadataAccessToken
            };
            metadata = JsonUtility.ToJson(md);
        }
        // enableVideoVp9Params が true の場合はメタデータを設定する
        string videoVp9ParamsJson = "";
        if (enableVideoVp9Params)
        {
            var vp9Params = new VideoVp9Params()
            {
                profile_id = videoVp9ParamsProfileId
            };
            videoVp9ParamsJson = JsonUtility.ToJson(vp9Params);
        }
        // enableVideoAv1Params が true の場合はメタデータを設定する
        string videoAv1ParamsJson = "";
        if (enableVideoAv1Params)
        {
            var av1Params = new VideoAv1Params()
            {
                profile = videoAv1ParamsProfile
            };
            videoAv1ParamsJson = JsonUtility.ToJson(av1Params);
        }
        // enableVideoH264Params が true の場合はメタデータを設定する
        string videoH264ParamsJson = "";
        if (enableVideoH264Params)
        {
            var h264Params = new VideoH264Params()
            {
                profile_level_id = videoH264ParamsProfileLevelId
            };
            videoH264ParamsJson = JsonUtility.ToJson(h264Params);
        }
        // enableForwardingFilterMetadata が true の場合はメタデータを設定する
        string forwardingFilterMetadataJson = "";
        if (enableForwardingFilterMetadata)
        {
            var ffm = new ForwardingFilterMetadata()
            {
                forwarding_filter_metadata = forwardingFilterMetadataMessage
            };
            forwardingFilterMetadataJson = JsonUtility.ToJson(ffm);
        }

        InitSora();

        int videoWidth;
        int videoHeight;
        GetVideoSize(videoSize, out videoWidth, out videoHeight);

        var config = new Sora.Config()
        {
            SignalingUrl = signalingUrl,
            SignalingUrlCandidate = signalingUrlCandidate,
            ChannelId = channelId,
            ClientId = clientId,
            BundleId = bundleId,
            SignalingNotifyMetadata = signalingNotifyMetadata,
            Metadata = metadata,
            Role = Role,
            Multistream = true,
            Insecure = insecure,
            Video = video,
            NoVideoDevice = noVideoDevice,
            Audio = audio,
            NoAudioDevice = noAudioDevice,
            UseHardwareEncoder = useHardwareEncoder,
            VideoCodecType = videoCodecType,
            VideoVp9Params = videoVp9ParamsJson,
            VideoAv1Params = videoAv1ParamsJson,
            VideoH264Params = videoH264ParamsJson,
            VideoBitRate = videoBitRate,
            CameraConfig = new Sora.CameraConfig()
            {
                CapturerType = captureUnityCamera && capturedCamera != null ? Sora.CapturerType.UnityCamera : Sora.CapturerType.DeviceCamera,
                UnityCamera = capturedCamera,
                VideoFps = videoFps,
                VideoWidth = videoWidth,
                VideoHeight = videoHeight,
                VideoCapturerDevice = videoCapturerDevice,
            },
            AudioCodecType = audioCodecType,
            AudioStreamingLanguageCode = audioStreamingLanguageCode,
            UnityAudioInput = unityAudioInput,
            UnityAudioOutput = unityAudioOutput,
            AudioRecordingDevice = audioRecordingDevice,
            AudioPlayoutDevice = audioPlayoutDevice,
            Spotlight = spotlight,
            SpotlightNumber = spotlightNumber,
            Simulcast = simulcast,
            // EnableDataChannelSignaling と DataChannelSignaling に dataChannelSignaling を指定しているが、
            // この実装だと dataChannelSignaling == false の場合は data_channel_signaling が無指定になるので、
            // サーバ側の判断によっては DC に切り替えられる可能性がある。
            EnableDataChannelSignaling = dataChannelSignaling,
            DataChannelSignaling = dataChannelSignaling,
            DataChannelSignalingTimeout = dataChannelSignalingTimeout,
            // EnableIgnoreDisconnectWebsocket と IgnoreDisconnectWebsocket に ignoreDisconnectWebsocket を指定しているが、
            // これも dataChannelSignaling と同じようにサーバ側の判断によっては切断される可能性がある。
            EnableIgnoreDisconnectWebsocket = ignoreDisconnectWebsocket,
            IgnoreDisconnectWebsocket = ignoreDisconnectWebsocket,
            DisconnectWaitTimeout = disconnectWaitTimeout,
            ProxyUrl = proxyUrl,
            ProxyUsername = proxyUsername,
            ProxyPassword = proxyPassword,
        };
        if (enableSpotlightFocusRid)
        {
            config.SpotlightFocusRid = spotlightFocusRid;
        }
        if (enableSpotlightUnfocusRid)
        {
            config.SpotlightUnfocusRid = spotlightUnfocusRid;
        }
        if (enableSimulcastRid)
        {
            config.SimulcastRid = simulcastRid;
        }
        if (dataChannels != null)
        {
            foreach (var m in dataChannels)
            {
                var c = new Sora.DataChannel();
                c.Label = m.label;
                c.Direction = m.direction;
                if (m.enableOrdered) c.Ordered = m.ordered;
                if (m.enableMaxPacketLifeTime) c.MaxPacketLifeTime = m.maxPacketLifeTime;
                if (m.enableMaxRetransmits) c.MaxRetransmits = m.maxRetransmits;
                if (m.enableProtocol) c.Protocol = m.protocol;
                if (m.enableCompress) c.Compress = m.compress;
                config.DataChannels.Add(c);
            }
            fixedDataChannelLabels = config.DataChannels.Select(x => x.Label).ToArray();
        }
        if (forwardingFilter.Length != 0)
        {
            config.ForwardingFilter = new Sora.ForwardingFilter();
            if (enableForwardingFilterAction) config.ForwardingFilter.Action = forwardingFilterAction;
            if (enableForwardingFilterName) config.ForwardingFilter.Name = forwardingFilterName;
            if (enableForwardingFilterPriority) config.ForwardingFilter.Priority = forwardingFilterPriority;
            foreach (var rs in forwardingFilter)
            {
                var ccrs = new List<Sora.ForwardingFilter.Rule>();
                foreach (var r in rs.data)
                {
                    var ccr = new Sora.ForwardingFilter.Rule();
                    ccr.Field = r.field;
                    ccr.Operator = r.op;
                    foreach (var v in r.values)
                    {
                        ccr.Values.Add(v);
                    }
                    ccrs.Add(ccr);
                }
                config.ForwardingFilter.Rules.Add(ccrs);
            }
            if (enableForwardingFilterVersion) config.ForwardingFilter.Version = forwardingFilterVersion;
            if (enableForwardingFilterMetadata) config.ForwardingFilter.Metadata = forwardingFilterMetadataJson;
        }

        sora.Connect(config);
        SetState(State.Started);
        Debug.LogFormat("Sora is Created: signalingUrl={0}, channelId={1}", signalingUrl, channelId);
    }

    private static void GetVideoSize(VideoSize videoSize, out int videoWidth, out int videoHeight)
    {
        videoWidth = 640;
        videoHeight = 480;

        switch (videoSize)
        {
            case VideoSize.QVGA:
                videoWidth = 320;
                videoHeight = 240;
                break;
            case VideoSize.VGA:
                videoWidth = 640;
                videoHeight = 480;
                break;
            case VideoSize.HD:
                videoWidth = 1280;
                videoHeight = 720;
                break;
            case VideoSize.FHD:
                videoWidth = 1920;
                videoHeight = 1080;
                break;
            case VideoSize._4K:
                videoWidth = 3840;
                videoHeight = 2160;
                break;
        }
    }

    public void OnClickEnd()
    {
        DisconnectSora();
    }

    public void OnClickSend()
    {
        if (fixedDataChannelLabels == null || sora == null)
        {
            return;
        }
        // DataChannel メッセージを使って全てのラベルに適当なデータを送る
        foreach (var label in fixedDataChannelLabels)
        {
            string message = "aaa";
            sora.SendMessage(label, System.Text.Encoding.UTF8.GetBytes(message));
        }
    }

    public void OnClickVideoMute()
    {
        if (sora == null)
        {
            return;
        }
        sora.VideoEnabled = !sora.VideoEnabled;
    }
    public void OnClickAudioMute()
    {
        if (sora == null)
        {
            return;
        }
        sora.AudioEnabled = !sora.AudioEnabled;
    }

    public void OnClickSwitchCamera()
    {
        if (sora == null)
        {
            return;
        }
        int videoWidth;
        int videoHeight;
        GetVideoSize(videoSize, out videoWidth, out videoHeight);

        if (captureUnityCamera)
        {
            sora.SwitchCamera(Sora.CameraConfig.FromDeviceCamera(videoCapturerDevice, videoWidth, videoHeight, videoFps));
            captureUnityCamera = false;
        }
        else
        {
            sora.SwitchCamera(Sora.CameraConfig.FromUnityCamera(capturedCamera, 16, videoWidth, videoHeight, videoFps));
            captureUnityCamera = true;
        }
    }

    void OnApplicationQuit()
    {
        DisposeSora();
    }
    public void OnClickHandsfree()
    {
        if (audioOutputHelper == null)
        {
            return;
        }
        audioOutputHelper.SetHandsfree(!audioOutputHelper.IsHandsfree());
    }
}