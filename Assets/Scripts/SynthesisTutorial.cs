using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynthesisTutorial : MonoBehaviour {

    public GameObject[] tutorialPrefabs;
    public GameObject tutorialLevelIndicator;
    public GameObject superDogTutorial;
    public GameObject emptyStaff;
    public GameObject firstNote;

    private int tutorialBoxesRemaining;

    private Minion tutorialMinion;
    private Note tutorialNote;
    private List<Minion> minions;
    private List<Note> notes;

    private GameObject currentTutorialBox;
    private TutorialBox currentTutorialBoxScript;

    private bool finished;

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

    private void EnableSuperDogTutorial()
    {
        if (superDogTutorial != null)
        {
            superDogTutorial.SetActive(true);
        }
    }

    private void DisableSuperDogTutorial()
    {
        if (superDogTutorial != null)
        {
            superDogTutorial.SetActive(false);
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
            // TODO
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

    // Use this for initialization
    void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        this.tutorialLevelIndicator.GetComponent<SpriteRenderer>().enabled = false;

        finished = false;
    }

    // Update is called once per frame
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
