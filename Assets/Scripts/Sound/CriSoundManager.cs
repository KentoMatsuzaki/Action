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

    /// <summary>�L���[��</summary>
    [SerializeField,
        Header("�L���[��")] private List<string> _cueName;

    /// <summary>�v���C���[</summary>
    private CriAtomExPlayer _player = new CriAtomExPlayer();

    /// <summary>�Đ����̃R���N�V����</summary>
    private Dictionary<int, CriAtomExPlayback> _playbackDic = 
        new Dictionary<int, CriAtomExPlayback>();

    /// <summary>�Đ�����ID</summary>
    private static int _nextPlaybackIndex = 0;

    /// <summary>�������Đ����ăR���N�V�����ɓo�^����</summary>
    /// <param name="cueSheetName">�L���[�V�[�g��</param>
    /// <param name="cueName">�L���[��</param>
    /// <param name="volume">�{�����[��</param>
    public void Play(string cueSheetName, string cueName, float volume)
    {
        // �v���C���[��ݒ�
        var currentAcb = CriAtom.GetAcb(cueSheetName);
        _player.SetCue(currentAcb, cueName);
        _player.SetVolume(volume * _volume);

        // �Đ������擾
        var playback = _player.Start();

        // �C���f�b�N�X���X�V
        _nextPlaybackIndex++;

        // �Đ�����o�^
        _playbackDic[_nextPlaybackIndex] = playback;
    }

    /// <summary>�Đ������|�[�Y</summary>
    /// <param name="index">�R���N�V�����̃L�[</param>
    public void Pause(int index)
    {
        if(_playbackDic.ContainsKey(index)) _playbackDic[index].Pause();
    }

    /// <summary>�Đ������|�[�Y����</summary>
    /// <param name="index">�R���N�V�����̃L�[</param>
    public void Resume(int index)
    {
        if(_playbackDic.ContainsKey(index)) 
            _playbackDic[index].Resume(CriAtomEx.ResumeMode.AllPlayback);
    }

    /// <summary>�Đ������~</summary>
    /// <param name="index">�R���N�V�����̃L�[</param>
    public void Stop(int index)
    {
        if(_playbackDic.ContainsKey(index))
            _playbackDic[index].Stop(false);
    }

    /// <summary>�S�Ă̍Đ������~</summary>
    public void StopAll()
    {
        _player.Stop(false);
        _playbackDic.Clear();
    }
}
