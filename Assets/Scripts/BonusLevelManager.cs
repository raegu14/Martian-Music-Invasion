using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BonusLevelManager : MonoBehaviour {

    #region Constants
    private const float TextureOffset = 1f;
    private const float GlowOffset = 0.5f;
    private const float FrontOffset = 2f;

    #endregion

    #region Public Member Variables 

    public BonusLevel[] BonusLevels;
    public LevelSelection LevelSelectionManager;

    public Vector3 ShowingTransform;

    public GameObject TutorialOverlay;
    public GameObject SeeBubbles;
    public GameObject HearBubbles;

    public GameObject HearBox1;
    public GameObject HearBox2;
    public GameObject SeeBox1;
    public GameObject SeeBox2;

    public GameObject SelectionPane;
    public GameObject SpaceshipPane;
    public GameObject MusicBox;
    public GameObject Captions;

    public GameObject GlowCloud;
    public AudioSource MusicSource;

    #endregion /* Public Variables */

    #region Private Types (MusicSelection, StorySelecion, Selection)

    private struct MusicSelectionType
    {
        public Texture2D[] Textures;
        public AudioClip[] Clips;
    }

    private struct StorySelectionType
    {
        public Texture2D[] Textures;
    }

    private struct SelectionType
    {
        public bool MusicSelectionMade;
        public MusicSelectionType MusicSelection;

        public bool StorySelectionMade;
        public StorySelectionType StorySelection;
    }

    #endregion /* Private Types */

    #region Private Member Variables

    private GameObject HearBoxFront;
    private GameObject SeeBoxFront;

    private GameObject SelectedSpaceship;
    private List<GameObject> SpaceshipsBox1;
    private List<GameObject> SpaceshipsBox2;
    private List<GameObject> SpaceshipsSelected;

    private GameObject SelectedMeasure;
    private List<GameObject> MeasuresBox1;
    private List<GameObject> MeasuresBox2;
    private List<GameObject> MeasuresSelected;

    private SelectionType Selection;
    private MusicSelectionType Hear1, Hear2;
    private StorySelectionType See1, See2;

    #endregion /* Private Member Variables */

    /** @brief Called when Tutorial overlay "OK" button is pressed */
    public void DismissTutorialOverlay()
    {
        HearBox1.SetActive(true);
        HearBox2.SetActive(true);
        SeeBox1.SetActive(true);
        SeeBox2.SetActive(true);
        Captions.SetActive(true);
        Destroy(TutorialOverlay);
        ShowObject(SeeBubbles);
        ShowObject(HearBubbles);
    }

    public void ShowObject(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        foreach (SpriteRenderer sr in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = true;
        }
        foreach (Canvas c in obj.GetComponentsInChildren<Canvas>())
        {
            c.enabled = true;
        }
        foreach (CanvasRenderer cr in obj.GetComponentsInChildren<CanvasRenderer>())
        {
            cr.SetAlpha(1f);
        }
    }

    public void HideObject(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        foreach (SpriteRenderer sr in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        foreach (Canvas c in obj.GetComponentsInChildren<Canvas>())
        {
            c.enabled = false;
        }
        foreach (CanvasRenderer cr in obj.GetComponentsInChildren<CanvasRenderer>())
        {
        }
    }

    public void DismissSeeBubbles()
    {
        if (SeeBubbles != null)
        {
            Destroy(SeeBubbles);
            SeeBubbles = null;
        }
    }

    public void DismissHearBubbles()
    {
        if (HearBubbles != null)
        {
            Destroy(HearBubbles);
            HearBubbles = null;
        }
    }

    public Vector3 HidingTransform;
    public float ShowTime = 2f;
    public float MergeDuration = 0.5f;
    public float PushBoxesDuration = 0.4f;
    public float SelectionShowMargin = 0.15f;
    public float BrightnessTransitionTime = 0.7f;

    private Vector3 HearBox1Initial;
    private Vector3 HearBox2Initial;
    private Vector3 SeeBox1Initial;
    private Vector3 SeeBox2Initial;

    private float ScreenWidth;
    private float ScreenHeight;

    protected void Awake()
    {
        ScreenHeight = Camera.allCameras[0].orthographicSize * 2;
        ScreenWidth = ScreenHeight * Camera.allCameras[0].aspect;

        ShowingTransform = Vector3.back * 4.5f;
        HidingTransform = ShowingTransform + Vector3.up * ScreenHeight;
        HideObject(this.gameObject);
    }

    private List<GameObject> FillWithTextures(GameObject obj, Texture2D[] textures)
    {
        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        List<GameObject> ret = new List<GameObject>();

        float xmin = box.bounds.center.x - box.bounds.extents.x;
        float xmax = box.bounds.center.x + box.bounds.extents.x;
        float ymin = box.bounds.center.y - box.bounds.extents.y;
        float ymax = box.bounds.center.y + box.bounds.extents.y;

        float xInc = (xmax - xmin) / 2;
        float xLeft = xmin;
        float margin = 0.1f;

        float yInc = (ymax - ymin) / 2;
        float yBottom = ymin;

        if (textures.Length < 4)
        {
            // "Centering"
            ymin += 0.5f * yInc;
        }

        bool movingRight = true;

        foreach (Texture2D t in textures)
        {
            float xRight = xLeft + xInc;
            float yTop = yBottom + yInc;
            float midX = (xRight + xLeft) / 2;
            float midY = (yTop + yBottom) / 2;
            Vector2 size = new Vector2((1f - margin) * (xRight - xLeft), (1f - margin) * (yTop - yBottom));
            Vector3 pos = new Vector3(midX, midY, this.transform.position.z - TextureOffset);
            ret.Add(SpriteUtil.AddSprite(t, size, pos, t.name, obj, true));  
            
            if (movingRight)
            {
                // On iterations 0 and 2, move to the right
                xLeft += xInc;
                movingRight = false;
            } else
            {
                // On iteration 1, move left and down
                xLeft -= xInc;
                yBottom += yInc;
                movingRight = true;
            }
        }

        return ret;
    }

    private void PopulateBoxes()
    {
        SpaceshipsBox1 = FillWithTextures(SeeBox1, See1.Textures);
        SpaceshipsBox2 = FillWithTextures(SeeBox2, See2.Textures);
        MeasuresBox1 = FillWithTextures(HearBox1, Hear1.Textures);
        MeasuresBox2 = FillWithTextures(HearBox2, Hear2.Textures);
    }

    private void LoadOptions(BonusLevel item)
    {

        if (Random.value > 0.5f)
        {
            Hear1.Textures = item.MusicHighUncertaintyImages;
            Hear1.Clips = item.MusicHighUncertaintyAudio;
            Hear2.Textures = item.MusicLowUncertaintyImages;
            Hear2.Clips = item.MusicLowUncertaintyAudio;
            Logger.Instance.LogAction("BonusLevel", "Music High Uncertainty Box", "1");
        } else
        {
            Hear2.Textures = item.MusicHighUncertaintyImages;
            Hear2.Clips = item.MusicHighUncertaintyAudio;
            Hear1.Textures = item.MusicLowUncertaintyImages;
            Hear1.Clips = item.MusicLowUncertaintyAudio;
            Logger.Instance.LogAction("BonusLevel", "Music High Uncertainty Box", "2");
        }

        if (Random.value > 0.5f)
        {
            See1.Textures = item.StoryHighUncertaintyImages;
            See2.Textures = item.StoryLowUncertaintyImages;
            Logger.Instance.LogAction("BonusLevel", "Spaceship High Uncertainty Box", "1");
        }
        else
        {
            See2.Textures = item.StoryHighUncertaintyImages;
            See1.Textures = item.StoryLowUncertaintyImages;
            Logger.Instance.LogAction("BonusLevel", "Spaceship High Uncertainty Box", "2");
        }

        PopulateBoxes();
    }

    public IEnumerator BonusLevel(uint bonusIndex)
    {
        Logger.Instance.LogAction("BonusLevel", "Start", bonusIndex.ToString());
        LoadOptions(BonusLevels[bonusIndex]);

        ShowObject(this.gameObject);

        HearBox1Initial = HearBox1.transform.localPosition;
        HearBox2Initial = HearBox2.transform.localPosition;
        SeeBox1Initial = SeeBox1.transform.localPosition;
        SeeBox2Initial = SeeBox2.transform.localPosition;

        bool showLowerUI = (TutorialOverlay == null);
        HearBox1.SetActive(showLowerUI);
        HearBox2.SetActive(showLowerUI);
        SeeBox1.SetActive(showLowerUI);
        SeeBox2.SetActive(showLowerUI);
        Captions.SetActive(showLowerUI);

        HideObject(HearBubbles);
        HideObject(SeeBubbles);
        HideObject(SelectionPane);
        HideObject(GlowCloud);

        SeeBox1.GetComponentInChildren<SpriteRenderer>().enabled = false;
        SeeBox2.GetComponentInChildren<SpriteRenderer>().enabled = false;
        HearBox1.GetComponentInChildren<SpriteRenderer>().enabled = false;
        HearBox2.GetComponentInChildren<SpriteRenderer>().enabled = false;

        Selection.MusicSelectionMade = false;
        Selection.StorySelectionMade = false;

        yield return Show();

        if (LevelSelection.IsAutoplaying())
        {
            yield return new WaitForSeconds(0.4f);
            DismissTutorialOverlay();
            yield return new WaitForSeconds(0.4f);
            SelectMusic(1 + (int)(2 * Random.value));
            yield return new WaitForSeconds(0.4f);
            SelectStory(1 + (int)(2 * Random.value));
        }

        yield return new WaitUntil(SelectionMade);
        Captions.SetActive(false);
        yield return ShowSelection();
        yield return Hide();
    }

    private bool SelectionMade()
    {
        return Selection.MusicSelectionMade && Selection.StorySelectionMade;
    }

    private IEnumerator Show()
    {
        Camera.allCameras[0].backgroundColor = Color.black;
        yield return Transition.TransitionBrightness(LevelSelectionManager.gameObject, this.gameObject,
            BrightnessTransitionTime, Bright, Dark * Dark);
        yield return Transition.Translate(this.transform, this.ShowingTransform, this.ShowTime);
    }

    private IEnumerator Hide()
    {
        Camera.allCameras[0].backgroundColor = Color.black;
        yield return Transition.Translate(this.transform, this.HidingTransform * 3, this.ShowTime);
        Destroy(SelectedSpaceship);
        Destroy(SelectedMeasure);
        RestoreBoxes();
        yield return Transition.TransitionBrightness(LevelSelectionManager.gameObject, gameObject,
            BrightnessTransitionTime, Dark * Dark, Bright);
    }

    public void SelectMusic(int which)
    {
        DismissHearBubbles();
        //ButtonUtil.Hide(HearButton1);
        //ButtonUtil.Hide(HearButton2);
        StartCoroutine(MergeHearBoxes(which));
    }

    public void SelectStory(int which)
    {
        DismissSeeBubbles();
        //ButtonUtil.Hide(SeeButton1);
       // ButtonUtil.Hide(SeeButton2);
        StartCoroutine(MergeSeeBoxes(which));
    }

    private IEnumerator MergeTwoBoxes(GameObject front, GameObject back)
    {
        front.GetComponent<Button>().interactable = false;
        back.GetComponent<Button>().interactable = false;

        GameObject bgSprite = null;
        GameObject referenceSprite = null;

        foreach (SpriteRenderer sr in front.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.gameObject.name != "Sprite")
            {
                // really good coding style right here
                referenceSprite = sr.gameObject;
                continue;
            }
            bgSprite = sr.gameObject;
            SpriteUtil.CenterSpriteInImage(sr.gameObject, front);
            sr.enabled = true;
        }
        bgSprite.transform.position = new Vector3(
            bgSprite.transform.position.x, bgSprite.transform.position.y,
            referenceSprite.transform.position.z + 0.1f);

        Vector3 center = 0.5f * (front.transform.position + back.transform.position);

        Vector3 frontOffset = FrontOffset * Vector3.back;
        front.transform.position += frontOffset;
        StartCoroutine(Transition.Translate(front.transform, center + frontOffset, MergeDuration));
        yield return Transition.Translate(back.transform, center, MergeDuration);
        back.SetActive(false);

        foreach (SpriteRenderer sr in back.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.gameObject.name != "Sprite")
            {
                Destroy(sr.gameObject);
            }
        }

        foreach (SpriteRenderer sr in front.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.gameObject.name != "Sprite")
            {
                // really good coding style right here
                // I even copied and pasted this if statement
                continue;
            }
            sr.enabled = false;
        }
    }

    private IEnumerator MergeHearBoxes(int which)
    {
        GameObject back;
        switch (which)
        {
            case 1:
                MeasuresSelected = MeasuresBox1;
                HearBoxFront = HearBox1;
                back = HearBox2;
                Logger.Instance.LogAction("BonusLevel", "Music Box Choice", "1");
                break;
            case 2:
                MeasuresSelected = MeasuresBox2;
                HearBoxFront = HearBox2;
                back = HearBox1;
                Logger.Instance.LogAction("BonusLevel", "Music Box Choice", "2");
                break;
            default:
                Debug.Log("MergeHearBoxes(" + which + ") is invalid");
                HearBoxFront = HearBox2;
                back = HearBox1;
                break;
        }

        yield return MergeTwoBoxes(HearBoxFront, back);

        switch (which)
        {
            case 1:
                Selection.MusicSelection = Hear1;
                Logger.Instance.LogAction("BonusLevel", "Spaceship Box Choice", "1");
                break;
            case 2:
                Selection.MusicSelection = Hear2;
                Logger.Instance.LogAction("BonusLevel", "Spaceship Box Choice", "2");
                break;
            default:
                Debug.Log("SelectMusic(" + which + ") is invalid");
                Selection.MusicSelection = Hear2;
                break;
        }
        Selection.MusicSelectionMade = true;
    }

    private IEnumerator MergeSeeBoxes(int which)
    {
        GameObject back;
        switch (which)
        {
            case 1:
                SeeBoxFront = SeeBox1;
                back = SeeBox2;
                SpaceshipsSelected = SpaceshipsBox1;
                break;
            case 2:
                SeeBoxFront = SeeBox2;
                back = SeeBox1;
                SpaceshipsSelected = SpaceshipsBox2;
                break;
            default:
                Debug.Log("MergeSeeBoxes(" + which + ") is invalid");
                SeeBoxFront = SeeBox2;
                back = SeeBox1;
                break;
        }

        yield return MergeTwoBoxes(SeeBoxFront, back);

        switch (which)
        {
            case 1:
                Selection.StorySelection = See1;
                break;
            case 2:
                Selection.StorySelection = See2;
                break;
            default:
                Debug.Log("SelectStory(" + which + ") is invalid");
                Selection.StorySelection = See2;
                break;
        }
        Selection.StorySelectionMade = true;
    }

    private void ChooseSpaceship()
    {
        int len = Selection.StorySelection.Textures.Length;
        int i = (int)(Random.value * len) % len;
        name = Selection.StorySelection.Textures[i].name;
        Logger.Instance.LogAction("BonusLevel", "Spaceship To Show", name);
        SelectedSpaceship = SeeBoxFront.transform.Find(name).gameObject;
    }

    private string ChooseMusic(out AudioClip clip)
    {
        int len = Selection.MusicSelection.Textures.Length;
        int i = (int)(Random.value * len) % len;
        string name = Selection.MusicSelection.Textures[i].name;
        Logger.Instance.LogAction("BonusLevel", "Music Clip To Play", name);
        clip = Selection.MusicSelection.Clips[i];
        SelectedMeasure = HearBoxFront.transform.Find(name).gameObject;
        return name;
    }

    private void DestroyUnless(GameObject obj, string name, Transform newParent, GameObject ignore)
    {
        // obj: destroy this object unless it matches name or ignore
        // name: If the object's name is equal to this string, attach it to newParent instead of destroying it
        // ignore: If the object is this object, don't do anything to it
        if (obj == ignore)
        {
            return;
        }
        else if (obj.name == name)
        {
            obj.transform.parent = newParent;
        } else
        {
            Destroy(obj);
        }
    }

    private IEnumerator CycleOptions(List<GameObject> options, GameObject choice)
    {
        int nchoices = options.Count * 3;
        float startInterval = 0.1f;
        float lengtheningFactor = 1.1f;
        Vector3 glowOffset = GlowOffset * Vector3.forward;
        int blinkCount = 3;

        for (int i = 0; i < nchoices || (options[i % options.Count] != choice); i++)
        {
            GameObject selection = options[i % options.Count];
            GlowCloud.transform.position = selection.transform.position + glowOffset;
            ShowObject(GlowCloud);
            yield return new WaitForSeconds(startInterval);
            startInterval *= lengtheningFactor;
        }

        GlowCloud.transform.position = choice.transform.position + glowOffset;

        float fadeOutTime = blinkCount * 2 * startInterval;
        foreach (GameObject other in options)
        {
            if (other != choice)
            {
                StartCoroutine(Transition.FadeOut(other, fadeOutTime));
            }
        }

        for (int i = 0; i < blinkCount; i++)
        {
            yield return new WaitForSeconds(startInterval);
            HideObject(GlowCloud);
            yield return new WaitForSeconds(startInterval);
            ShowObject(GlowCloud);
        }
        yield return new WaitForSeconds(startInterval);
        HideObject(GlowCloud);

        GlowCloud.transform.parent = this.transform;

        foreach (GameObject other in options)
        {
            if (other != choice)
            {
                Destroy(other);
            }
        }
    }
    private void RestoreBoxes()
    {
        SeeBox1.SetActive(true);
        SeeBox2.SetActive(true);
        HearBox1.SetActive(true);
        HearBox2.SetActive(true);

        SeeBox1.GetComponent<Button>().interactable = true;
        SeeBox2.GetComponent<Button>().interactable = true;
        HearBox1.GetComponent<Button>().interactable = true;
        HearBox2.GetComponent<Button>().interactable = true;

        SeeBox1.transform.localPosition = SeeBox1Initial;
        SeeBox2.transform.localPosition = SeeBox2Initial;
        HearBox1.transform.localPosition = HearBox1Initial;
        HearBox2.transform.localPosition = HearBox2Initial;
    }

    private IEnumerator PushBoxesOut()
    {
        Bounds bgBounds = this.GetComponent<SpriteRenderer>().bounds;

        float seeWidth = SeeBoxFront.GetComponent<RectTransform>().rect.width;
        float seeScale = SeeBoxFront.GetComponent<RectTransform>().lossyScale.x;

        float hearWidth = HearBoxFront.GetComponent<RectTransform>().rect.width;
        float hearScale = HearBoxFront.GetComponent<RectTransform>().lossyScale.x;

        //float bgRight = bgBounds.center.x + bgBounds.extents.x;
        //float bgLeft = bgBounds.center.x - bgBounds.extents.x;

        Vector3 seeBoxDest = new Vector3((ScreenWidth / 2) - (seeWidth * seeScale / 2), 
            SeeBoxFront.transform.position.y, SeeBoxFront.transform.position.z);
        Vector3 hearBoxDest = new Vector3(-(ScreenWidth / 2) + (hearWidth * hearScale / 2),
            HearBoxFront.transform.position.y, HearBoxFront.transform.position.z);
        

        StartCoroutine(Transition.Translate(SeeBoxFront.transform, seeBoxDest, PushBoxesDuration));
        yield return Transition.Translate(HearBoxFront.transform, hearBoxDest, PushBoxesDuration);
    }

    private IEnumerator MoveSelectionObject(GameObject obj, GameObject dest, GameObject oldContainer)
    {
        Bounds destBounds = dest.GetComponent<SpriteRenderer>().bounds;

        // Size the object will end up with
        Vector2 objSize = destBounds.size * (1 - SelectionShowMargin);

        // Location the object will end up
        Vector3 objPos = destBounds.center + Vector3.back * TextureOffset;

        // Move the object over
        obj.transform.parent = dest.transform;

        // Get rid of the thing the object was on
        StartCoroutine(Transition.FadeOut(oldContainer, 0.8f));

        // Move the object over
        yield return Transition.TranslateResize(obj, objPos, objSize, 0.8f);

        // Hide the old container but restore its alpha value
        oldContainer.SetActive(false);
        yield return Transition.FadeIn(oldContainer, 0f, false);
    }

    private IEnumerator Glow(GameObject obj, float length)
    {
        Color start = new Color(1f, 1f, 1f, 1f);
        Color mid = new Color(0.4f, 0.9f, 0.7f, 1f);

        yield return Transition.TransitionColor(obj, length / 2, start, mid, false);
        yield return Transition.TransitionColor(obj, length / 2, mid, start, false);
    }

    private IEnumerator PlayClip(GameObject music, AudioPlayer player, float length)
    {
        StartCoroutine(Glow(music, length));
        yield return player.PlayBlocking();
    }

    public float Dark = 0.5f;
    public float Bright = 1f;

    private IEnumerator Brighten(GameObject obj)
    {
        yield return Transition.TransitionBrightness(obj, null, BrightnessTransitionTime, Dark, Bright);
    }

    private IEnumerator Darken(GameObject obj)
    {
        yield return Transition.TransitionBrightness(obj, null, BrightnessTransitionTime, Bright, Dark);
    }

    private IEnumerator ShowSelection()
    {
        AudioClip musicChoiceClip;

        HideObject(Captions);

        ChooseSpaceship();
        ChooseMusic(out musicChoiceClip);

        AudioPlayer player = new AudioPlayer(musicChoiceClip, MusicSource);

        musicChoiceClip.LoadAudioData();

        // Push the "front" selection boxes off to the side
        yield return PushBoxesOut();

        // Show the "SelectionPane" where the selections will be shown
        yield return Transition.FadeIn(SelectionPane, 1f, enableSr: true);

        // Bring attention to the seebox
        GlowCloud.transform.parent = SeeBoxFront.transform;
        yield return Transition.TransitionBrightness(gameObject, SeeBoxFront, 
            BrightnessTransitionTime, Bright, Dark);

        // Animate the process of choosing a spaceship
        yield return CycleOptions(SpaceshipsSelected, SelectedSpaceship);
        SelectedSpaceship.transform.parent = SpaceshipPane.transform;

        // Bring attention to the spaceship pane
        StartCoroutine(Darken(SeeBoxFront));
        yield return Brighten(SpaceshipPane);

        // Put the spaceship on its frame
        yield return MoveSelectionObject(SelectedSpaceship, SpaceshipPane, SeeBoxFront);
        yield return new WaitForSeconds(musicChoiceClip.length);

        // Bring attention to the hearbox
        StartCoroutine(Darken(SpaceshipPane));
        yield return Brighten(HearBoxFront);

        // Animate the process of choosing a measure
        GlowCloud.transform.parent = HearBoxFront.transform;
        yield return CycleOptions(MeasuresSelected, SelectedMeasure);
        SelectedMeasure.transform.parent = MusicBox.transform;

        // Bring attention to the musicbox
        StartCoroutine(Darken(HearBoxFront));
        yield return Brighten(MusicBox);

        // Put the measure on its frame
        yield return MoveSelectionObject(SelectedMeasure, MusicBox, HearBoxFront);

        yield return PlayClip(SelectedMeasure, player, musicChoiceClip.length);

        yield return Transition.TransitionBrightness(gameObject, MusicBox, 
            BrightnessTransitionTime, Dark, Bright);

        yield return null;
    }

}
