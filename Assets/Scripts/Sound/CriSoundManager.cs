using UnityEngine;
using System;
using CriWare;
using System.Collections.Generic;

public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    /// <summary>�{�����[��</summary>
    [SerializeField, Range(0, 1), 
        Header("���C���{�����[��")] private float _volume;

    /// <summary>�L���[�V�[�g��</summary>
    [SerializeField,
        Header("�L���[�V�[�g��")] private List<string> _cueSheetName;

    /// <summary>�v���C���[��SE</summary>
    [SerializeField,
        Header("�L���[��")] public List<string> _playerCueNames;

    /// <summary>�G��SE</summary>
    [SerializeField,
        Header("�L���[��")] public List<string> _enemyCueNames;

    /// <summary>�A�N�V������SE</summary>
    [SerializeField,
        Header("�L���[��")] public List<string> _actionCueNames;

    /// <summary>�v���C���[</summary>
    private CriAtomExPlayer _player;

    /// <summary>�Đ����̃R���N�V����</summary>
    /// <summary>Key = �L���[���CValue = Playback</summary>
    private Dictionary<string, CriAtomExPlayback> _playbackDic = 
        new Dictionary<string, CriAtomExPlayback>();

    void Start()
    {
        _player = new CriAtomExPlayer();
    }

    /// <summary>�������Đ����ăR���N�V�����ɓo�^����</summary>
    /// <param name="cueName">�R���N�V�����̃L�[</param>
    public void Play(string cueSheetName, string cueName, float volume)
    {
        // �v���C���[��ݒ�
        var currentAcb = CriAtom.GetAcb(cueSheetName);
        _player.SetCue(currentAcb, cueName);
        _player.SetVolume(volume * _volume);

        // �Đ������擾
        var playback = _player.Start();

        // �Đ�����o�^
        _playbackDic[cueName] = playback;
    }

    /// <summary>�Đ������|�[�Y</summary>
    /// <param name="cueName">�R���N�V�����̃L�[</param>
    public void Pause(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName)) _playbackDic[cueName].Pause();
    }

    /// <summary>�Đ������|�[�Y����</summary>
    /// <param name="cueName">�R���N�V�����̃L�[</param>
    public void Resume(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName)) 
            _playbackDic[cueName].Resume(CriAtomEx.ResumeMode.AllPlayback);
    }

    /// <summary>�Đ������~</summary>
    /// <param name="cueName">�R���N�V�����̃L�[</param>
    public void Stop(string cueName)
    {
        if(_playbackDic.ContainsKey(cueName))
            _playbackDic[cueName].Stop(false);
    }

    /// <summary>�S�Ă̍Đ������~</summary>
    public void StopAll()
    {
        _player.Stop(false);
        _playbackDic.Clear();
    }
}
