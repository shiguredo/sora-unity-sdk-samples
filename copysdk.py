# 開発用スクリプト。 ..\sora-unity-sdk にあるソースと bundle をこのプロジェクトにコピーする
import argparse
import logging
import os
import shutil

from install import enum_all_files, mkdir_p, rm_rf

logging.basicConfig(level=logging.INFO)


def copy_if_exists(srcfile, dstfile):
    if not os.path.exists(srcfile):
        logging.info(f"Not found: {srcfile}")
        return
    if os.path.isdir(srcfile):
        rm_rf(dstfile)
        shutil.copytree(srcfile, dstfile)
    else:
        dstdir = os.path.dirname(dstfile)
        mkdir_p(dstdir)
        shutil.copyfile(srcfile, dstfile)
    logging.info(f"[COPY] {srcfile} to {dstfile}")


BASE_DIR = os.path.abspath(os.path.dirname(__file__))


def main():
    os.chdir(BASE_DIR)

    parser = argparse.ArgumentParser()
    parser.add_argument("--sdk-path")
    # WSL で sora-unity-sdk の Android 版ビルドをして Windows で利用しようと思った場合、
    # sora-unity-sdk のパスが通常とは別の場所になってしまうので、Android だけ別途設定可能にする
    parser.add_argument("--android-sdk-path")
    parser.add_argument("--relwithdebinfo", action="store_true")
    parser.add_argument("--debug", action="store_true")

    args = parser.parse_args()

    if args.sdk_path is None:
        src_base = os.path.join("..", "sora-unity-sdk")
    else:
        src_base = args.sdk_path

    if args.android_sdk_path is None:
        android_src_base = src_base
    else:
        android_src_base = args.android_sdk_path

    dst_base = os.path.join("SoraUnitySdkSamples", "Assets")

    configdir = "release"
    if args.debug:
        configdir = "debug"

    if args.relwithdebinfo:
        vsconfigdir = "RelWithDebInfo"
    else:
        vsconfigdir = "Release"

    # Source
    for file in enum_all_files(os.path.join(src_base, "Sora"), os.path.join(src_base, "Sora")):
        copy_if_exists(
            os.path.join(src_base, "Sora", file), os.path.join(dst_base, "SoraUnitySdk", file)
        )

    if args.android_sdk_path is not None:
        for file in enum_all_files(
            os.path.join(android_src_base, "Sora"), os.path.join(android_src_base, "Sora")
        ):
            copy_if_exists(
                os.path.join(android_src_base, "Sora", file),
                os.path.join(dst_base, "SoraUnitySdk", file),
            )

    # Windows
    copy_if_exists(
        os.path.join(
            src_base,
            "_build",
            "windows_x86_64",
            configdir,
            "sora_unity_sdk",
            vsconfigdir,
            "SoraUnitySdk.dll",
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "windows", "x86_64", "SoraUnitySdk.dll"),
    )
    # copy_if_exists(
    #    os.path.join(src_base, '_build', 'windows_x86_64', configdir, 'sora_unity_sdk', vsconfigdir, 'SoraUnitySdk.pdb'),
    #    os.path.join(dst_base, 'Plugins', 'SoraUnitySdk', 'windows', 'x86_64', 'SoraUnitySdk.pdb'))

    # macOS
    copy_if_exists(
        os.path.join(
            src_base, "_build", "macos_x86_64", configdir, "sora_unity_sdk", "SoraUnitySdk.bundle"
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "macos", "x86_64", "SoraUnitySdk.bundle"),
    )
    copy_if_exists(
        os.path.join(
            src_base, "_build", "macos_arm64", configdir, "sora_unity_sdk", "SoraUnitySdk.bundle"
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "macos", "arm64", "SoraUnitySdk.bundle"),
    )

    # iOS
    copy_if_exists(
        os.path.join(src_base, "_build", "ios", configdir, "sora_unity_sdk", "libSoraUnitySdk.a"),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "ios", "libSoraUnitySdk.a"),
    )
    copy_if_exists(
        os.path.join(src_base, "_install", "ios", configdir, "webrtc", "lib", "libwebrtc.a"),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "ios", "libwebrtc.a"),
    )
    copy_if_exists(
        os.path.join(src_base, "_install", "ios", configdir, "boost", "lib", "libboost_json.a"),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "ios", "libboost_json.a"),
    )
    copy_if_exists(
        os.path.join(
            src_base, "_install", "ios", configdir, "boost", "lib", "libboost_filesystem.a"
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "ios", "libboost_filesystem.a"),
    )
    copy_if_exists(
        os.path.join(src_base, "_install", "ios", configdir, "sora", "lib", "libsora.a"),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "ios", "libsora.a"),
    )
    # copy_if_exists(
    #    os.path.join(src_base, '_build', 'sora-unity-sdk', 'ios', 'libSoraUnitySdk.a'),
    #    os.path.join(dst_base, 'Plugins', 'SoraUnitySdk', 'ios', 'libSoraUnitySdk.a'))
    # copy_if_exists(
    #    os.path.join(src_base, '_install', 'ios', 'webrtc', 'lib', 'libwebrtc.a'),
    #    os.path.join(dst_base, 'Plugins', 'SoraUnitySdk', 'ios', 'libwebrtc.a'))
    # copy_if_exists(
    #    os.path.join(src_base, '_install', 'ios', 'boost', 'lib', 'libboost_json.a'),
    #    os.path.join(dst_base, 'Plugins', 'SoraUnitySdk', 'ios', 'libboost_json.a'))

    # Android
    copy_if_exists(
        os.path.join(
            android_src_base, "_build", "android", configdir, "sora_unity_sdk", "libSoraUnitySdk.so"
        ),
        os.path.join(
            dst_base, "Plugins", "SoraUnitySdk", "android", "arm64-v8a", "libSoraUnitySdk.so"
        ),
    )
    copy_if_exists(
        os.path.join(
            android_src_base, "_install", "android", configdir, "webrtc", "jar", "webrtc.jar"
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "android", "webrtc.jar"),
    )

    # Ubuntu x86_64
    copy_if_exists(
        os.path.join(
            android_src_base,
            "_build",
            "ubuntu-20.04_x86_64",
            configdir,
            "sora_unity_sdk",
            "libSoraUnitySdk.so",
        ),
        os.path.join(dst_base, "Plugins", "SoraUnitySdk", "linux", "x86_64", "libSoraUnitySdk.so"),
    )


if __name__ == "__main__":
    main()
