using UnityEngine;
using System;
using CriWare;
using System.Collections.Generic;

public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    /// <summary>ボリューム</summary>
    [SerializeField, Range(0, 1), 
        Header("メインボリューム")] private float _volume;

    /// <summary>キューシート名</summary>
    [SerializeField,
        Header("キューシート名")] private List<string> _cueSheetName;

    /// <summary>キュー名</summary>
    [SerializeField,
        Header("キュー名")] private List<string> _cueName;

    /// <summary>プレイヤー</summary>
    private CriAtomExPlayer _player = new CriAtomExPlayer();

    /// <summary>再生音のコレクション</summary>
    private Dictionary<int, CriAtomExPlayback> _playbackDic = 
        new Dictionary<int, CriAtomExPlayback>();

    /// <summary>再生音のID</summary>
    private static int _nextPlaybackIndex = 0;

    /// <summary>音声を再生してコレクションに登録する</summary>
    /// <param name="cueSheetName">キューシート名</param>
    /// <param name="cueName">キュー名</param>
    /// <param name="volume">ボリューム</param>
    public void Play(string cueSheetName, string cueName, float volume)
    {
        // プレイヤーを設定
        var currentAcb = CriAtom.GetAcb(cueSheetName);
        _player.SetCue(currentAcb, cueName);
        _player.SetVolume(volume * _volume);

        // 再生音を取得
        var playback = _player.Start();

        // インデックスを更新
        _nextPlaybackIndex++;

        // 再生音を登録
        _playbackDic[_nextPlaybackIndex] = playback;
    }

    /// <summary>再生音をポーズ</summary>
    /// <param name="index">コレクションのキー</param>
    public void Pause(int index)
    {
        if(_playbackDic.ContainsKey(index)) _playbackDic[index].Pause();
    }

    /// <summary>再生音をポーズ解除</summary>
    /// <param name="index">コレクションのキー</param>
    public void Resume(int index)
    {
        if(_playbackDic.ContainsKey(index)) 
            _playbackDic[index].Resume(CriAtomEx.ResumeMode.AllPlayback);
    }

    /// <summary>再生音を停止</summary>
    /// <param name="index">コレクションのキー</param>
    public void Stop(int index)
    {
        if(_playbackDic.ContainsKey(index))
            _playbackDic[index].Stop(false);
    }

    /// <summary>全ての再生音を停止</summary>
    public void StopAll()
    {
        _player.Stop(false);
        _playbackDic.Clear();
    }
}
