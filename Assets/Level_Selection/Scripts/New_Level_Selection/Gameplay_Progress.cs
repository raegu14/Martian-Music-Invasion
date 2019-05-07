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

    public int LevelAvailable => Mathf.Min(_levelsUnlocked + 1, _levelCap);

    private int _levelCap;
    public int LevelCap => _levelCap;

    [SerializeField]
    private GameObject _levelSelectionPrefab;

    [SerializeField]
    private string[] _worldUnlockPasswords;
    private string _inputPassword;

    [SerializeField]
    private string[] _worldCompletePasswords;

    private string _completePassword;
    public string CompletePassword => _completePassword;

    private bool _enteredCorrectPassword = false;
    public bool EnteredCorrectPassword => _enteredCorrectPassword;

    #region Not Saving Progress

    public void Reset()
    {
        _levelsUnlocked = 0;
    }

    #endregion

    public void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
       if (!SaveProgress)
        {
            Reset();
        }

        _enteredCorrectPassword = false;
    }

    public void SetPassword(string str)
    {
        _inputPassword = str;
    }

    public void UnlockWorld()
    {
        for(int i = 0; i < _worldUnlockPasswords.Length; i++)
        {
            if(_inputPassword.Equals(_worldUnlockPasswords[i], System.StringComparison.OrdinalIgnoreCase))
            {
                _levelsUnlocked = i * 6;
                _levelCap = Mathf.Min((i + 1) * 6, _totalLevels);
                if (i < _worldCompletePasswords.Length)
                {
                    _completePassword = _worldCompletePasswords[i];
                }
                _enteredCorrectPassword = true;
                break;
            }
        }
    }

    public void CompleteLevel(int levelNum)
    {
        _levelsUnlocked = Mathf.Min(Mathf.Max(_levelsUnlocked, levelNum), _levelCap);
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
        if (_enteredCorrectPassword)
        {

            GameObject levelSelection = Instantiate(_levelSelectionPrefab);
            levelSelection.GetComponent<Level_Selection>().StartGame();
        }
    }
}
