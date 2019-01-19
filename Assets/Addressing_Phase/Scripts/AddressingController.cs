using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AddressingController : MonoBehaviour {

    public uint levelNumber;

    public float sgGrav = 0.1f;
    public float sgFriction = 0.85f;
    public float sgSpring = 0.2f;

    public float swaptime = 1.2f;
    public float flytime = 1.2f;

    public float tutorialFadeTime = 10f;

    public bool IsFirstLevel = true;

    public GameObject Superdog;
    public GameObject SuperdogDialogue;
    public Button SuperdogButton;

    private SuperdogController superdogController;
    private AudioSource superdogAudioSource;
    private DialogueController dialogueController;

    public GameObject LevelCompleteObject;
    public GameObject GameOver;
    public GameObject GrayCirclePrefab;
    public GameObject Supergirl;
    public GameObject SupergirlArm;
    public GameObject SupergirlVineCurled;
    public Animator SupergirlAnimator;
    public GameObject Vine;
    public GameObject ThrowingVine;
    public BoxCollider2D StaffCollider;

    private VineController vineController;

    public int LivesCount;
    public GameObject[] Lives;

    public AddressAudio Audio;

    [SerializeField]
    private AudioSource _backgroundAudio;

    public GameObject[] LevelSteps;

    public GameObject BackgroundParent;
    public GameObject[] Backgrounds;
    private int BackgroundLeft = 0;

    private Vector3 BackgroundDelta;

    private int BackgroundRight { get { return 1 - this.BackgroundLeft; } }
    public bool TransitioningBackgrounds = false;

    private GameObject CurrentStepObject;
    private AddressingStep CurrentStep;
    private GameObject[] IncorrectCircles;
    private GrayCircle CorrectCircle;

    private GameObject NextStepObject;
    private AddressingStep NextStep;

    private int stepsCompleted;

    private Vector3 sgVelocity;

    public GameObject hintObject;
    private HintController hintController;

    private bool HintDisplayed = false;

    private bool CompletingLevel;
    private int levelAttempts;

    public void Restart()
    {
        // Logger.Instance.LogAction("AddressingController", "Restart", "");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private static float GetYByAddress(string address)
    { switch (address) {
            case "1st Line": return 0f / 8f;
            case "1st Space": return 1f / 8f;
            case "2nd Line": return 2f / 8f;
            case "2nd Space": return 3f / 8f;
            case "3rd Line": return 4f / 8f;
            case "3rd Space": return 5f / 8f;
            case "4th Line": return 6f / 8f;
            case "4th Space": return 7f / 8f;
            case "5th Line": return 8f / 8f;
            default:
                Debug.Log("Invalid address: " + address);
                return -0.5f;
        } }

    private static int GetNByAddress(string address)
    {
        switch (address)
        {
            case "1st Line": return 0;
            case "1st Space": return 1;
            case "2nd Line": return 2;
            case "2nd Space": return 3;
            case "3rd Line": return 4;
            case "3rd Space": return 5;
            case "4th Line": return 6;
            case "4th Space": return 7;
            case "5th Line": return 8;
            default:
                return 0;
        }
    }

    private static string GetDifferential(string start, string end)
    {
        int s = GetNByAddress(start);
        int e = GetNByAddress(end);
        int diff = e - s;
        if (diff > 1)
        {
            return string.Format("{0} steps above", diff);
        }
        else if (diff == 1)
        {
            return "1 step above";
        }
        else if (diff == 0)
        {
            return "the same as";
        }
        else if (diff == -1)
        {
            return "1 step below";
        }
        else if (diff < -1)
        {
            return string.Format("{0} steps below", -diff);
        }
        else
        {
            return "across from";
        }
    }

    private void SwapBackgrounds()
    {
        BackgroundLeft = 1 - BackgroundLeft;
    }

    private void PlaceInBox(GameObject obj)
    {
        Bounds bounds = StaffCollider.bounds;
        float boundsAR = bounds.size.x / bounds.size.y;

        SpriteRenderer sr = obj.transform.Find("Staff").GetComponent<SpriteRenderer>();
        float srAR = sr.bounds.size.x / sr.bounds.size.y;

        float scaleAdjustment;

        if (boundsAR > srAR)
        {
            // Height is limiting factor
            scaleAdjustment = bounds.size.y / sr.bounds.size.y;
        } else
        {
            // Width is limiting factor
            scaleAdjustment = bounds.size.x / sr.bounds.size.x;
        }

        obj.transform.localScale *= scaleAdjustment;

        Vector3 posDelta = bounds.center - sr.gameObject.transform.position;

        var fncb = obj.GetComponent<AddressingStep>().FirstNoteCollider.bounds;
        fncb.center += posDelta;
        obj.transform.position += posDelta;

        //sr.gameObject.transform.position = bounds.center;
    }

    private GameObject[] LoadGrayNotes(GameObject stepObj, AddressingStep step, out GrayCircle correct)
    {
        Bounds bounds = step.NotesBox.bounds;
        float yMin = bounds.center.y - bounds.extents.y;
        float yMax = bounds.center.y + bounds.extents.y;
        float xMin = bounds.center.x - bounds.extents.x;
        float xMax = bounds.center.x + bounds.extents.x;

        correct = null;

        GameObject[] circles = new GameObject[step.Notes.Length - 1];
        int j = 0;

        for (int i = 0; i < step.Notes.Length; i++)
        {
            Vector3 position = new Vector3(xMin + (xMax - xMin) * (((float)i) / step.Notes.Length),
                                           yMin + (yMax - yMin) * GetYByAddress(step.Notes[i]), -2);
            GameObject gray = (GameObject)Instantiate(GrayCirclePrefab, position, Quaternion.identity);
            gray.name = step.Notes[i];
            gray.transform.parent = stepObj.transform;
            gray.name = step.Notes[i];
            GrayCircle circ = gray.GetComponent<GrayCircle>();
            bool isCorrect = (i == step.CorrectIndex);
            if (isCorrect)
            {
                circ.IsCorrect = true;
                correct = circ;
            } else
            {
                circ.IsCorrect = false;
                circles[j++] = gray;
            }
            circ.controller = this;
        }
        return circles;
    }

    private void ConfigureFirstStep()
    {
        CurrentStepObject = Instantiate(LevelSteps[0]);
        CurrentStep = CurrentStepObject.GetComponent<AddressingStep>();
        CurrentStepObject.transform.parent = this.transform;

        PlaceInBox(CurrentStepObject);
        IncorrectCircles = LoadGrayNotes(CurrentStepObject, CurrentStep, out CorrectCircle);

        firstNoteHeight = CurrentStep.FirstNoteCollider.bounds.center.y;

        //if (((levelNumber == 3) || (levelNumber == 4)) ||
        //      ((levelNumber == 5 || levelNumber == 6) && (Random.value < 0.5f)))
        //{
        //    SuperdogText.text = "Swing to the note " +
        //        GetDifferential(CurrentStep.StartNote, CurrentStep.Notes[CurrentStep.CorrectIndex]) +
        //        " the note you're hanging from!";
        //}
        //else
        //{
        //    SuperdogText.text = "Swing to the " + CurrentStep.Notes[CurrentStep.CorrectIndex] + "!";
        //}

        if (!IsFirstLevel && LevelSelection.IsAutoplaying())
        {
            CorrectCircleClicked(CorrectCircle);
        }
    }

    float firstNoteHeight;

    public bool InTutorial = false;
    public int TutorialIndex = 0;

    public void TutorialNextButtonPressed()
    {
        GBL_Interface.SendTutorialDialogSeen(levelNumber, TutorialIndex, !superdogAudioSource.isPlaying);
        // Logger.Instance.LogAction(string.Format("Address Level {0}", levelNumber),
        //    "Tutorial Button Pressed", string.Format("{0}", TutorialIndex));
        StartCoroutine(Tutorial());
    }

    public float Dark = 0.5f;
    public float Bright = 1f;

    private void HideLives()
    {
        foreach (GameObject go in Lives)
        {
            go.SetActive(false);
        }
    }

    private void ShowLives()
    {
        foreach (GameObject go in Lives)
        {
            go.SetActive(true);
        }
    }

    private IEnumerator Tutorial()
    {
        if (TutorialIndex == dialogueController.tutorialLength) // ONCE TUTORIAL MESSAGES ARE FINISHED
        {
            InTutorial = false;

            SuperdogButton.gameObject.SetActive(false); //keep
            StartCoroutine(Transition.TransitionBrightness(gameObject, Superdog, tutorialFadeTime, Dark, Bright)); //keep
            //yield return Transition.FadeOut(SuperdogText.gameObject, tutorialFadeTime); //keep
            //if (CurrentStep.TutorialObject == null)
            //{
            //    SuperdogText.text = "Swing to the " + CurrentStep.Notes[CurrentStep.CorrectIndex] + "!";
            //} else
            //{
            //    SuperdogText.text = CurrentStep.TutorialSuperdogText;
            //}
            dialogueController.updateDialogue(TutorialIndex++);

            if (LevelSelection.IsAutoplaying())
            {
                CorrectCircleClicked(CorrectCircle);
            }
            if (CurrentStep.TutorialObject != null)
            {
                CurrentStep.TutorialObject.SetActive(true);
            }
            TransitioningBackgrounds = false;
            ShowLives();
        }

        else if (TutorialIndex == 0) //first step of tutorial
        {
            TransitioningBackgrounds = true;
            HideLives();

            // TutorialObject is the arrows on the staff. This block hides it (since step 1 will have it and we don't want it shown while he's talkin)
            if (CurrentStep.TutorialObject != null)
            {
                CurrentStep.TutorialObject.SetActive(false);
            }

            SuperdogDialogue.SetActive(true);
            dialogueController.updateDialogue(TutorialIndex++);

            SuperdogButton.gameObject.SetActive(true);
            //SuperdogText.text = TutorialMessages[TutorialIndex++];
            InTutorial = true;
            StartCoroutine(Transition.FadeIn(SuperdogDialogue, tutorialFadeTime, false));
            yield return Transition.TransitionBrightness(gameObject, Superdog, tutorialFadeTime, Bright, Dark);
            if (LevelSelection.IsAutoplaying())
            {
                TutorialNextButtonPressed();
            }
        }

        else { //other tutorial steps
            SuperdogButton.gameObject.SetActive(false);
            // yield return Transition.FadeOut(SuperdogText.gameObject, tutorialFadeTime);
            dialogueController.updateDialogue(TutorialIndex++);

            // This block dictates when the staff will be brightened/darkened
            // PRESERVE FUNCTIONALITY
            if (TutorialIndex == 3)
            {
                StartCoroutine(Transition.TransitionBrightness(CurrentStepObject, null, tutorialFadeTime, Dark, Bright));
            }
            else if (TutorialIndex == 8)
            {
                StartCoroutine(Transition.TransitionBrightness(CurrentStepObject, null, tutorialFadeTime, Bright, Dark));
            }

            yield return new WaitForSeconds(tutorialFadeTime);
            if (TutorialIndex == dialogueController.tutorialLength)
            {
                SuperdogButton.GetComponentInChildren<Text>().text = "Let's go!";
            }
            SuperdogButton.gameObject.SetActive(true);
            if (LevelSelection.IsAutoplaying())
            {
                TutorialNextButtonPressed();
            }
        }
    }

    private IEnumerator InitializeSuperdog()
    {
        // Reset duration slot to track time spent in tutorial dialogs
        GBL_Interface.ResetTutorialDialogDurationSlot();
        yield return Tutorial();
    }

    protected void OnEnable()
    {
        _backgroundAudio.clip = AudioManagerUtility.JungleClip;
        _backgroundAudio.loop = true;
        _backgroundAudio.Play();
    }

    protected void Start() {
        LivesCount = Lives.Length;
        BackgroundDelta = (Backgrounds[BackgroundRight].transform.position -
                           Backgrounds[BackgroundLeft].transform.position);
        stepsCompleted = 0;
        ConfigureFirstStep();

        superdogController = Superdog.GetComponent<SuperdogController>();
        vineController = Vine.GetComponent<VineController>();
        dialogueController = SuperdogDialogue.GetComponent<DialogueController>();
        hintController = hintObject.GetComponent<HintController>();

        superdogAudioSource = SuperdogDialogue.GetComponent<AudioSource>();

        vineController.InitializeVineLength(CurrentStep);
        StartCoroutine(InitializeSuperdog());
        sgVelocity = Vector3.zero;

        //GBLXAPI
        GBL_Interface.SendLevelStarted(levelNumber);
    }

    protected void Update()
    {
        vineController.UpdateHangingVine(CurrentStep);
        vineController.NormalizeVineLength(CurrentStep, sgSpring, firstNoteHeight);


        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.Z) &&!CompletingLevel)
        {
            CompletingLevel = true;
            print("pressed");
            Destroy(CurrentStepObject);
            CurrentStepObject = Instantiate(LevelSteps[LevelSteps.Length - 1]);
            StartCoroutine(CompleteLevel());
        }

    }

    public void CorrectCircleClicked(GrayCircle circ)
    {
        if (TransitioningBackgrounds)
        {
            // Debug.Log("Failed click");
            return;
        }

        // Debug.Log("Successful click " + CurrentStep.name + " and transitionbackgrounds is " + TransitioningBackgrounds);
        TransitioningBackgrounds = true;
        // Logger.Instance.LogAction("Correct Circle", stepsCompleted.ToString(), "");
        if (++stepsCompleted == LevelSteps.Length)
        {
            StartCoroutine(LevelComplete(circ));
        }
        else
        {
            StartCoroutine(LoadNextStep(circ));
        }
    }

    public void IncorrectCircleClicked(GrayCircle circ)
    {
        if (TransitioningBackgrounds)
        {
            return;
        }
        // Logger.Instance.LogAction("Incorrect Circle", stepsCompleted.ToString(), circ.name);

        // Debating on where to put this code //
        TransitioningBackgrounds = true;

        if (LivesCount == 0)
        {
            Debug.Log("Fail from IncorrectCircleClicked()");

            SuperdogDialogue.SetActive(false);
            GameOver.SetActive(true);
        }
        else
        {
            TransitioningBackgrounds = false;
        }
        //                                    //

        StartCoroutine(VineFakeout(circ));

    }

    private IEnumerator LevelComplete(GrayCircle circ)
    {
        // Debug.Log("TODO");
        yield return null;
    }

    private IEnumerator LoseLife()
    {
        GameObject losing = Lives[LivesCount - 1];
        --LivesCount;
        if (LivesCount == 0)
        {
            //GBLXAPI
            levelAttempts++;
            GBL_Interface.SendLevelFailed(levelNumber);

            SuperdogDialogue.SetActive(false);
            GameOver.SetActive(true);
        }
        StartCoroutine(Transition.Resize(losing.transform, losing.transform.localScale * 2, 2.4f));
        yield return Transition.FadeOut(losing, 2.4f, Transition.FinishType.Destroy);

    }

    private IEnumerator Zap(GrayCircle circ)
    {
        StartCoroutine(LoseLife());
        StartCoroutine(Audio.Zap());
        yield return Transition.TransitionColor(circ.gameObject, 0.4f, Color.white, Color.red);
        StartCoroutine(Transition.TransitionColor(circ.gameObject, 0.3f, Color.red, Color.white));
        yield return null;
    }


    // Revise this function and move to vine controller
    private IEnumerator VineFakeout(GrayCircle circ)
    {
        // yield return Transition.Rotate(SupergirlArm.transform, swaptime / 2, 0f, 160f);

        SupergirlVineCurled.SetActive(false);
        SupergirlAnimator.SetTrigger("noteClicked"); // Trigger vine throwing animation
        yield return vineController.ThrowVine(ThrowingVine, SupergirlVineCurled.transform.position, circ.transform.position, swaptime / 2);

        StartCoroutine(Zap(circ));
        yield return vineController.RetractVine(ThrowingVine, SupergirlVineCurled.transform.position, circ.transform.position, swaptime / 5);

        SupergirlVineCurled.SetActive(true);
        yield return Transition.Rotate(SupergirlArm.transform, swaptime / 2, 160f, 0f);
    }

    private IEnumerator LoadNextStep(GrayCircle circ)
    {
        // Debug.Log("loading next step" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        hintController.HideHint();
        HintDisplayed = false;

        bool isLastLevel;
        GameObject[] nextIncorrect;
        AddressingStep OldStep = CurrentStep;
        GameObject OldStepObject = CurrentStepObject;
        GameObject NewStepObject = Instantiate(LevelSteps[stepsCompleted]);
        AddressingStep NextStep = NewStepObject.GetComponent<AddressingStep>();
        GrayCircle NextCorrect = null;
        isLastLevel = NextStep.Notes.Length == 0;
        NewStepObject.transform.parent = this.transform;
        TransitioningBackgrounds = true;

        // Debug.Log("transitioningBackgrounds set to True" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        if (OldStep.TutorialObject != null)
        {
            Destroy(OldStep.TutorialObject);
            // Debug.Log("Destroyed OldStep.TutorialObject" + " and transitionbackgrounds is " + TransitioningBackgrounds);
        }

        //PlaceInBox(NewStepObject);

        float currentNoteBoxHeight = OldStep.NotesBox.bounds.size.y;
        float newNoteBoxHeight = NextStep.NotesBox.bounds.size.y;
        NewStepObject.transform.localScale *= (currentNoteBoxHeight / newNoteBoxHeight);
        Vector3 positionDeltaY = Vector3.up * (OldStep.NotesBox.bounds.center.y - NextStep.NotesBox.bounds.center.y);
        NewStepObject.transform.position += positionDeltaY;

        if (!isLastLevel)
        {
            nextIncorrect = LoadGrayNotes(NewStepObject, NextStep, out NextCorrect);
        } else
        {
            nextIncorrect = new GameObject[0];
        }

        Vector3 offset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - NextStep.FirstNoteCollider.bounds.center);
        Vector3 finalOffset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - OldStep.FirstNoteCollider.bounds.center);
        NewStepObject.transform.position += offset;

        OldStepObject.transform.position += Vector3.forward * 0.2f;

        SuperdogDialogue.SetActive(false);

        // Debug.Log("Placed new object in box, deactivated Superdog" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        // Stage 1: "Swap"
        StartCoroutine(Transition.FadeOut(IncorrectCircles, swaptime, Transition.FinishType.Destroy));
        StartCoroutine(Transition.FadeOut(circ.gameObject, swaptime));
        if (NextStep.TutorialAlpha != null) { NextStep.TutorialAlpha.SetActive(false); }
        StartCoroutine(Transition.FadeIn(NewStepObject, swaptime, exclude: NextStep.TutorialAlpha));
        //yield return Transition.Rotate(SupergirlArm.transform, swaptime / 2, 0f, 160f);

        //Debug.Log("Finished swap" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        // Stage 1.5: Throw vine
        SupergirlVineCurled.SetActive(false);
        SupergirlAnimator.SetTrigger("noteClicked"); // Trigger vine throwing animation
        yield return vineController.ThrowVine(ThrowingVine, SupergirlVineCurled.transform.position, circ.transform.position, swaptime / 2);
        Audio.PlayNote(CurrentStep.Notes[CurrentStep.CorrectIndex]);

        // Debug.Log("Vine thrown" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        if (circ != null) { Destroy(circ.gameObject); }
        

        // Debug.Log("circle destroyed" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        CurrentStep = NextStep;
        CurrentStepObject = NewStepObject;
        IncorrectCircles = nextIncorrect;
        CorrectCircle = NextCorrect;

        // Debug.Log("Switched all the variables" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        if (!isLastLevel)
        {
            StartCoroutine(TransitionBackgrounds());
            StartCoroutine(Transition.Translate(CurrentStepObject.transform, CurrentStepObject.transform.position - finalOffset, flytime));
            StartCoroutine(superdogController.FlySuperdog(flytime));
        } else
        {
            StartCoroutine(Transition.Translate(CurrentStepObject.transform, Vector3.Scale(CurrentStepObject.transform.position, new Vector3(0f, 1f, 1f)), flytime));
        }
        StartCoroutine(Transition.FadeOut(OldStepObject, flytime));
        StartCoroutine(Transition.Translate(OldStepObject.transform, OldStepObject.transform.position - finalOffset, flytime));
        SupergirlVineCurled.SetActive(true);
        StartCoroutine(Transition.Rotate(SupergirlArm.transform, flytime, 160f, 0f));

        yield return new WaitForSeconds(flytime + 0.2f);

        // Debug.Log("Supergirl has flown" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        TransitioningBackgrounds = false;

        // Debug.Log("transitioningBackgrounds set to False" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        if (!isLastLevel)
        {
            SuperdogDialogue.SetActive(true);
            dialogueController.updateDialogue(TutorialIndex++);

            // Debug.Log("Superdog reactivated" + " and transitionbackgrounds is " + TransitioningBackgrounds);
        }

        Destroy(OldStepObject);

        // Debug.Log("Old step object destroyed" + " and transitionbackgrounds is " + TransitioningBackgrounds);

        if (NextStep.TutorialAlpha != null) { NextStep.TutorialAlpha.SetActive(true); }

        if (isLastLevel)
        {
            yield return CompleteLevel();
        } else if (LevelSelection.IsAutoplaying())
        {
            CorrectCircleClicked(CorrectCircle);
            // Debug.Log("Called CorrectCircleClicked" + " and transitionbackgrounds is " + TransitioningBackgrounds);
        }

        
    }

    private IEnumerator FadeInLevelComplete(float duration)
    {
        LevelCompleteObject.SetActive(true);
        Vector3 levelCompleteInitialScale = LevelCompleteObject.transform.localScale;

        StartCoroutine(Transition.Resize(LevelCompleteObject.transform, levelCompleteInitialScale * 1.6f, duration));
        yield return Transition.FadeIn(LevelCompleteObject, duration / 4, false);
        yield return new WaitForSeconds(duration / 2);
        yield return Transition.FadeOut(new GameObject[] { LevelCompleteObject }, duration / 3, Transition.FinishType.Destroy);
    }

    private IEnumerator CompleteLevel()
    {
        // GBLXAPI
        GBL_Interface.SendLevelCompleted(levelNumber, levelAttempts, LivesCount);
        
        print("entered");
        StartCoroutine(FadeInLevelComplete(flytime * 1.4f));
        yield return superdogController.FlySuperdogAway(flytime);

        GameObject measureObject = CurrentStepObject.GetComponentInChildren<SpriteRenderer>().gameObject;
        StartCoroutine(Transition.Translate(measureObject.transform, 6f * Vector3.back, 0.6f));
        yield return Transition.TransitionBrightness(gameObject, measureObject, 0.6f, Bright, Dark);

        yield return Audio.PlayMeasure();
        LevelSelection.LevelCompleted(this.levelNumber, measureObject.transform);

        
    }

    public void ClearBackground()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator TransitionBackgrounds()
    {
        TransitioningBackgrounds = true;
        Vector3 startPosition = BackgroundParent.transform.position;
        Vector3 endPosition = startPosition - BackgroundDelta;

        yield return Transition.Translate(BackgroundParent.transform, endPosition, flytime);

        SwapBackgrounds();
        Backgrounds[BackgroundRight].transform.position += 2 * BackgroundDelta;
        // TransitioningBackgrounds = false;
    }

    public void ShowHelp()
    {
        if (!InTutorial && !HintDisplayed && CurrentStep.TutorialObject == null) {
            hintController.ShowHint(CurrentStep);

            if (LivesCount == 0)
            {
                Debug.Log("Fail from ShowHelp()");
                SuperdogDialogue.SetActive(false);
                GameOver.SetActive(true);
            }
            else
            {
                TransitioningBackgrounds = false;
            }
            //                                    //

            if (levelNumber != 1 && levelNumber != 2)
            {
                StartCoroutine(LoseLife());
            }
            HintDisplayed = true;

            // GBLXAPI
            GBL_Interface.SendHintRequested(levelNumber);
        }
    }
}
