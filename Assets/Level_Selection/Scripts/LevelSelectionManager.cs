using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MartianMusicInvasion.FreeExploration;
using MartianMusicInvasion.LevelSelection;

public class LevelSelectionManager : MonoBehaviour {

    public bool Autoplay = false;

    public void EndAutoplay()
    {
        Autoplay = false;
    }

    public void BeginAutoplay()
    {
        Autoplay = true;
    }

    public bool IsAutoplaying()
    {
        return Autoplay;
    }

    public bool DebugBonus;

    public AudioClip LockedClip;
    public float LevelSelectionGridDropTime = 2.5f;
    public float TextureSwapTime = 2.5f;
    public float ObservationTime = 0.8f;
    public float MeasureMoveTime = 0.8f;
    public float MeasureShrinkTime = 1.6f;
    
    public float TilePaddingY = 0.0f;
    public float TileMarginY = 0.1f;
    public float ComicPaddingX = 0.0f;
    public float ComicPaddingY = 0.0f;
    public float MeasurePaddingX = 0.0f;
    public float MeasurePaddingY = 0.0f;
    public float OverallMarginX = 0.2f;

    public uint LevelsPerLine = 6;
    public float LevelListLoadDelay = 0.03f;

    public LevelListItem[] LevelList;
    public BonusLevelManager BonusManager;

    public Free_Exploration_Level_Manager FreeExplorationManager;
    public List<int> FreeExplorationLevels;
    private List<int> _freeExplorationLevels;

    public Texture2D ComicBackground;
    public Texture2D MeasureBackground;
    public Texture2D ComicLock;
    public Texture2D MeasureLock;

    public GameObject PlayButtonCanvas;
    public GameObject PlayButton;

    public EventSystem SavedEventSystem;

    public GameObject HeaderPrefab;

    public Color HeaderLockedColor;
    public Color HeaderCompleteColor;
    public Color HeaderTutorialColor;
    public Color HeaderNextColor;

    public float TileZ = -1f;

    public LevelSelectionAnimator levelSelectionAnimator;

    private GameVersion.T Version;

    public void SetVersion(GameVersion.T v)
    {
        Version = v;
    }

    private bool LevelHasStarted = false;

    private uint LevelsCompleted;

    private float ScreenWidth;
    private float ScreenHeight;
    private float LevelWidth;

    public struct LLIObject {
        public GameObject parent;
        public GameObject comicBg;
        public GameObject comicTile;
        public GameObject measureBg;
        public GameObject measureTile;
        public GameObject header;
        public AudioSource audioSource;
        public AudioPlayer audioPlayer;
    };

    private LLIObject[] Objects;

    void Start()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(this.gameObject);

        _freeExplorationLevels = new List<int>();
        foreach (int i in FreeExplorationLevels)
        {
            _freeExplorationLevels.Add(i);
        }

        LevelsCompleted = 0;
        //Version = GameVersion.T.NotIntegrated;

        levelSelectionAnimator.SetDerivedParameters();
        Objects = new LLIObject[LevelList.Length];
        StartCoroutine(LoadLevelList());
    }

    private bool IsTutorialLevel(uint i)
    {
        return (i % 3) == 0;
    }

    private bool LevelIsLocked(uint i)
    {
        return !IsTutorialLevel(i) && (i >= LevelsCompleted);
    }

    private bool ComicIsLocked(uint i)
    {
        switch (Version)
        {
            case GameVersion.T.Integrated:
                return false;
            case GameVersion.T.NotIntegrated:
                return LevelIsLocked(i);
            default:
                Debug.Log(string.Format("Invalid GameVersion: {0}", Version));
                return false;
        }
    }

    private bool MeasureIsLocked(uint i)
    {
        switch (Version)
        {
            case GameVersion.T.Integrated:
                return LevelIsLocked(i);
            case GameVersion.T.NotIntegrated:
                return false;
            default:
                Debug.Log(string.Format("Invalid GameVersion: {0}", Version));
                return false;
        }
    }

    private Texture2D ComicTexture(LevelListItem item, uint i)
    {
        if (ComicIsLocked(i))
        {
            return ComicLock;
        } else
        {
            return item.comic;
        }
    }

    private Texture2D MeasureTexture(LevelListItem item, uint i)
    {
        if (MeasureIsLocked(i))
        {
            return MeasureLock;
        } else
        {
            return item.measure;
        }
    }
   
    private GameObject AddHeader(GameObject parent, uint level, Vector3 pos)
    {
        GameObject obj = Instantiate<GameObject>(HeaderPrefab);

        obj.transform.position = pos;
        obj.transform.parent = parent.transform;

        levelSelectionAnimator.UpdateHeader(level);

        return obj;
    }

    private IEnumerator LoadNewAudio(uint i)
    {
        AudioPlayer player = Objects[i].audioPlayer;
        LevelListItem item =LevelList[i];
        player.LoadNewClip(item.clip);
        yield return null;
    }

    #region Button Utilities

    private void ShowPlayButton()
    {
        Vector3 buttonPos;

        switch (Version)
        {
            case GameVersion.T.Integrated:
                buttonPos = Objects[LevelsCompleted].measureTile.transform.position;
                break;
            case GameVersion.T.NotIntegrated:
            default:
                buttonPos = Objects[LevelsCompleted].comicTile.transform.position;
                break;
        }

        buttonPos = Objects[LevelsCompleted].header.transform.position;

       PlayButton.transform.position = buttonPos + 4f * Vector3.back;

        ButtonUtil.Show(PlayButton);
    }

    private void HidePlayButton()
    {
        ButtonUtil.Hide(PlayButton);
    }

    #endregion

    private IEnumerator LoadLevelList()
    {
        uint i;

        HidePlayButton();

        for (i = 0; i <LevelList.Length; i++)
        {
            levelSelectionAnimator.AddLevelListItem(i, LevelList[i]);
        }

        yield return new WaitForSeconds(LevelListLoadDelay);

        if (DebugBonus)
        {
            yield return StartCoroutine(BonusManager.BonusLevel(0));
        }
       StartCoroutine(PlayMusic());

        yield return new WaitForSeconds(LevelListLoadDelay);
    }

    public void PlayNextLevel()
    {
        Logger.Instance.LogAction("LevelSelection", "Play Level Button Pressed", (LevelsCompleted + 1).ToString());
        LevelHasStarted = true;
        HidePlayButton();
        GameObject.Find("WorldCanvas").GetComponent<ForwardWorldSelection>().levelBackground.SetActive(false);
        StartCoroutine(OpenNextLevel());
    }
    
    private IEnumerator OpenNextLevel()
    {
        string scenename;
        if (LevelsCompleted == 6)
        {
            scenename = "Interlude";
        } else
        {
            scenename =LevelList[LevelsCompleted].LevelSceneName;
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync(scenename);

        yield return levelSelectionAnimator.TransitionNextLevel(ao);

        Logger.Instance.LogAction("LevelSelection", "Level Selection Screen Hidden", (LevelsCompleted + 1).ToString());
    }

    public void DownOneLevel()
    {
        if (LevelsCompleted != 0)
        {
            LevelsCompleted--;
        }
        levelSelectionAnimator.UpdateHeader(LevelsCompleted + 1);
        levelSelectionAnimator.UpdateHeader(LevelsCompleted + 2);
        HidePlayButton();
        ShowPlayButton();
    }

    public void UpOneLevel()
    {
        if (LevelsCompleted !=LevelList.Length - 1)
        {
            LevelsCompleted++;
            levelSelectionAnimator.UpdateHeader(LevelsCompleted + 1);
            levelSelectionAnimator.UpdateHeader(LevelsCompleted);
            HidePlayButton();
            ShowPlayButton();
        }
    }

    public void LevelCompleted(uint levelNum, Transform measure)
    {
        levelNum = (levelNum - 1) % (uint)LevelList.Length + 1;

        Logger.Instance.LogAction("LevelSelection", "Level Completed", (LevelsCompleted + 1).ToString());

        LevelHasStarted = false;
        bool needsNewAudio = MeasureIsLocked(levelNum - 1);
        bool isBonusLevel = (levelNum % 3) == 0 ||DebugBonus;

        LevelsCompleted = levelNum;

        levelSelectionAnimator.UpdateHeader(levelNum);

        if (LevelsCompleted !=LevelList.Length)
        {
            levelSelectionAnimator.UpdateHeader(levelNum + 1);
        }
        else
        {
            //GameObject.Find("World Selection Manager").GetComponent<WorldSelection>().UnlockWorld();
        }

        if (needsNewAudio)
        {
           StartCoroutine(LoadNewAudio(levelNum - 1));
        }
        
       StartCoroutine(DropLevelSelectionGrid(measure));
       StartCoroutine(ReplaceMeasure(measure, isBonusLevel));
    }

    /** Drop the level selection grid into place above a level scene has been completed. 
     **/
    private IEnumerator DropLevelSelectionGrid(Transform measure)
    {
        yield return levelSelectionAnimator.CenterTile(LevelsCompleted,LevelSelectionGridDropTime);

        GameObject.Find("WorldCanvas").GetComponent<ForwardWorldSelection>().levelBackground.SetActive(true);

        Logger.Instance.LogAction("LevelSelection", "Level Selection Screen Showing", (LevelsCompleted + 1).ToString());
    }

    private IEnumerator ReplaceMeasure(Transform measure, bool isBonusLevel)
    {
        uint level = LevelsCompleted - 1;

        SpriteRenderer sr = measure.GetComponentInChildren<SpriteRenderer>();

        float startWidth = (sr.sprite.texture.width / (100f)) * (measure.localScale.x * sr.transform.localScale.x);
        float startHeight = (sr.sprite.texture.height / (100f)) * (measure.localScale.y * sr.transform.localScale.y);

        Vector2 startSize = new Vector2(startWidth, startHeight);
        Vector3 startPosition = sr.gameObject.transform.position;

        // Swap out old sprite renderer for new one
        Texture2D newText;
        switch (Version)
        {
            case GameVersion.T.Integrated:
                newText =LevelList[level].measure;
                break;
            case GameVersion.T.NotIntegrated:
                float meanSize = (startSize.x + startSize.y) / 2;
                startSize = new Vector2(meanSize, meanSize); // Comic tiles are square
                newText =LevelList[level].comic;
                break;
            default:
                Debug.Log("Invalid Version");
                newText =LevelList[level].comic;
                break;
        }

        GameObject newGo = SpriteUtil.AddSprite(newText, startSize, startPosition, "Temp", transform.parent);
        SpriteRenderer newSr = newGo.GetComponent<SpriteRenderer>();
        newSr.sortingOrder = 36;

        float t;
        float currentTime = 0f;
        float moveTime =TextureSwapTime;

        while (currentTime <= moveTime)
        {
            t = currentTime / moveTime;
            sr.color = new Color(1f, 1f, 1f, 1f - t);
            newSr.color = new Color(1f, 1f, 1f, t);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }

        Destroy(measure.gameObject);
        newSr.color = new Color(1f, 1f, 1f, 1f);

        StartCoroutine(ShrinkIntoPlace(level, newSr, newGo, startSize, isBonusLevel));
    }

    private IEnumerator ShrinkIntoPlace(uint level, SpriteRenderer newSr, GameObject newGo, Vector2 startSize, bool isBonusLevel)
    {
        yield return levelSelectionAnimator.ShrinkIntoPlace(level, newSr, newGo, startSize, isBonusLevel);

        Logger.Instance.LogAction("LevelSelection", "Replaced Lock with Measure or Comic", (LevelsCompleted + 1).ToString());

        Debug.Log("Free EXploration " +FreeExplorationLevels[0]);
        //check if is free exploration prototype level and then add it
        if(FreeExplorationLevels.Contains((int)level))
        {
            yield return FreeExplorationManager.PlayFreeExploration(FreeExplorationLevels[0] == (int)level);
        }

        if (isBonusLevel)
        {
            uint bonusStageIndex;
            if (DebugBonus)
            {
                bonusStageIndex = LevelsCompleted;
            }
            else {
                bonusStageIndex = ((LevelsCompleted / 3) - 1);
            }
            yield return BonusManager.BonusLevel(bonusStageIndex);
            yield return PlayMusic();
        }
        else
        {
            yield return PlayMusic();
        }

        newSr.sortingOrder = 35;
    }

    /** @brief Play music on the level selection screen */
    private IEnumerator PlayMusic()
    {
        uint startLevel, endLevel, level;
        float waitTime;

        if (LevelsCompleted %LevelsPerLine == 0)
        {
            startLevel = 0;
            endLevel = (uint)LevelList.Length - 1;
        } else
        {
            startLevel = (LevelsCompleted /LevelsPerLine) *LevelsPerLine;
            endLevel = startLevel +LevelsPerLine - 1;
        }

        Logger.Instance.LogAction("LevelSelection",
            string.Format("Playing music from {0} to {1}", startLevel, endLevel), (LevelsCompleted + 1).ToString());

        /*
        if (LevelsCompleted !=LevelList.Length)
        {
            ShowPlayButton();
        }
        */

        for (level = startLevel; level <= endLevel && !LevelHasStarted; level++)
        {
            if (!LevelHasStarted)
            {
                StartCoroutine(levelSelectionAnimator.CenterTile(level, 0.5f));
            }

            AudioPlayer player = Objects[level].audioPlayer;
            if (MeasureIsLocked(level))
            {
                waitTime = 1.1f;
            } else
            {
                waitTime = 2.0f;
            }

            if (level == LevelsCompleted)
            {
                HidePlayButton();
            }

            StartCoroutine(levelSelectionAnimator.PopoutTile(level, waitTime));
            StartCoroutine(player.PlayBlocking());
            yield return new WaitForSeconds(waitTime);
            if (level == LevelsCompleted)
            {
                ShowPlayButton();
                if (IsAutoplaying())
                {
                    PlayNextLevel();
                }
            }
        }

        Logger.Instance.LogAction("LevelSelection", "Done Playing Music", (LevelsCompleted + 1).ToString());

        if (!LevelHasStarted)
        {
            StartCoroutine(levelSelectionAnimator.CenterTile(LevelsCompleted, 0.5f));
        }


        if (LevelsCompleted ==LevelList.Length)
        {
            // Application.LoadLevel("OutroCutscene1");
            HttpWriter.Flush();
            Logger.Instance.LogAction("Transitioning to next scene", "", "");
            SceneManager.LoadScene(GameObject.Find("WorldCanvas").GetComponent<ForwardWorldSelection>().nextWorld);
            Destroy(gameObject);
            Destroy(GameObject.Find("WorldCanvas"));
        }

    }
}
