import subprocess
import logging
import os
import urllib.parse
import zipfile
import tarfile
import shutil
import platform
import argparse
from typing import Callable, Optional, List, Union


logging.basicConfig(level=logging.DEBUG)


SORA_UNITY_SDK_VERSION="2023.2.0"


class ChangeDirectory(object):
    def __init__(self, cwd):
        self._cwd = cwd

    def __enter__(self):
        self._old_cwd = os.getcwd()
        logging.debug(f'pushd {self._old_cwd} --> {self._cwd}')
        os.chdir(self._cwd)

    def __exit__(self, exctype, excvalue, trace):
        logging.debug(f'popd {self._old_cwd} <-- {self._cwd}')
        os.chdir(self._old_cwd)
        return False


def cd(cwd):
    return ChangeDirectory(cwd)


def cmd(args, **kwargs):
    logging.debug(f'+{args} {kwargs}')
    if 'check' not in kwargs:
        kwargs['check'] = True
    if 'resolve' in kwargs:
        resolve = kwargs['resolve']
        del kwargs['resolve']
    else:
        resolve = True
    if resolve:
        args = [shutil.which(args[0]), *args[1:]]
    return subprocess.run(args, **kwargs)


def download(url: str, output_dir: Optional[str] = None, filename: Optional[str] = None) -> str:
    if filename is None:
        output_path = urllib.parse.urlparse(url).path.split('/')[-1]
    else:
        output_path = filename

    if output_dir is not None:
        output_path = os.path.join(output_dir, output_path)

    if os.path.exists(output_path):
        return output_path

    try:
        if shutil.which('curl') is not None:
            cmd(["curl", "-fLo", output_path, url])
        else:
            cmd(["wget", "-cO", output_path, url])
    except Exception:
        # ゴミを残さないようにする
        if os.path.exists(output_path):
            os.remove(output_path)
        raise

    return output_path


def rm_rf(path: str):
    if not os.path.exists(path):
        logging.debug(f'rm -rf {path} => path not found')
        return
    if os.path.isfile(path) or os.path.islink(path):
        os.remove(path)
        logging.debug(f'rm -rf {path} => file removed')
    if os.path.isdir(path):
        shutil.rmtree(path)
        logging.debug(f'rm -rf {path} => directory removed')


def mkdir_p(path: str):
    if os.path.exists(path):
        logging.debug(f'mkdir -p {path} => already exists')
        return
    os.makedirs(path, exist_ok=True)
    logging.debug(f'mkdir -p {path} => directory created')


# アーカイブが単一のディレクトリに全て格納されているかどうかを調べる。
#
# 単一のディレクトリに格納されている場合はそのディレクトリ名を返す。
# そうでない場合は None を返す。
def _is_single_dir(infos: List[Union[zipfile.ZipInfo, tarfile.TarInfo]],
                   get_name: Callable[[Union[zipfile.ZipInfo, tarfile.TarInfo]], str],
                   is_dir: Callable[[Union[zipfile.ZipInfo, tarfile.TarInfo]], bool]) -> Optional[str]:
    # tarfile: ['path', 'path/to', 'path/to/file.txt']
    # zipfile: ['path/', 'path/to/', 'path/to/file.txt']
    # どちらも / 区切りだが、ディレクトリの場合、後ろに / が付くかどうかが違う
    dirname = None
    for info in infos:
        name = get_name(info)
        n = name.rstrip('/').find('/')
        if n == -1:
            # ルートディレクトリにファイルが存在している
            if not is_dir(info):
                return None
            dir = name.rstrip('/')
        else:
            dir = name[0:n]
        # ルートディレクトリに２個以上のディレクトリが存在している
        if dirname is not None and dirname != dir:
            return None
        dirname = dir

    return dirname


def is_single_dir_tar(tar: tarfile.TarFile) -> Optional[str]:
    return _is_single_dir(tar.getmembers(), lambda t: t.name, lambda t: t.isdir())


def is_single_dir_zip(zip: zipfile.ZipFile) -> Optional[str]:
    return _is_single_dir(zip.infolist(), lambda z: z.filename, lambda z: z.is_dir())


# 解凍した上でファイル属性を付与する
def _extractzip(z: zipfile.ZipFile, path: str):
    z.extractall(path)
    if platform.system() == 'Windows':
        return
    for info in z.infolist():
        if info.is_dir():
            continue
        filepath = os.path.join(path, info.filename)
        mod = info.external_attr >> 16
        if (mod & 0o120000) == 0o120000:
            # シンボリックリンク
            with open(filepath, 'r') as f:
                src = f.read()
            os.remove(filepath)
            with cd(os.path.dirname(filepath)):
                if os.path.exists(src):
                    os.symlink(src, filepath)
        if os.path.exists(filepath):
            # 普通のファイル
            os.chmod(filepath, mod & 0o777)


# zip または tar.gz ファイルを展開する。
#
# 展開先のディレクトリは {output_dir}/{output_dirname} となり、
# 展開先のディレクトリが既に存在していた場合は削除される。
#
# もしアーカイブの内容が単一のディレクトリであった場合、
# そのディレクトリは無いものとして展開される。
#
# つまりアーカイブ libsora-1.23.tar.gz の内容が
# ['libsora-1.23', 'libsora-1.23/file1', 'libsora-1.23/file2']
# であった場合、extract('libsora-1.23.tar.gz', 'out', 'libsora') のようにすると
# - out/libsora/file1
# - out/libsora/file2
# が出力される。
#
# また、アーカイブ libsora-1.23.tar.gz の内容が
# ['libsora-1.23', 'libsora-1.23/file1', 'libsora-1.23/file2', 'LICENSE']
# であった場合、extract('libsora-1.23.tar.gz', 'out', 'libsora') のようにすると
# - out/libsora/libsora-1.23/file1
# - out/libsora/libsora-1.23/file2
# - out/libsora/LICENSE
# が出力される。
def extract(file: str, output_dir: str, output_dirname: str, filetype: Optional[str] = None):
    path = os.path.join(output_dir, output_dirname)
    logging.info(f"Extract {file} to {path}")
    if filetype == 'gzip' or file.endswith('.tar.gz'):
        rm_rf(path)
        with tarfile.open(file) as t:
            dir = is_single_dir_tar(t)
            if dir is None:
                os.makedirs(path, exist_ok=True)
                t.extractall(path)
            else:
                logging.info(f"Directory {dir} is stripped")
                path2 = os.path.join(output_dir, dir)
                rm_rf(path2)
                t.extractall(output_dir)
                if path != path2:
                    logging.debug(f"mv {path2} {path}")
                    os.replace(path2, path)
    elif filetype == 'zip' or file.endswith('.zip'):
        rm_rf(path)
        with zipfile.ZipFile(file) as z:
            dir = is_single_dir_zip(z)
            if dir is None:
                os.makedirs(path, exist_ok=True)
                # z.extractall(path)
                _extractzip(z, path)
            else:
                logging.info(f"Directory {dir} is stripped")
                path2 = os.path.join(output_dir, dir)
                rm_rf(path2)
                # z.extractall(output_dir)
                _extractzip(z, output_dir)
                if path != path2:
                    logging.debug(f"mv {path2} {path}")
                    os.replace(path2, path)
    else:
        raise Exception('file should end with .tar.gz or .zip')


# dir 以下にある全てのファイルパスを、dir2 からの相対パスで返す
def enum_all_files(dir, dir2):
    for root, _, files in os.walk(dir):
        for file in files:
            yield os.path.relpath(os.path.join(root, file), dir2)


BASE_DIR = os.path.abspath(os.path.dirname(__file__))


def main():
    os.chdir(BASE_DIR)

    parser = argparse.ArgumentParser()
    # これを指定している場合、Sora Unity SDK のダウンロードをせず、このディレクトリの内容をコピーする
    # GitHub Actions からバイナリをダウンロードしてきたものを反映させる時用のフラグ
    parser.add_argument("--sdk-path")

    args = parser.parse_args()

    if args.sdk_path is None:
        rm_rf('SoraUnitySdk.zip')
        rm_rf('SoraUnitySdk')
        url = f'https://github.com/shiguredo/sora-unity-sdk/releases/download/{SORA_UNITY_SDK_VERSION}/SoraUnitySdk.zip'
        path = download(url, '.')
        extract(path, '.', 'SoraUnitySdk')

    # 既存のファイル（特にメタデータ系）が残ってる可能性があるので
    # １個１個ファイルをコピーしていく
    sdk_path = 'SoraUnitySdk' if args.sdk_path is None else args.sdk_path
    for file in enum_all_files(sdk_path, sdk_path):
        dst_base = os.path.join('SoraUnitySdkSamples', 'Assets')
        # このディレクトリだけは全部置き換える
        if 'SoraUnitySdk.bundle' in file:
            continue
        srcfile = os.path.join(sdk_path, file)
        dstfile = os.path.join(dst_base, file)
        dstdir = os.path.dirname(dstfile)
        mkdir_p(dstdir)
        shutil.copyfile(srcfile, dstfile)
    # .bundle ディレクトリの置き換え
    for root, dirs, _ in os.walk(sdk_path):
        dst_base = os.path.join('SoraUnitySdkSamples', 'Assets')
        for dir in dirs:
            if dir == 'SoraUnitySdk.bundle':
                bundle_dir = os.path.relpath(os.path.join(root, dir), sdk_path)
                src_bundle_dir = os.path.join(sdk_path, bundle_dir)
                dst_bundle_dir = os.path.join(dst_base, bundle_dir)
                mkdir_p(os.path.dirname(dst_bundle_dir))
                rm_rf(dst_bundle_dir)
                shutil.copytree(src_bundle_dir, dst_bundle_dir)

    if args.sdk_path is None:
        rm_rf('SoraUnitySdk.zip')
        rm_rf('SoraUnitySdk')


if __name__ == '__main__':
    main()
