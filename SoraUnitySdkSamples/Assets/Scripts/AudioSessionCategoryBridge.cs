using System.Runtime.InteropServices;
using UnityEngine;

namespace iOSNative
{
    public static class AudioSessionCategoryBridge
    {
        // Objective-Cの関数とのインターフェース
        // 試行錯誤の結果で色々用意しているが、__setAudioSessionCategoryAmbient しか使っていない。
        [DllImport("__Internal")]
        private static extern void __setAudioSessionCategoryAmbient();

        [DllImport("__Internal")]
        private static extern void __setAudioSessionCategoryPlayAndRecord();

        [DllImport("__Internal")]
        private static extern string __getAudioSessionCategory();

        /// <summary>
        /// AVAudioSessionのカテゴリをAmbientに設定します。
        /// このメソッドはiOS上でのみ動作します。
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_IOS")]
        public static void SetAudioSessionCategoryAmbient()
        {
            __setAudioSessionCategoryAmbient();
        }

        /// <summary>
        /// AVAudioSessionのカテゴリをPlayAndRecordに設定します。
        /// このメソッドはiOS上でのみ動作します。
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_IOS")]
        public static void SetAudioSessionCategoryPlayAndRecord()
        {
            __setAudioSessionCategoryPlayAndRecord();
        }

        public static void LogAudioSessionCategory()
        {
            string category = __getAudioSessionCategory();
            Debug.Log($"Current AVAudioSession category: {category}");
        }

        // 事象を見るにはカテゴリを起動時に変えられると困るので利用しない
        // ゲームが起動するときに、AVAudioSessionのカテゴリを自動的にAmbientに設定します。
        //[RuntimeInitializeOnLoadMethod]
        //static void Initialize()
        //{
        //    SetAudioSessionCategoryAmbient();
    }
}

