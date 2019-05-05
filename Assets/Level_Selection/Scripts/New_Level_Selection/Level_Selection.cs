using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level_Selection : MonoBehaviour {

    [Header("World Selection")]
    [SerializeField]
    private GameObject _worldSelectionParent;

    [SerializeField]
    private Transform _worldButtonParent = null;

    [SerializeField]
    private GameObject _worldButtonPrefab;

    [SerializeField]
    private World_Info[] _worldInfoList = null;

    [Header("Level Selection")]
    [SerializeField]
    private GameObject _levelSelectionParent = null;

    [SerializeField]
    private Image _levelSelectionBackground = null;

    [SerializeField]
    private Transform _levelButtonParent = null;

    [SerializeField]
    private GameObject _levelButtonPrefab = null;

    [SerializeField]
    private Image _selectionMarker = null;

    [SerializeField]
    private Image _completeLevel = null;

    [SerializeField]
    private float _completeLevelAnimationTime = 1f;

    [SerializeField]
    private Gameplay_Progress _progress = null;

    [SerializeField]
    private AudioSource _audioSource = null;

    [SerializeField]
    private AudioClip _lockedAudio = null;

    private World_Info _activeWorld = null;
    private string _selectedLevelName;

    #region World Selection 

    public void DisplayWorldSelection()
    {
        SetMode(LevelSelectionMode.World);

        UpdateLevels();
        ResetWorld();

        for (int i = 0; i < _worldInfoList.Length; i++)
        {
            int index = i;
            GameObject obj = Instantiate(_worldButtonPrefab, _worldButtonParent);
            obj.GetComponent<World_Button>().Display(_worldInfoList[index]);
            obj.GetComponent<World_Button>().Button.onClick.AddListener(() => DisplayLevelSelectionByWorld(index));
        }
    }

    private void ResetWorld()
    {
        for (int i = 0; i < _worldButtonParent.childCount; i++)
        {
            Destroy(_worldButtonParent.GetChild(i).gameObject);
        }
    }

    #endregion

    #region Level Selection

    public void DisplayLevelSelectionByWorld(int worldNum)
    {
        SetMode(LevelSelectionMode.Level);
        _activeWorld = _worldInfoList[worldNum];

        _levelSelectionBackground.sprite = _activeWorld.Background;

        UpdateLevels();
        ResetLevelSelectionLevels();
        ResetLevelSelectionSelection();

        Level_Selection_Util.SelectLevel.RemoveListener(SelectLevel);
        Level_Selection_Util.SelectLevel.AddListener(SelectLevel);

        for (int i = 0; i < _activeWorld.Levels.Length; i++)
        {
            GameObject obj = Instantiate(_levelButtonPrefab, _levelButtonParent);
            obj.GetComponent<Level_Button>().Display(_activeWorld.Levels[i]);
        }

        Level_Selection_Util.SetButtonInteractivity.Invoke(false);

        PlayLevelAudio();
    }

    //instantiate level prefabs
    //Complete Level
    public void DisplayLevelSelectionByLevel(int levelNum)
    {
        SetMode(LevelSelectionMode.Level);
        SetActiveWorld(levelNum);

        _levelSelectionBackground.sprite = _activeWorld.Background;

        UpdateLevels();
        ResetLevelSelectionLevels();
        ResetLevelSelectionSelection();

        Level_Selection_Util.SelectLevel.RemoveListener(SelectLevel);
        Level_Selection_Util.SelectLevel.AddListener(SelectLevel);

        for (int i = 0; i < _activeWorld.Levels.Length; i++)
        {
            if (_activeWorld.Levels[i].LevelNumber == levelNum)
            {
                GameObject obj = Instantiate(_levelButtonPrefab, _levelButtonParent);
                obj.GetComponent<Level_Button>().Display(_activeWorld.Levels[i], true);
            }
            else
            {
                GameObject obj = Instantiate(_levelButtonPrefab, _levelButtonParent);
                obj.GetComponent<Level_Button>().Display(_activeWorld.Levels[i]);
            }
        }

        CompleteLevel(GetLevelIndex(levelNum));
    }

    private void ResetLevelSelectionLevels()
    {
        for (int i = 0; i < _levelButtonParent.childCount; i++)
        {
            Destroy(_levelButtonParent.GetChild(i).gameObject);
        }
        _selectedLevelName = "";

        _completeLevel.enabled = false;
    }

    private void CompleteLevel(int index)
    {
        StartCoroutine(CompleteLevelAsync(index));
    }

    private IEnumerator CompleteLevelAsync(int index)
    {
        yield return CompleteLevelAnimation(index);
        yield return PlayLevelAudioAsync(0);
    }

    private IEnumerator CompleteLevelAnimation(int index)
    {
        yield return new WaitForEndOfFrame();

        _completeLevel.enabled = true;
        _completeLevel.sprite = _activeWorld.Levels[index].MusicSprite;

        Vector3 initialPosition = _completeLevel.transform.position;
        Vector2 initialScale = _completeLevel.GetComponent<RectTransform>().sizeDelta;
        Level_Button targetButton = _levelButtonParent.GetChild(index).GetComponent<Level_Button>();
        RectTransform target = targetButton.MusicTile.GetComponent<RectTransform>();
        Vector3 targetPosition = target.position;
        Vector2 targetScale = target.sizeDelta;
        float t = 0;

        //lerp completeLevel sprite back to position
        while(t < 1)
        {
            t = Mathf.Min(1, t + (Time.deltaTime / _completeLevelAnimationTime));
            _completeLevel.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            _completeLevel.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(initialScale, targetScale, t);
            yield return new WaitForEndOfFrame();
        }

        _completeLevel.transform.position = initialPosition;
        _completeLevel.GetComponent<RectTransform>().sizeDelta = initialScale;
        _completeLevel.enabled = false;

        targetButton.GetComponent<Level_Button>().Display(_activeWorld.Levels[index]);
    }

    private void PlayLevelAudio()
    {
        StartCoroutine(PlayLevelAudioAsync(0));
    }

    private IEnumerator PlayLevelAudioAsync(int clipNum)
    {
        if (clipNum < _activeWorld.Levels.Length)
        {
            GameObject levelButton = _levelButtonParent.GetChild(clipNum).gameObject;
            levelButton.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            levelButton.GetComponent<Canvas>().overrideSorting = true;
            levelButton.GetComponent<Canvas>().sortingOrder = SortingLayer.GetLayerValueFromName("LevelSelectionAnimation");

            if (_activeWorld.Levels[clipNum].Unlocked)
            {
                _audioSource.clip = _activeWorld.Levels[clipNum].Audio;
            }
            else
            {
                _audioSource.clip = _lockedAudio;
            }
            _audioSource.Play();
            yield return new WaitWhile(() => _audioSource.isPlaying);

            levelButton = _levelButtonParent.GetChild(clipNum).gameObject;
            levelButton.transform.localScale = new Vector3(1f, 1f, 1f);
            levelButton.GetComponent<Canvas>().sortingOrder = SortingLayer.GetLayerValueFromName("LevelSelection");
            levelButton.GetComponent<Canvas>().overrideSorting = false;

            StartCoroutine(PlayLevelAudioAsync(clipNum + 1));
        }
    }

    public void SelectLevel(int selectedLevel)
    {
        ResetLevelSelectionSelection();

        _selectionMarker.enabled = true;
        _selectionMarker.color = Color.green;
        for(int i = 0; i < _activeWorld.Levels.Length; i++)
        {
            if(_activeWorld.Levels[i].LevelNumber == selectedLevel)
            {
                _selectionMarker.transform.position = _levelButtonParent.GetChild(i).position;
                _levelButtonParent.GetChild(i).GetComponent<Level_Button>().Select();
            }
        }
        _selectedLevelName = "Level_Play" + selectedLevel;
    }

    private void ResetLevelSelectionSelection()
    {
        for (int i = 0; i < _levelButtonParent.childCount; i++)
        {
            Level_Button level = _levelButtonParent.GetChild(i).GetComponent<Level_Button>();
            level.UnSelect();
        }
        _selectionMarker.enabled = false;
    }

    public void LoadLevel()
    {
        if (_selectedLevelName != "")
        {
            SceneManager.LoadScene(_selectedLevelName);
        }
    }

    #endregion

    private void UpdateLevels()
    {
        foreach(World_Info world in _worldInfoList)
        {
            foreach(Level_Info level in world.Levels)
            {
                level.SetUnlocked(_progress.LevelsUnlocked);
                level.SetAvailable(_progress.LevelAvailable);
            }
            world.SetUnlocked();
        }
    }

    private void SetActiveWorld(int levelNum)
    {
        _activeWorld = null;
        for(int i = 0; i < _worldInfoList.Length; i++)
        {
            if (_worldInfoList[i].ContainsLevel(levelNum))
            {
                _activeWorld = _worldInfoList[i];
                break;
            }
        }
    }

    private int GetLevelIndex(int levelNum)
    {
        for(int i = 0; i < _activeWorld.Levels.Length; i++)
        {
            if(_activeWorld.Levels[i].LevelNumber == levelNum)
            {
                return i;
            }
        }
        return 0;
    }

    private void SetMode(LevelSelectionMode mode)
    {
        if(mode == LevelSelectionMode.World)
        {
            _levelSelectionParent.SetActive(false);
            _worldSelectionParent.SetActive(true);
        }

        else
        {
            _levelSelectionParent.SetActive(true);
            _worldSelectionParent.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (_progress.LevelAvailable == 0)
        {
            SceneManager.LoadScene("IntroCutscene1");
        }
        else
        {
            StartCoroutine(StartGameAsync());
        }
    }

    private IEnumerator StartGameAsync()
    {
        bool loaded = false;
        yield return new WaitForEndOfFrame();
        int level = Mathf.Min(_progress.LevelAvailable, _progress.TotalLevels);
        Debug.Log(level);
        for (int i = 0; i < _worldInfoList.Length; i++)
        {
            if (_worldInfoList[i].ContainsLevel(level))
            {
                DisplayLevelSelectionByWorld(i);
                loaded = true;
                break;
            }
        }

        if (!loaded)
        {
            DisplayLevelSelectionByWorld(0);
        }
    }
}

public enum LevelSelectionMode
{
    World = 0,
    Level = 1
}

