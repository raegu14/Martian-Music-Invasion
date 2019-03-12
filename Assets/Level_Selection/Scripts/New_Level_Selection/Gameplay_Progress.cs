﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay_Progress", menuName = "Data/Gameplay_Progress")]
public class Gameplay_Progress : ScriptableObject {

    public bool SaveProgress = false;

    [SerializeField]
    private int _worldsUnlocked;
    public int WorldsUnlocked => _worldsUnlocked;

    [SerializeField]
    private int _worldIndex;

    [SerializeField]
    private int _totalLevels;
    public int TotalLevels => _totalLevels;

    [SerializeField]
    private int _levelsUnlocked;
    public int LevelsUnlocked => _levelsUnlocked;

    [SerializeField]
    private int[] _specificLevelsUnlocked;

    public  int LevelAvailable => _levelsUnlocked + 1;

    [SerializeField]
    private GameObject _levelSelectionPrefab;

    #region Not Saving Progress

    public void Reset()
    {
        _worldsUnlocked = 1;
        _levelsUnlocked = 1;
    }

    #endregion

    public void OnEnable()
    {
        if (!SaveProgress)
        {
            Reset();
        }
    }

    public void CompleteLevel(int levelNum)
    {
        _levelsUnlocked = Mathf.Max(_levelsUnlocked, levelNum);
        DisplayLevelSelection();
    }

    public void UnlockAllLevels()
    {
        _levelsUnlocked = _totalLevels;
        DisplayLevelSelection();
    }

    public void DisplayLevelSelection()
    {
        GameObject levelSelection = Instantiate(_levelSelectionPrefab);
        levelSelection.GetComponent<Level_Selection>().InstantiateLevelSelection();
    }
}
