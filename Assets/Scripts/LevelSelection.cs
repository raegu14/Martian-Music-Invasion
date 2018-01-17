using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelection : MonoBehaviour {

    public static bool Autoplay = false;

    public static void EndAutoplay()
    {
        Autoplay = false;
    }

    public static void BeginAutoplay()
    {
        Autoplay = true;
    }

    public static bool IsAutoplaying()
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
    public float ComicMarginX = 0.0f;
    public float ComicPaddingY = 0.0f;
    public float MeasurePaddingX = 0.0f;
    public float MeasureMarginX = 0.0f;
    public float MeasurePaddingY = 0.0f;
    public float OverallMarginX = 0.2f;

    public uint TutorialLevelPeriod = 3;
    public uint BonusLevelPeriod = 3;
    public uint LevelsPerLine = 6;
    public float LevelListLoadDelay = 0.03f;

    public LevelListItem[] LevelList;
    public BonusLevelManager BonusManager;

    public Texture2D ComicBackground;
    public Texture2D MeasureBackground;
    public Texture2D ComicLock;
    public Texture2D MeasureLock;

    public GameObject PlayButtonCanvas;
    public GameObject PlayButton;
    public GameObject LoadingButton;
    public Scrollbar ScrollBar;

    public EventSystem SavedEventSystem;

    Vector3 ScrollBarInitial;

    public void ScrollBarValueUpdated(int value)
    {
        this.transform.position = ScrollBar.value * Vector3.up * (TotalHeight() - ScreenHeight);
        ScrollBar.transform.position = ScrollBarInitial;
    }

    public void UpdateScrollbarValue()
    {
        ScrollBar.value = (this.transform.position.y) / (TotalHeight() - ScreenHeight);
    }

    public GameObject HeaderPrefab;

    public Color HeaderLockedColor;
    public Color HeaderCompleteColor;
    public Color HeaderTutorialColor;
    public Color HeaderNextColor;

    public float TileZ = -1f;

    private static GameVersion.T Version;

    public static void SetVersion(GameVersion.T v)
    {
        Version = v;
    }

    public static LevelSelection Instance = null;

    private static bool LevelHasStarted = false;

    private static uint LevelsCompleted;

    private static float ScreenWidth;
    private static float ScreenHeight;
    private static float LevelWidth;

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

    private static LLIObject[] Objects;

    private static void SetDerivedParameters()
    {
        ScreenHeight = Camera.allCameras[0].orthographicSize * 2;
        ScreenWidth = ScreenHeight * Camera.allCameras[0].aspect;
        LevelWidth = ScreenWidth / ((float)Instance.LevelsPerLine + Instance.OverallMarginX * 2f);
        MeasureHeightMemoized = new Dictionary<uint, float>();
        LLIScaleMemoized = new Dictionary<uint, Vector3>();
        LevelHeightsMemoized = new Dictionary<uint, float>();
        LevelCenterYMemoized = new Dictionary<uint, float>();
    }

	void Start () {
        if (Instance == null)
        {
            ScrollBarInitial = ScrollBar.transform.position;
            Instance = this;

            DontDestroyOnLoad(this);
            DontDestroyOnLoad(this.gameObject);

            LevelsCompleted = 0;
            //Version = GameVersion.T.NotIntegrated;

            SetDerivedParameters();
            Objects = new LLIObject[Instance.LevelList.Length];
            StartCoroutine(LoadLevelList());
        }
        else
        {
            Debug.LogError("Duplicate LevelSelection created");
        }
	}

    private static bool IsTutorialLevel(uint i)
    {
        return (i % 3) == 0;
    }

    private static bool LevelIsLocked(uint i)
    {
        return !IsTutorialLevel(i) && (i >= LevelsCompleted);
    }

    private static bool ComicIsLocked(uint i)
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

    private static bool MeasureIsLocked(uint i)
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

    private static Texture2D ComicTexture(LevelListItem item, uint i)
    {
        if (ComicIsLocked(i))
        {
            return Instance.ComicLock;
        } else
        {
            return item.comic;
        }
    }

    private static Texture2D MeasureTexture(LevelListItem item, uint i)
    {
        if (MeasureIsLocked(i))
        {
            return Instance.MeasureLock;
        } else
        {
            return item.measure;
        }
    }


    #region LLI Sizing and Positioning

    private static Dictionary<uint, float> MeasureHeightMemoized;
    private static float MeasureHeight(uint row)
    {
        float height, width, H2W, maxHeight, maxWidth, maxH2W;
        uint i;
        Texture2D measureTexture;

        if (!MeasureHeightMemoized.TryGetValue(row, out height))
        {
            maxHeight = maxWidth = maxH2W = -1f;
            for (i = row * Instance.LevelsPerLine; i < (row + 1) * Instance.LevelsPerLine; i++)
            {
                if (i >= Instance.LevelList.Length) break;
                measureTexture = Instance.LevelList[i].measure;
                height = (float)measureTexture.height;
                width = (float)measureTexture.width;
                H2W = height / width;
                if (H2W > maxH2W)
                {
                    maxHeight = height;
                    maxWidth = width;
                    maxH2W = H2W;
                }
            }
            height = LevelWidth * maxH2W;
            MeasureHeightMemoized.Add(row, height);
        }
        return height;
    }
    
    private static Dictionary<uint, float> LevelHeightsMemoized;
    private static float LevelHeight(uint row)
    {
        float height;

        if (!LevelHeightsMemoized.TryGetValue(row, out height))
        {
            height = MeasureHeight(row);                                // Measure height
            height += LevelWidth * (1.0f - 2 * Instance.ComicPaddingX); // Comic height
            height += LevelWidth * 2 * (Instance.ComicPaddingY);        // Space above and below comic
            height += LevelWidth * 2 * (Instance.MeasurePaddingY);      // Space above and below measure
            height += LevelWidth * Instance.TilePaddingY;                // Space between comic and measure
            height += LevelWidth * 2 * (Instance.TileMarginY);          // Space above measure and below comic 
            LevelHeightsMemoized.Add(row, height);
        }
        return height;
    }

    private static float TotalHeight()
    {
        float height;
        uint row;
        uint rowCount = ((uint)Instance.LevelList.Length - 1) / (Instance.LevelsPerLine) + 1;
        for (row = 0, height = 0f; row < rowCount; row++)
        {
            height += LevelHeight(row);
        }
        return height + (rowCount - 1) * (Instance.TileMarginY);
    }

    private static Dictionary<uint, float> LevelCenterYMemoized;
    private static float LevelCenterY(uint row)
    {
        uint i;
        float y, centerY;
        if (!LevelCenterYMemoized.TryGetValue(row, out centerY))
        {
            for (y = 0, i = 0; i < row; i++)
            {
                y += LevelHeight(i);
            }
            y += 0.5f * LevelHeight(row);
            centerY = 0.5f * ScreenHeight - y;
            LevelCenterYMemoized.Add(row, centerY);
        }
        return centerY;
    }

    private static Dictionary<uint, Vector3> LLIScaleMemoized;
    private static Vector3 LLIScale(uint row)
    {
        Vector3 scale;
        if (!LLIScaleMemoized.TryGetValue(row, out scale))
        {
            float xScale = LevelWidth / ScreenHeight;
            float yScale = LevelHeight(row) / ScreenHeight;
            scale = new Vector3(xScale, yScale, 1.0f);
            LLIScaleMemoized.Add(row, scale);
        }
        return scale;
    }

    private static float LevelCenterX(uint col)
    {
        return (-ScreenWidth / 2) + ((float)col + 0.5f + Instance.OverallMarginX) * LevelWidth;
    }

    private static Vector3 LLICenterPosition(uint row, uint col)
    {
        return new Vector3(LevelCenterX(col), LevelCenterY(row), Instance.TileZ);
    }

    private static Vector3 LLICenterPosition(uint level)
    {
        return LLICenterPosition(level / Instance.LevelsPerLine, level % Instance.LevelsPerLine);
    }

    /** @brief Offset from LLI Center to Comic Background and Comic Tile centers 
    */
    private static Vector3 ComicOffset(Vector2 totalSize, Vector2 comicBgSize)
    {
        float marginOffset = LevelWidth * Instance.TileMarginY;
        float totalHeight = totalSize.y;
        float comicBgHeight = comicBgSize.y;

        return Vector3.down * (0.5f * (totalHeight - comicBgHeight) - marginOffset);
    }

    /** @brief Offset from LLI Center to Measure Background and Measure Tile centers 
     */
    private static Vector3 MeasureOffset(Vector2 totalSize, Vector2 measureBgSize)
    {
        float marginOffset = LevelWidth * Instance.TileMarginY;
        float totalHeight = totalSize.y;
        float measureBgHeight = measureBgSize.y;

        return Vector3.up * (0.5f * (totalHeight - measureBgHeight) - marginOffset);
    }

    private static Vector3 HeaderOffset(Vector2 totalSize, Vector2 comicBgSize)
    {
        float marginOffset = LevelWidth * Instance.TileMarginY;
        float totalHeight = totalSize.y;
        return ComicOffset(totalSize, comicBgSize) + Vector3.up * (Instance.ComicPaddingY * -0.5f + LevelWidth / 2);
        //return Vector3.up * (0.5f * totalHeight - marginOffset);
    }

    /** @brief Offset from LLI Center to Comic Background and Measure Background centers
     */
    private static Vector3 BgOffset = Vector3.forward * 0.1f;

    /** @brief Size of the Comic Background for a LLI
     */
    private static Vector2 ComicBgSize()
    {
        float width = LevelWidth;
        float height = LevelWidth + 2 * (Instance.ComicPaddingY - Instance.ComicPaddingX);
        return new Vector2(width, height);
    }

    /** @brief Size of the Comic Tile for a LLI
     */
    private static Vector2 ComicTileSize(Vector2 comicBgSize)
    {
        float bgWidth = comicBgSize.x;
        float bgHeight = comicBgSize.y;
        float width = bgWidth - 2 * (LevelWidth * Instance.ComicPaddingX);
        float height = bgHeight - 2 * (LevelWidth * Instance.ComicPaddingY);
        return new Vector2(width, height);
    }

    private static Vector2 ComicTileSize()
    {
        return ComicTileSize(ComicBgSize());
    }

    private static Vector3 ComicTilePos(uint level)
    {
        uint row = level / Instance.LevelsPerLine;
        uint col = level % Instance.LevelsPerLine;
        return LLICenterPosition(row, col) + ComicOffset(TotalSize(row), ComicBgSize());
    }

    /** @brief Size of the Measure Background for a LLI
     */
    private static Vector2 MeasureBgSize(uint row)
    {
        float width = LevelWidth;
        float height = MeasureHeight(row) + 2 * (LevelWidth * Instance.MeasurePaddingY);
        return new Vector2(width, height);
    }

    /** @brief Size of the Measure Tile for a LLI
     */
    private static Vector2 MeasureTileSize(Vector2 measureBgSize)
    {
        float bgWidth = measureBgSize.x;
        float bgHeight = measureBgSize.y;
        float width = bgWidth - 2 * (LevelWidth * Instance.MeasurePaddingX);
        float height = bgHeight - 2 * (LevelWidth * Instance.MeasurePaddingY);
        return new Vector2(width, height);
    }

    private static Vector3 MeasureTilePos(uint level)
    {
        uint row = level / Instance.LevelsPerLine;
        uint col = level % Instance.LevelsPerLine;
        return LLICenterPosition(row, col) + MeasureOffset(TotalSize(row), MeasureBgSize(row));
    }

    private static Vector2 MeasureTileSize(uint level)
    {
        uint row = level / Instance.LevelsPerLine;
        return MeasureTileSize(MeasureBgSize(row));
    }

    /** @brief Size of an entire LLI
     */
    private static Vector2 TotalSize(uint row)
    {
        return new Vector2(LevelWidth, LevelHeight(row));
    }

    #endregion

    private static void UpdateHeader(uint level, GameObject obj=null)
    {
        if (obj == null)
        {
            obj = Objects[level - 1].header;
        }

        SpriteRenderer ball = obj.transform.Find("Ball").gameObject.GetComponent<SpriteRenderer>();
        //SpriteRenderer message = obj.transform.FindChild("Message").gameObject.GetComponent<SpriteRenderer>();
        Transform canvas = obj.transform.Find("Canvas");
        Text statusText = canvas.Find("Status Text").gameObject.GetComponent<Text>();
        Text numberText = canvas.Find("Number Text").gameObject.GetComponent<Text>();

        numberText.text = level.ToString();
        
        if (LevelsCompleted >= level)
        {
            statusText.text = "Complete";
            ball.color = Instance.HeaderCompleteColor;
        }
        else if (LevelsCompleted + 1 == level)
        {
            statusText.text = "";
            ball.color = Instance.HeaderCompleteColor;
            statusText.color = Instance.HeaderCompleteColor;
        }
        else if (IsTutorialLevel(level - 1))
        {
            statusText.text = "Tutorial";
            ball.color = Instance.HeaderLockedColor;
            statusText.color = Instance.HeaderTutorialColor;
        } else
        {
            statusText.text = "";
            ball.color = Instance.HeaderLockedColor;
        }
    }

    private static GameObject AddHeader(GameObject parent, uint level, Vector3 pos)
    {
        GameObject obj = Instantiate<GameObject>(Instance.HeaderPrefab);

        obj.transform.position = pos;
        obj.transform.parent = parent.transform;

        UpdateHeader(level, obj);

        return obj;
    }

    private static void AddLevelListItem(uint i, LevelListItem item)
    {
        uint row, col;
        Vector3 center, comicOffset, measureOffset, headerOffset;
        Vector3 comicBgPos, measureBgPos, comicTilePos, measureTilePos, headerPos;
        Vector2 comicBgSize, measureBgSize, comicTileSize, measureTileSize, totalSize;

        row = i / Instance.LevelsPerLine;
        col = i % Instance.LevelsPerLine;

        comicBgSize = ComicBgSize();
        comicTileSize = ComicTileSize(comicBgSize);
        measureBgSize = MeasureBgSize(row);
        measureTileSize = MeasureTileSize(measureBgSize);
        totalSize = TotalSize(row);

        center = LLICenterPosition(row, col);
        comicOffset = ComicOffset(totalSize, comicBgSize);
        measureOffset = MeasureOffset(totalSize, measureBgSize);
        headerOffset = HeaderOffset(totalSize, comicBgSize);

        comicBgPos = center + comicOffset + BgOffset;
        comicTilePos = center + comicOffset;
        measureBgPos = center + measureOffset + BgOffset;
        measureTilePos = center + measureOffset;
        headerPos = center + headerOffset;

        GameObject parent = new GameObject(string.Format("Level {0}", i + 1));
        parent.transform.parent = Instance.transform;
        parent.transform.position = center;
        Objects[i].parent = parent;

        Objects[i].comicBg = SpriteUtil.AddSprite(Instance.ComicBackground, comicBgSize, comicBgPos,
            string.Format("Comic Background {0}", i + 1), parent);

        Objects[i].comicTile = SpriteUtil.AddSprite(ComicTexture(item, i), comicTileSize, comicTilePos,
            string.Format("Comic Tile {0}", i + 1), parent);

        Objects[i].measureBg = SpriteUtil.AddSprite(Instance.MeasureBackground, measureBgSize, measureBgPos,
            string.Format("Measure Background {0}", i + 1), parent);

        Objects[i].measureTile = SpriteUtil.AddSprite(MeasureTexture(item, i), measureTileSize, measureTilePos,
            string.Format("Measure Tile {0}", i + 1), parent);

        Objects[i].header = AddHeader(parent, i + 1, headerPos);

        GameObject audioObject = new GameObject(string.Format("Audio Source {0}", i + 1));
        audioObject.transform.parent = parent.transform;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        Objects[i].audioSource = audioSource;
        AudioClip clip;

        if (MeasureIsLocked(i))
        {
            clip = Instance.LockedClip;
        } else
        {
            clip = item.clip;
        }
        Objects[i].audioPlayer = new AudioPlayer(clip, audioSource);
    }

    private static IEnumerator LoadNewAudio(uint i)
    {
        AudioPlayer player = Objects[i].audioPlayer;
        LevelListItem item = Instance.LevelList[i];
        player.LoadNewClip(item.clip);
        yield return null;
    }

    #region Button Utilities

    private static void ShowPlayButton()
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

        Instance.PlayButton.transform.position = buttonPos + 4f * Vector3.back;

        ButtonUtil.Show(Instance.PlayButton);
    }

    private static void HidePlayButton()
    {
        ButtonUtil.Hide(Instance.PlayButton);
    }

    #endregion

    private static IEnumerator LoadLevelList()
    {
        uint i;

        HidePlayButton();
        Instance.LoadingButton.SetActive(true);

        for (i = 0; i < Instance.LevelList.Length; i++)
        {
            AddLevelListItem(i, Instance.LevelList[i]);
            //yield return new WaitForSeconds(Instance.LevelListLoadDelay);
        }

        Instance.LoadingButton.SetActive(false);

        yield return new WaitForSeconds(Instance.LevelListLoadDelay);

        if (Instance.DebugBonus)
        {
            yield return Instance.StartCoroutine(Instance.BonusManager.BonusLevel(0));
        }
        Instance.StartCoroutine(PlayMusic());

        yield return new WaitForSeconds(Instance.LevelListLoadDelay);
    }

    public void PlayNextLevel()
    {
        Logger.Instance.LogAction("LevelSelection", "Play Level Button Pressed", (LevelsCompleted + 1).ToString());
        LevelHasStarted = true;
        HidePlayButton();
        StartCoroutine(OpenNextLevel());
    }
    
    private static IEnumerator OpenNextLevel()
    {
        float t;

        Transform transform = Instance.transform;
        Vector3 startPosition = transform.position;
        Vector3 destPosition = Vector3.up * TotalHeight();

        string scenename;
        if (LevelsCompleted == 6)
        {
            scenename = "Interlude";
        } else
        {
            scenename = Instance.LevelList[LevelsCompleted].LevelSceneName;
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync(scenename);

        while (!ao.isDone)
        {
            t = ao.progress;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            transform.position = Vector3.Lerp(startPosition, destPosition, t);
            Instance.UpdateScrollbarValue();

            yield return new WaitForEndOfFrame();
        }
        transform.position = destPosition;
        Instance.UpdateScrollbarValue();

        Instance.ScrollBar.gameObject.SetActive(false);
        Instance.transform.position = destPosition;

        RemoveEventSystem();

        Logger.Instance.LogAction("LevelSelection", "Level Selection Screen Hidden", (LevelsCompleted + 1).ToString());
    }

    public static void RemoveEventSystem()
    {
        return;
        /* EventSystem es = Instance.transform.root.GetComponentInChildren<EventSystem>();
        es.gameObject.SetActive(false);
        Instance.SavedEventSystem = es;
        es.transform.parent = null; */
    }

    public static void RestoreEventSystem()
    {
        return;
        /* EventSystem es = Instance.SavedEventSystem;
        es.gameObject.SetActive(true);
        Instance.SavedEventSystem = null;
        es.transform.parent = Instance.transform; */
    }

    public static void DownOneLevel()
    {
        if (LevelsCompleted != 0)
        {
            LevelsCompleted--;
        }
        UpdateHeader(LevelsCompleted + 1);
        UpdateHeader(LevelsCompleted + 2);
        HidePlayButton();
        ShowPlayButton();
    }

    public static void UpOneLevel()
    {
        if (LevelsCompleted != Instance.LevelList.Length - 1)
        {
            LevelsCompleted++;
            UpdateHeader(LevelsCompleted + 1);
            UpdateHeader(LevelsCompleted);
            HidePlayButton();
            ShowPlayButton();
        }
    }

    public static void LevelCompleted(uint levelNum, Transform measure, LevelManager lm)
    {
        Logger.Instance.LogAction("LevelSelection", "Level Completed", (LevelsCompleted + 1).ToString());

        LevelHasStarted = false;
        bool needsNewAudio = MeasureIsLocked(levelNum - 1);
        bool isBonusLevel = (levelNum % 3) == 0 || Instance.DebugBonus;

        LevelsCompleted = levelNum;

        UpdateHeader(levelNum);

        if (LevelsCompleted != Instance.LevelList.Length)
        {
            UpdateHeader(levelNum + 1);
        }

        if (needsNewAudio)
        {
            Instance.StartCoroutine(LoadNewAudio(levelNum - 1));
        }
        
        Instance.StartCoroutine(DropLevelSelectionGrid(measure, null, lm));
        Instance.StartCoroutine(ReplaceMeasure(measure, isBonusLevel));
    }

    public static void AddressingLevelCompleted(uint levelNum, Transform measure, AddressingController lm)
    {
        //measure.parent = Instance.gameObject.transform;
        //measure.position = new Vector3(measure.position.x, measure.position.y, -6f);

        HttpWriter.Flush();
        Logger.Instance.LogAction("LevelSelection", "Level Completed", (LevelsCompleted + 1).ToString());

        LevelHasStarted = false;
        bool needsNewAudio = MeasureIsLocked(levelNum - 1);
        bool isBonusLevel = (levelNum % 3) == 0 || Instance.DebugBonus;

        LevelsCompleted = levelNum;

        UpdateHeader(levelNum);
        UpdateHeader(levelNum + 1);

        if (needsNewAudio)
        {
            Instance.StartCoroutine(LoadNewAudio(levelNum - 1));
        }

        Instance.StartCoroutine(DropLevelSelectionGrid(measure, lm, null));
        Instance.StartCoroutine(ReplaceMeasure(measure, isBonusLevel));
    }

    private static IEnumerator CenterTile(uint tileIndex, float duration)
    {
        uint row = tileIndex / Instance.LevelsPerLine;
        float desY = Mathf.Max(0f, Mathf.Min(TotalHeight() - ScreenHeight, -LevelCenterY(row)));
        Vector3 dest = Vector3.up * desY;

        float elapsed = 0;
        float t;
        Transform transform = Instance.transform;
        Vector3 startPosition = transform.position;

        while (elapsed <= duration)
        {
            t = elapsed / duration;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            if (LevelHasStarted)
            {
                yield break;
            }

            transform.position = Vector3.Lerp(startPosition, dest, t);
            Instance.UpdateScrollbarValue();

            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        transform.position = dest;
        Instance.UpdateScrollbarValue();
        yield return null;
    }

    /** Drop the level selection grid into place above a level scene has been completed. 
     **/
    private static IEnumerator DropLevelSelectionGrid(Transform measure, AddressingController lma, LevelManager lm)
    {
        Instance.ScrollBar.gameObject.SetActive(true);
        yield return CenterTile(LevelsCompleted, Instance.LevelSelectionGridDropTime);
        if (lm != null)
        {
            lm.ClearBackground();
        } else if (lma != null)
        {
            lma.ClearBackground();
        }

        RestoreEventSystem();

        Logger.Instance.LogAction("LevelSelection", "Level Selection Screen Showing", (LevelsCompleted + 1).ToString());
    }

    private static IEnumerator ReplaceMeasure(Transform measure, bool isBonusLevel)
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
                newText = Instance.LevelList[level].measure;
                break;
            case GameVersion.T.NotIntegrated:
                float meanSize = (startSize.x + startSize.y) / 2;
                startSize = new Vector2(meanSize, meanSize); // Comic tiles are square
                newText = Instance.LevelList[level].comic;
                break;
            default:
                Debug.Log("Invalid Version");
                newText = Instance.LevelList[level].comic;
                break;
        }

        GameObject newGo = SpriteUtil.AddSprite(newText, startSize, startPosition, "Temp", Instance.transform.parent);
        SpriteRenderer newSr = newGo.GetComponent<SpriteRenderer>();

        float t;
        float currentTime = 0f;
        float moveTime = Instance.TextureSwapTime;

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

        Instance.StartCoroutine(ShrinkIntoPlace(level, newSr, newGo, startSize, isBonusLevel));

        if (!isBonusLevel)
        {
            Instance.StartCoroutine(PlayMusic());
        }
    }

    private static IEnumerator ShrinkIntoPlace(uint level, SpriteRenderer newSr, GameObject newGo, Vector2 startSize, bool isBonusLevel)
    {
        Vector2 endSize;
        Vector3 startPos, endPos;

        startPos = newGo.transform.position;
        startPos = new Vector3(startPos.x, startPos.y, -6f);

        Transform destTransform;

        switch (Version)
        {
            case GameVersion.T.Integrated:
                endSize = MeasureTileSize(level);
                endPos = MeasureTilePos(level);
                destTransform = Objects[level].measureTile.transform;
                break;
            case GameVersion.T.NotIntegrated:
            default:
                endSize = ComicTileSize();
                endPos = ComicTilePos(level);
                destTransform = Objects[level].comicTile.transform;
                break;
        }

        Vector3 destScaleInitial = Vector3.Scale(destTransform.localScale, destTransform.parent.localScale);
        Vector3 destScaleCur;

        yield return new WaitForSeconds(Instance.ObservationTime);

        float t, s;
        float currentTime = 0f;
        float moveTime = Instance.MeasureMoveTime;
        float shrinkTime = Instance.MeasureShrinkTime;

        Transform transform = newGo.transform;

        endPos = new Vector3(endPos.x, endPos.y, startPos.z);

        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(startScale.x * (endSize.x / startSize.x),
            startScale.y * (endSize.y / startSize.y), 1f);

        Vector3 goalScale, goalPos;

        while (currentTime <= Mathf.Max(moveTime, shrinkTime))
        {
            t = Mathf.Min(currentTime / moveTime, 1f);
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            s = Mathf.Min(currentTime / shrinkTime, 1f);
            s = s * s * s * (s * (6f * s - 15f) + 10f);

            destScaleCur = Vector3.Scale(destTransform.localScale, destTransform.parent.localScale);

            goalScale = new Vector3(endScale.x * (destScaleCur.x / destScaleInitial.x),
                endScale.y * (destScaleCur.y / destScaleInitial.y),
                endScale.z);

            goalPos = destTransform.position;

            transform.position = Vector3.Lerp(startPos, goalPos, t);
            transform.localScale = Vector3.Lerp(startScale, goalScale, s);

            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }

        destScaleCur = Vector3.Scale(destTransform.localScale, destTransform.parent.localScale);

        goalScale = new Vector3(endScale.x * (destScaleCur.x / destScaleInitial.x),
            endScale.y * (destScaleCur.y / destScaleInitial.y),
            endScale.z);

        goalPos = destTransform.position;

        transform.position = goalPos;
        transform.localScale = goalScale;

        GameObject replacing;
        switch (Version)
        {
            case GameVersion.T.Integrated:
                replacing = Objects[level].measureTile;
                break;
            case GameVersion.T.NotIntegrated:
            default:
                replacing = Objects[level].comicTile;
                break;
        }

        transform.position = replacing.transform.position;
        transform.parent = Objects[level].parent.transform;
        Destroy(replacing);

        Logger.Instance.LogAction("LevelSelection", "Replaced Lock with Measure or Comic", (LevelsCompleted + 1).ToString());

        if (isBonusLevel)
        {
            uint bonusStageIndex;
            if (Instance.DebugBonus)
            {
                bonusStageIndex = LevelsCompleted;
            }
            else {  
                bonusStageIndex = ((LevelsCompleted / 3) - 1);
            }
            yield return Instance.BonusManager.BonusLevel(bonusStageIndex);
            yield return PlayMusic();
        }
    }

    private static IEnumerator PopoutTile(uint i, float waitTime)
    {
        Transform t = Objects[i].parent.transform;
        Vector3 initialScale = t.localScale;
        t.position += 2 * Vector3.back;
        yield return Transition.Resize(t, initialScale * 1.2f, waitTime * 0.1f);
        //t.localScale *= 1.2f;
        yield return new WaitForSeconds(waitTime * 0.8f);
        yield return Transition.Resize(t, initialScale, waitTime * 0.1f);
        t.position -= 2 * Vector3.back;
        //t.localScale /= 1.2f;
    }

    /** @brief Play music on the level selection screen */
    private static IEnumerator PlayMusic()
    {
        uint startLevel, endLevel, level;
        float waitTime;

        if (LevelsCompleted % Instance.LevelsPerLine == 0)
        {
            startLevel = 0;
            endLevel = (uint)Instance.LevelList.Length - 1;
        } else
        {
            startLevel = (LevelsCompleted / Instance.LevelsPerLine) * Instance.LevelsPerLine;
            endLevel = startLevel + Instance.LevelsPerLine - 1;
        }

        Logger.Instance.LogAction("LevelSelection",
            string.Format("Playing music from {0} to {1}", startLevel, endLevel), (LevelsCompleted + 1).ToString());

        if (LevelsCompleted != Instance.LevelList.Length)
        {
            ShowPlayButton();
        }

        for (level = startLevel; level <= endLevel && !LevelHasStarted; level++)
        {
            if (!LevelHasStarted)
            {
                Instance.StartCoroutine(CenterTile(level, 0.5f));
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

            Instance.StartCoroutine(PopoutTile(level, waitTime));
            Instance.StartCoroutine(player.PlayBlocking());
            yield return new WaitForSeconds(waitTime);
            if (level == LevelsCompleted)
            {
                ShowPlayButton();
                if (IsAutoplaying())
                {
                    Instance.PlayNextLevel();
                }
            }
        }

        Logger.Instance.LogAction("LevelSelection", "Done Playing Music", (LevelsCompleted + 1).ToString());

        if (!LevelHasStarted)
        {
            Instance.StartCoroutine(CenterTile(LevelsCompleted, 0.5f));
        }


        if (LevelsCompleted == Instance.LevelList.Length)
        {
            // Application.LoadLevel("OutroCutscene1");
            HttpWriter.Flush();
            Logger.Instance.LogAction("Transitioning to Outro", "", "");
            SceneManager.LoadScene("OutroCutscene1");
            Destroy(Instance.gameObject);
        }

    }
    /*
    public static string Url()
    {
        string result = Application.absoluteURL;

        if (result.Length < 4)
        {
            return "http://127.0.0.1:8000";
        }

        //removing "/MyGame.unity3D" 
        for (int i = result.Length - 1; i >= 0; --i)
        {
            if (result[i] == '/')
            {
                return result.Remove(i);
            }
        }

        return "";
    }
    */
    //private string LOGGING_SERVICE_URL = Url() + "/log";

    void Update () {
	    /*if (!stuff)
        {
            stuff = true;
            Debug.Log(Url() + "/log");
        }*/

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && ScrollBar.gameObject.activeInHierarchy)
        {
            ScrollBar.value = Mathf.Min(1f, Mathf.Max(0f, ScrollBar.value - scroll));
            ScrollBarValueUpdated(-1);
        }
    }
}
