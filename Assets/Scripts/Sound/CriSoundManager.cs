using UnityEngine;
using System;
using CriWare;
using System.Collections.Generic;
using System.Threading;

public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    /// <summary>�{�����[��</summary>
    [SerializeField, Range(0, 1), 
        Header("���C���{�����[��")] private float _volume;

    /// <summary>�L���[�V�[�g��</summary>
    [SerializeField,
        Header("�L���[�V�[�g��")] private List<string> _cueSheetName;

    /// <summary>�L���[��</summary>
    [SerializeField,
        Header("�L���[��")] private List<string> _cueName;

    /// <summary>�v���C���[</summary>
    private CriAtomExPlayer _player = new CriAtomExPlayer();

    /// <summary>�L�����Z��</summary>
    private CancellationTokenSource _tokenSource = new CancellationTokenSource();

    ~CriSoundManager()
    {
        // �L�����Z�������s
        _tokenSource.Cancel();

        // �v���C���[��j��
        _player.Dispose();
    }
}
