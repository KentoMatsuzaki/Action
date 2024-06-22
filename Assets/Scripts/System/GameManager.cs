using UnityEngine;

/// <summary>ê_ÉNÉâÉX</summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] float _bgmVolume = 0.5f;
    CriSoundManager _sound;
    UIManager _ui;
    int _index = 10;

    void Start()
    {
        _sound = CriSoundManager.Instance;
        _ui = UIManager.Instance;
        StartBGM();
    }

    private void StartBGM()
    {
        _sound.Play("CueSheet_1", "BGM", _bgmVolume);
    }

    public void SetXPBar()
    {
        _ui.SetXpBar(GetXPBar() + 0.1f * _index);
        _index--;
    }

    private float GetXPBar()
    {
        return _ui.GetXPBar();
    }

    public void OnLevelUp()
    {

    }
}
