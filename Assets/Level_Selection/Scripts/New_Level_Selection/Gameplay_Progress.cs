using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Gameplay_Progress", menuName = "Data/Gameplay_Progress")]
public class Gameplay_Progress : ScriptableObject {

    public bool SaveProgress = false;

    [SerializeField]
    private int _totalLevels;
    public int TotalLevels => _totalLevels;

    [SerializeField]
    private int _levelsUnlocked;
    public int LevelsUnlocked => _levelsUnlocked;

    [SerializeField]
    private int[] _specificLevelsUnlocked;

    public int LevelAvailable => _levelsUnlocked + 1;

    [SerializeField]
    private GameObject _levelSelectionPrefab;

    [SerializeField]
    private string[] _worldUnlockPasswords;
    private string _inputPassword;

    [SerializeField]
    private string[] _worldCompletePasswords;

    #region Not Saving Progress

    public void Reset()
    {
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

    public void SetPassword(string str)
    {
        _inputPassword = str;
    }

    public void UnlockWorld()
    {
        bool set = false;
        for(int i = 0; i < _worldUnlockPasswords.Length; i++)
        {
            if(_inputPassword.Equals(_worldUnlockPasswords[i], System.StringComparison.OrdinalIgnoreCase))
            {
                _levelsUnlocked = (i + 1) * 6;
                set = true;
            }
        }
        if (!set)
        {
            _levelsUnlocked = 1;
        }
    }

    public void CompleteLevel(int levelNum)
    {
        _levelsUnlocked = Mathf.Max(_levelsUnlocked, levelNum);
        DisplayLevelSelection(levelNum);
    }

    public void UnlockAllLevels()
    {
        _levelsUnlocked = _totalLevels;
        DisplayWorldSelection();
    }

    public void DisplayWorldSelection()
    {
        /*
        UnityAction<Scene, LoadSceneMode> loadLevelSelection = new UnityAction<Scene, LoadSceneMode>((Scene scene, LoadSceneMode mode) => {
            Debug.Log("loaded");
            GameObject levelSelection = Instantiate(_levelSelectionPrefab);
            levelSelection.GetComponent<Level_Selection>().DisplayWorldSelection();
        });
        SceneManager.sceneLoaded += loadLevelSelection;
        SceneManager.LoadScene("Level_Selection", LoadSceneMode.Single);
        SceneManager.sceneLoaded -= loadLevelSelection;
        */

        GameObject levelSelection = Instantiate(_levelSelectionPrefab);
        levelSelection.GetComponent<Level_Selection>().DisplayWorldSelection();
    }

    public void DisplayLevelSelection(int levelNum)
    {
        /*
        UnityAction<Scene, LoadSceneMode> loadLevelSelection = new UnityAction<Scene, LoadSceneMode>((Scene scene, LoadSceneMode mode) => {
            Debug.Log("loaded");
            GameObject levelSelection = Instantiate(_levelSelectionPrefab);
            Debug.Log("loaded");

            levelSelection.GetComponent<Level_Selection>().DisplayLevelSelectionByLevel(levelNum);
        });
        SceneManager.sceneLoaded += loadLevelSelection;
        SceneManager.LoadScene("Level_Selection", LoadSceneMode.Single);
        SceneManager.sceneLoaded -= loadLevelSelection;
        */

        GameObject levelSelection = Instantiate(_levelSelectionPrefab);
        levelSelection.GetComponent<Level_Selection>().DisplayLevelSelectionByLevel(levelNum);
    }

    public void StartGame()
    {
        /*
        UnityAction<Scene, LoadSceneMode> loadLevelSelection = new UnityAction<Scene, LoadSceneMode>((Scene scene, LoadSceneMode mode) => {
            Debug.Log("loaded");
            GameObject levelSelection = Instantiate(_levelSelectionPrefab);
            Debug.Log("loaded");
            levelSelection.GetComponent<Level_Selection>().StartGame();
        });
        SceneManager.sceneLoaded += loadLevelSelection;
        SceneManager.LoadScene("Level_Selection", LoadSceneMode.Single);
        SceneManager.sceneLoaded -= loadLevelSelection;
        */

        GameObject levelSelection = Instantiate(_levelSelectionPrefab);
        levelSelection.GetComponent<Level_Selection>().StartGame();
    }
}
