using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World_Info", menuName = "Data/World_Info")]
public class World_Info : ScriptableObject
{
    [SerializeField]
    private int _worldNumber = 0;
    public int WorldNumber => _worldNumber;

    [SerializeField]
    private Sprite _worldSprite = null;
    public Sprite WorldSprite => _worldSprite;

    [SerializeField]
    private Sprite _background;
    public Sprite Background => _background;

    [SerializeField]
    private Level_Info[] _levels = null;
    public Level_Info[] Levels => _levels;

    private bool _unlocked;
    public bool Unlocked => _unlocked;

    public void SetUnlocked(bool unlocked)
    {
        _unlocked = unlocked;
    }

    public void SetUnlocked()
    {
        _unlocked = IsUnlocked();
    }

    public bool IsUnlocked()
    {
        foreach (Level_Info level in _levels)
        {
            if (level.Unlocked || level.Available)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsLevel(int levelNum)
    {
        foreach(Level_Info level in _levels)
        {
            if(level.LevelNumber == levelNum)
            {
                return true;
            }
        }

        return false;
    }
}
