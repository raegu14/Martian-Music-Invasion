using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynthesisTutorial : MonoBehaviour {

    /* public tutorial objects */
    public GameObject[] tutorialPrefabs;
    public GameObject tutorialLevelIndicator;
    public GameObject superDogTutorial;
    public GameObject emptyStaff;
    public GameObject firstNote;

    /* Lists for interactable objects */
    private Minion tutorialMinion;
    private Note tutorialNote;
    private List<Minion> minions;
    private List<Note> notes;

    /* track current tutorial position */
    private GameObject currentTutorialBox;
    private TutorialBox currentTutorialBoxScript;

    /* track tutorial status */
    private bool finished;
    private int tutorialBoxesRemaining;

    /* Zooming components */
    [Header("Camera")]
    private GameObject cam;
    public float zoomSpeed;
    private bool zooming;
    private Vector3 targetPos;
    private float targetSize;
    private Vector3 prevPos;
    private float prevSize;
    private float t;


    public bool showingTutorials
    {
        get
        {
            return tutorialBoxesRemaining > 0;
        }
    }

    private void EnableSuperDogTutorial()
    {
        if (superDogTutorial != null)
        {
            superDogTutorial.SetActive(true);
            Superdog.singleton.HideSuperdog();
        }
    }

    private void DisableSuperDogTutorial()
    {
        if (superDogTutorial != null)
        {
            superDogTutorial.SetActive(false);
            Superdog.singleton.ShowSuperdog();
        }
    }

    private void DisableEmptyStaff()
    {
        if (emptyStaff != null)
        {
            emptyStaff.SetActive(false);
        }
    }

    private void DisableFirstNote()
    {
        if (firstNote != null)
        {
            firstNote.SetActive(false);
        }
    }

    /* Initialize Tutorial */
    public void InitTutorials(List<Minion> minionList, List<Note> noteList)
    {
        this.tutorialBoxesRemaining = this.tutorialPrefabs.Length;
        minions = minionList;
        notes = noteList;
        if (this.showingTutorials)
        {
            StartCoroutine(this.OpenTutorialBox());
        }
    }

    /* Opens the next tutorial box and sets up tutorial phase */
    private IEnumerator OpenTutorialBox()
    {
        yield return new WaitForSeconds(0.1f);
        int prefabIx = this.tutorialPrefabs.Length - this.tutorialBoxesRemaining;
        if (prefabIx != tutorialPrefabs.Length - 1)
        {
            yield return Zoom(this.tutorialPrefabs[prefabIx]);
        }

        GameObject box = Instantiate<GameObject>(this.tutorialPrefabs[prefabIx]);

        Button completeButton = box.GetComponentInChildren<Button>();
        if (completeButton != null)
        {
            completeButton.onClick.AddListener(this.CloseTutorialBox);
        }

        TutorialBox tbox = box.GetComponent<TutorialBox>();
        this.tutorialNote = null;
        this.tutorialMinion = null;
        if (tbox != null)
        {
            tbox.Open(this.minions, this.notes);
            this.tutorialMinion = tbox.minion;
            this.tutorialNote = tbox.note;
        }

        this.currentTutorialBox = box;
        this.currentTutorialBoxScript = tbox;
    }

    /* Handles interactivity for this tutorial box */
    public void NoteClickedInTutorial(Note n)
    {
        if (n == this.tutorialNote)
            this.CloseTutorialBox();
    }

    public void MinionClickedInTutorial(Minion m)
    {
        if (m == this.tutorialMinion)
            this.CloseTutorialBox();
    }

    /* Finishes this tutorial box and calls the new tutorial box */
    public void CloseTutorialBox()
    {
        TutorialBox tbox = this.currentTutorialBoxScript;
        if (tbox != null)
        {
            tbox.Close();
        }

        Destroy(this.currentTutorialBox);
        this.tutorialBoxesRemaining--;
        if (this.showingTutorials)
            StartCoroutine(this.OpenTutorialBox());
        else
        {
            //Close tutorial
            finished = true;
        }
    }

    public IEnumerator CleanUpTutorial()
    {
        this.DisableSuperDogTutorial();
        this.DisableEmptyStaff();
        this.DisableFirstNote();

        if (!finished)
        {
            TutorialBox tbox = this.currentTutorialBoxScript;
            if (tbox != null)
            {
                tbox.Close();
            }

            Destroy(this.currentTutorialBox);
        }

        zoomSpeed = 0.1f;
        GameObject tmp = new GameObject();
        tmp.AddComponent<Zoom>();
        tmp.GetComponent<Zoom>().pos = new Vector3(0, 0, -10);
        tmp.GetComponent<Zoom>().size = 4.25f;
        yield return Zoom(tmp);
    }

    private void AutoMatch()
    {
        if (this.showingTutorials)
        {
            if (this.currentTutorialBox != null)
                this.CloseTutorialBox();
            return;
        }
    }

    /* Initiates zoom and waits until zoom is completed */
    public IEnumerator Zoom(GameObject tPrefab)
    {
        Zoom z = tPrefab.GetComponent<Zoom>();
        if (z != null)
        {
            t = 0;
            prevPos = transform.position;
            prevSize = cam.GetComponent<Camera>().orthographicSize;
            targetPos = z.pos;
            targetSize = z.size;
            zooming = true;
            yield return new WaitForSeconds(1f / zoomSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.5f);
            if (z.superDogTutorial)
            {
                EnableSuperDogTutorial();
            }
        }
    }

    public bool IsFinishedTutorial()
    {
        return finished;
    }

    /* Set initial parameters */
    void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        this.tutorialLevelIndicator.GetComponent<SpriteRenderer>().enabled = true;

        finished = false;
    }

    /* Handles zooming if activated */
    void Update () {

        if (zooming)
        {
            t += zoomSpeed;
            cam.transform.position = Vector3.Lerp(prevPos, targetPos, t);
            cam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(prevSize, targetSize, t);

            if (t >= 1)
            {
                zooming = false;
            }
        }

    }
}
