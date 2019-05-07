using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_Info", menuName = "Data/Level_Info")]
public class Level_Info : ScriptableObject
{
    [SerializeField]
    private int _levelNumber = 0;
    public int LevelNumber => _levelNumber;

    [SerializeField]
    private Sprite _musicSprite = null;
    public Sprite MusicSprite => _musicSprite;

    [SerializeField]
    private Sprite _comicSprite = null;
    public Sprite ComicSprite => _comicSprite;

    [SerializeField]
    private AudioClip _audio = null;
    public AudioClip Audio => _audio;

    private bool _unlocked = false;
    public bool Unlocked => _unlocked;

    private bool _available = false;
    public bool Available => _available || _unlocked;

    public void SetUnlocked(bool unlocked)
    {
        _unlocked = unlocked;
    }

    public void SetUnlocked(int maxLevelUnlocked)
    {
        if(_levelNumber <= maxLevelUnlocked)
        {
            _unlocked = true;
        }
        else
        {
            _unlocked = false;
        }
    }

    public void SetAvailable(int levelNum)
    {
        if(_levelNumber == levelNum)
        {
            _available = true;
        }
        else
        {
            _available = _unlocked;
        }
    }
}
