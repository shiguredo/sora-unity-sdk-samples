using UnityEditor;
using UnityEngine;
using System.Text;
using System;

public class GetStatsWindow : EditorWindow
{
    private StringBuilder _stringBuilder = new StringBuilder();
    
    private string[] whitelist = new string[] { "GetStats:" };

    // メニューからウィンドウを表示する
    [MenuItem("Sora/GetStats/StatsWindow")]
    public static void OpenWindow()
    {
        GetStatsWindow.GetWindow(typeof(GetStatsWindow));
    }

    private void OnEnable()
    {
        // コンソールのログを取得するようにする
        Application.logMessageReceived += OnReceiveLog;

    }

    private void OnReceiveLog(string logText, string stackTrace, LogType logType)
    {

        _stringBuilder.Clear();

        if (0 < whitelist.Length)
        {
            // Getstats 以外は不要なので除外
            for (int i = 0; i < whitelist.Length; i++)
            {
                if (whitelist[i] == string.Empty)
                {
                    return;
                }
                else if (!logText.Contains(whitelist[i]))
                {
                    return;
                }
            }
        }
        _stringBuilder.Append(logText);
    }

    // GetStats のラインナップ
    [Serializable]
    public class InputJson
    {
        public GetStats[] getstats;
    }

    [Serializable]
    public class GetStats
    {
        public string type;
        public string id;
        public long timestamp;
        public string trackIdentifier;
        public string kind;
        public float audioLevel;
        public float totalAudioEnergy;
        public float totalSamplesDuration;
        public int echoReturnLoss;
        public float echoReturnLossEnhancement;
        public string fingerprint;
        public string fingerprintAlgorithm;
        public string base64Certificate;
        public string transportId;
        public int payloadType;
        public string mimeType;
        public int clockRate;
        public int channels;
        public string sdpFmtpLine;
        public string label;
        public string protocol;
        public int dataChannelIdentifier;
        public string state;
        public int messagesSent;
        public int bytesSent;
        public int messagesReceived;
        public int bytesReceived;
        public string localCandidateId;
        public string remoteCandidateId;
        public float priority;
        public bool nominated;
        public bool writable;
        public int packetsSent;
        public int packetsReceived;
        public float totalRoundTripTime;
        public int requestsReceived;
        public int requestsSent;
        public int responsesReceived;
        public int responsesSent;
        public int consentRequestsSent;
        public int packetsDiscardedOnSend;
        public int bytesDiscardedOnSend;
        public float currentRoundTripTime;
        public int availableOutgoingBitrate;
        public bool isRemote;
        public string networkType;
        public string ip;
        public string address;
        public int port;
        public string relayProtocol;
        public string candidateType;
        public string url;
        public bool vpn;
        public string networkAdapterType;
        public string mediaSourceId;
        public bool remoteSource;
        public bool ended;
        public bool detached;
        public int frameWidth;
        public int frameHeight;
        public int framesSent;
        public int hugeFramesSent;
        public string streamIdentifier;
        public string[] trackIds;
        public long ssrc;
        public string trackId;
        public string codecId;
        public string mediaType;
        public int retransmittedPacketsSent;
        public int headerBytesSent;
        public int retransmittedBytesSent;
        public int targetBitrate;
        public int nackCount;
        public string remoteId;
        public int framesEncoded;
        public int keyFramesEncoded;
        public float totalEncodeTime;
        public int totalEncodedBytesTarget;
        public int framesPerSecond;
        public float totalPacketSendDelay;
        public string qualityLimitationReason;
        public Qualitylimitationdurations qualityLimitationDurations;
        public int qualityLimitationResolutionChanges;
        public string encoderImplementation;
        public int firCount;
        public int pliCount;
        public int qpSum;
        public int dataChannelsOpened;
        public int dataChannelsClosed;
        public float jitter;
        public int packetsLost;
        public string localId;
        public float roundTripTime;
        public int fractionLost;
        public int roundTripTimeMeasurements;
        public string dtlsState;
        public string selectedCandidatePairId;
        public string localCertificateId;
        public string remoteCertificateId;
        public string tlsVersion;
        public string dtlsCipher;
        public string dtlsRole;
        public string srtpCipher;
        public int selectedCandidatePairChanges;
        public string iceRole;
        public string iceLocalUsernameFragment;
        public string iceState;
        public int width;
        public int height;
        public int frames;
    }

    [Serializable]
    public class Qualitylimitationdurations
    {
        public int bandwidth;
        public int cpu;
        public float none;
        public int other;
    }

    private void OnGUI()
    {
        string logText = _stringBuilder.ToString();
        string line;

        if (!string.IsNullOrEmpty(logText))
        {
            // いい感じに編集してあげないと JsonUtility が機能しないのでなんとかする
            line = logText.Replace("GetStats: ", "\"getstats\":");
            line = line.Replace("'", "\"");
            line = "{" + line + "}";

            InputJson inputJson = JsonUtility.FromJson<InputJson>(line);
            EditorGUILayout.LabelField("test");
            // 値の確認
            // media-source の値
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].type);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].id);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].timestamp);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].trackIdentifier);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].kind);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].audioLevel);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].totalAudioEnergy);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].totalSamplesDuration);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].echoReturnLoss);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[0].echoReturnLossEnhancement);
            // certificate の値
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].type);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].id);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].timestamp);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].fingerprint);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].fingerprintAlgorithm);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[1].base64Certificate);
            // certificate の値
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].type);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].id);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].timestamp);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].fingerprint);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].fingerprintAlgorithm);
            EditorGUILayout.LabelField("item type " + inputJson.getstats[2].base64Certificate);
        }
    }

    private void OnDisable()
    {
        // ログを取得をやめる
        Application.logMessageReceived -= OnReceiveLog;
    }
}
