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

    /// <summary>プレイヤーのSE</summary>
    [SerializeField,
        Header("キュー名")] public List<string> _playerCueNames;

    /// <summary>敵のSE</summary>
    [SerializeField,
        Header("キュー名")] public List<string> _enemyCueNames;

    /// <summary>アクションのSE</summary>
    [SerializeField,
        Header("キュー名")] public List<string> _actionCueNames;

    /// <summary>プレイヤー</summary>
    private CriAtomExPlayer _player;

    /// <summary>再生音のコレクション</summary>
    /// <summary>Key = キュー名，Value = Playback</summary>
    private Dictionary<string, CriAtomExPlayback> _playbackDic = 
        new Dictionary<string, CriAtomExPlayback>();

    void Start()
    {
        _player = new CriAtomExPlayer();
    }

    /// <summary>音声を再生してコレクションに登録する</summary>
    /// <param name="cueName">コレクションのキー</param>
    public void Play(string cueSheetName, string cueName, float volume)
    {
        // プレイヤーを設定
        var currentAcb = CriAtom.GetAcb(cueSheetName);
        _player.SetCue(currentAcb, cueName);
        _player.SetVolume(volume * _volume);

        // 再生音を取得
        var playback = _player.Start();

        // 再生音を登録
        _playbackDic[cueName] = playback;
    }

    /// <summary>再生音をポーズ</summary>
    /// <param name="cueName">コレクションのキー</param>
    public void Pause(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName)) _playbackDic[cueName].Pause();
    }

    /// <summary>再生音をポーズ解除</summary>
    /// <param name="cueName">コレクションのキー</param>
    public void Resume(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName)) 
            _playbackDic[cueName].Resume(CriAtomEx.ResumeMode.AllPlayback);
    }

    /// <summary>再生音を停止</summary>
    /// <param name="cueName">コレクションのキー</param>
    public void Stop(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName))
            _playbackDic[cueName].Stop(false);
    }

    /// <summary>全ての再生音を停止</summary>
    public void StopAll()
    {
        _player.Stop(false);
        _playbackDic.Clear();
    }
}
