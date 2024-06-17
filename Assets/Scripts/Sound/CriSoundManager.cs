using UnityEngine;
using System;
using CriWare;
using System.Collections.Generic;
using System.Threading;

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

    /// <summary>キャンセル</summary>
    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    ~CriSoundManager()
    {
        // キャンセルを実行
        _tokenSource.Cancel();

        // プレイヤーを破棄
        _player.Dispose();
    }
}
