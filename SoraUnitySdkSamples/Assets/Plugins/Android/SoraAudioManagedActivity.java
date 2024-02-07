package jp.shiguredo.sora.unityaudiomanager;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;
import jp.shiguredo.sora.audiomanager.SoraAudioManager;
import jp.shiguredo.sora.audiomanager.SoraAudioManagerFactory;
import jp.shiguredo.sora.audiomanager.SoraThreadUtils;

public class SoraAudioManagedActivity extends UnityPlayerActivity {

    SoraAudioManager soraAudioManager;

    private class OnChangeRouteObserver implements SoraAudioManager.OnChangeRouteObserver {
        SoraAudioManager.OnChangeRouteObserver observer;

        public OnChangeRouteObserver(SoraAudioManager.OnChangeRouteObserver observer) {
            this.observer = observer;
        }

        public void OnChangeRoute() {
            //メインスレッドでない場合はメインスレッドで再実行
            if (!SoraThreadUtils.runOnMainThread(() -> OnChangeRoute())) {
                return;
            }

            if (this.observer == null) {
                return;
            }
            this.observer.OnChangeRoute();
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        soraAudioManager = SoraAudioManagerFactory.create(getApplicationContext());
    }

    @Override
    protected void onDestroy() {
        stopAudioManager();
        soraAudioManager = null;
        super.onDestroy();
    }
    
    public void startAudioManager(SoraAudioManager.OnChangeRouteObserver observer) {
        //メインスレッドでない場合はメインスレッドで再実行
        if (!SoraThreadUtils.runOnMainThread(() -> startAudioManager(observer))) {
            return;
        }

        soraAudioManager.start(new OnChangeRouteObserver(observer));
    }

    public void stopAudioManager() {
        //メインスレッドでない場合はメインスレッドで再実行
        if (!SoraThreadUtils.runOnMainThread(() -> stopAudioManager())) {
            return;
        }

        if (soraAudioManager == null) {
            return;
        }
        soraAudioManager.stop();
    }

    public void setHandsfree(boolean on) {
        //メインスレッドでない場合はメインスレッドで再実行
        if (!SoraThreadUtils.runOnMainThread(() -> setHandsfree(on))) {
            return;
        }

        if (soraAudioManager == null) {
            return;
        }
        soraAudioManager.setHandsfree(on);
    }

    public boolean isHandsfree() {
        if (soraAudioManager == null) {
            return false;
        }
        return soraAudioManager.isHandsfree();
    }
}
