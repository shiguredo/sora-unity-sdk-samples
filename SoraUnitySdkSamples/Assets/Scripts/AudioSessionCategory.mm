#import <AVFoundation/AVFoundation.h>

@interface AudioSessionCategory : NSObject

// AVAudioSessionのカテゴリをAmbientに設定します。
+ (void)setAmbient;

// AVAudioSessionのカテゴリをPlayAndRecordに設定します。
+ (void)setPlayAndRecord;

@end

@implementation AudioSessionCategory

+ (void)setAmbient {
    // AVAudioSessionの共有インスタンスを取得します。
    AVAudioSession* audioSession = [AVAudioSession sharedInstance];

    // エラーを受け取るための変数を宣言します。
    NSError *error;

    // AVAudioSessionのカテゴリをAmbientに設定します。
    [audioSession setCategory:AVAudioSessionCategoryAmbient error:&error];

    // カテゴリの設定中にエラーが発生した場合、エラーメッセージをログに出力します。
    if (error) {
        NSLog(@"Error setting audio session category to Ambient: %@", error.localizedDescription);
    }
}

+ (void)setPlayAndRecord {
    AVAudioSession* audioSession = [AVAudioSession sharedInstance];
    NSError *error;
    [audioSession setCategory:AVAudioSessionCategoryPlayAndRecord error:&error];
    if (error) {
        NSLog(@"Error setting audio session category to PlayAndRecord: %@", error.localizedDescription);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void __setAudioSessionCategoryPlayAndRecord() {
    [AudioSessionCategory setPlayAndRecord];
}

void __setAudioSessionCategoryAmbient() {
    [AudioSessionCategory setAmbient];
}

const char* __getAudioSessionCategory() {
    AVAudioSessionCategory category = [[AVAudioSession sharedInstance] category];
    return [category UTF8String];
}

#ifdef __cplusplus
}
#endif
