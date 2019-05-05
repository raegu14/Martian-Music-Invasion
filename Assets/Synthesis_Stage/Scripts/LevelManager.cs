using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour {

    [SerializeField]
    private Gameplay_Progress _gameProgress = null;
    public uint levelNumber;
	public uint maxLives = 3;

	public GameObject[] lifePrefabs;
	public GameObject allLivesLostPrefab;
    public GameObject[] glow;

    private bool completingLevel;

	private GameObject[] lifeObjects;

	private uint livesRemaining;

	public static LevelManager singleton;

	private List<Minion> minions;
	private List<Minion> minionsAutoclicked;
	private List<Note> notes;
	private List<Note> notesAutoclicked;
	private Random random;

	private Hero hero;
	private BackgroundClick background;

    public SpriteRenderer MeasureSR;
	private Transform measureTransform;
    public SynthesisTutorial tutorial;
    private bool finishedTutorial;

	private Dictionary<char, uint> numNeeded, numRemaining;

	// Source to play clips from
	private AudioSource backgroundAudioSource;
	private AudioSource zapAudioSource;

	// Individual note AudioClips
	public AudioClip[] noteClips;
	private Dictionary<string, AudioSource> noteNameToSource;

	// Final measure clip
	public AudioClip measureClip;

	// Fail note clip
	public AudioClip noteFailClip;

	// Background music clip
	public AudioClip buildingsBackground;

	public GameObject DemoPopupGO;


	private static class Constants
	{
		public static readonly uint firstChordLevel = 22;

		// The amount of time that it takes an audio clip to load before playing it
		// probably machine dependent, but fuck it for now
		// need a better way to avoid this delay...
		public static readonly float audioDelay = 0.2f;

		public static readonly float measureCenterTime = 1f;

		public static readonly Color32 semiTransparent = new Color32(0xFF, 0xFF, 0xFF, 0x80);

        public static readonly Color32 superTransparent = new Color32(040, 0x40, 0x40, 0x40);

		public static readonly float lifeDistance = 0.2f;
	}

	public static float audioDelay {
		get { return Constants.audioDelay; }
	}

	public bool ChordsAllowed () {
		return this.levelNumber >= Constants.firstChordLevel;
	}

	public void PrePlayNote(Note note, float delay) {
		StartCoroutine (NoteMatchDelayed (note, delay));
	}

	public void PreFailNote(Note note, float delay) {
		StartCoroutine (NoteFailDelayed (delay));
	}

	private IEnumerator NoteMatchDelayed(Note note, float delay) {
		// Set up the playing of the sounds
		float maxClipTime = 0f;
		foreach (string name in note.names) {
			AudioSource src = this.noteNameToSource[name];
			this.noteNameToSource[name].PlayDelayed(delay - Constants.audioDelay);
			maxClipTime = Mathf.Max (maxClipTime, src.clip.length);
		}

		// Wait for the note to finish playing
		yield return new WaitForSeconds (delay);

		// Correct Match
		this.notesRemaining--;

		Logger.Instance.LogAction ("LevelManager", "Progress", string.Format ("{0} Notes Remaining, {1} Lives Remaining", this.notesRemaining, this.livesRemaining));

		Superdog.singleton.HideHelp ();

		if (this.notesRemaining == 0)
			this.CompleteLevel ();
	}

	private IEnumerator NoteFailDelayed(float delay) {
		this.zapAudioSource.PlayDelayed (delay - Constants.audioDelay);

		yield return new WaitForSeconds (delay);

		this.LoseLife ();

		Logger.Instance.LogAction ("LevelManager", "Progress", string.Format ("{0} Notes Remaining, {1} Lives Remaining", this.notesRemaining, this.livesRemaining));
	}

	public bool StillNeedsMinion(Minion m) {
		uint needed = 0, remaining = 0;

		this.numNeeded.TryGetValue (m.letter, out needed);
		this.numRemaining.TryGetValue (m.letter, out remaining);

		return (remaining <= needed);
	}

	public void DoneWithMinion(Minion m) {
		if (this.minionsAutoclicked.Contains (m))
			this.minionsAutoclicked.Remove (m);
	}

	public void CompleteLevel() {
        completingLevel = true;
		if (this.livesRemaining <= 0) {
			// nice try...
			return;
		}
		StartCoroutine (CompleteLevelAsync ());
	}

	private void SetChildrenColor(GameObject obj, Color32 color, GameObject except=null) {
		foreach (SpriteRenderer rend in obj.GetComponentsInChildren<SpriteRenderer>()) {
			if (rend.gameObject == except)
				continue;
			rend.color = color;
		}
	}

	private void DimChildren(GameObject obj, GameObject except=null) {
		this.SetChildrenColor (obj, Constants.semiTransparent, except);
	}

    private void SuperdimChildren(GameObject obj)
    {
        this.SetChildrenColor(obj, Constants.superTransparent, null);
    }

    public void ClearBackground()
    {
        Destroy(this.gameObject);
    }

	private void UndimChildren(GameObject obj) {
		this.SetChildrenColor (obj, new Color32 (0xFF, 0xFF, 0xFF, 0xFF));
	}
	
    private void CleanUpNotes()
    {
        GameObject measure = GameObject.Find("Measure");
        for (int i = 0; i < measure.transform.childCount; i++)
        {
            if(measure.transform.GetChild(i).name.Contains("Note"))
            {
                measure.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

	private IEnumerator CompleteLevelAsync() {
        yield return CloseTutorial();
        this.CleanUpNotes();
		GameManager.currentLevel = (int)(this.levelNumber + 1);

		// Move the measure to the center of the screen
		float moveTime = LevelManager.Constants.measureCenterTime;
		float currentTime = 0f;

		Vector3 measureStart = this.measureTransform.position;
		Vector3 measureEnd = new Vector3 (0, 0, -8);
        if (MeasureSR)
        {
            MeasureSR.sortingOrder = 30;
        }

		Vector3 measureScaleStart = this.measureTransform.localScale;
		Vector3 measureScaleEnd = measureScaleStart * 1.5f;

		this.DimChildren (this.gameObject);

		float t;

		while (currentTime <= moveTime) {
			t = currentTime / moveTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);

			this.measureTransform.position = Vector3.Lerp (measureStart, measureEnd, t);
			this.measureTransform.localScale = Vector3.Lerp (measureScaleStart, measureScaleEnd, t);

			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}
		this.measureTransform.position = measureEnd;

		// Play the final measure
		AudioClip clip = this.measureClip;
		this.backgroundAudioSource.clip = clip;
		this.backgroundAudioSource.volume = 1f;
		this.backgroundAudioSource.Play ();

        for (int i = 0; i < glow.Length; i++)
        {
            glow[i].GetComponent<Glow>().StartGlow();
        }

		yield return new WaitForSeconds (clip.length + 0.2f);

        this.SuperdimChildren(this.gameObject);

        /*
		// Conditional for demo popup
		if (this.levelNumber == 12) {
			this.ShowDemoPopup();
		} else {
			LevelSelection.LevelCompleted(this.levelNumber, this.measureTransform);
		}
        */

        _gameProgress.CompleteLevel((int)levelNumber);
    }

    public void RegisterNote(Note note) {
		uint charCount;
		foreach (char c in note.letters) {
			charCount = 0;
			this.numNeeded.TryGetValue(c, out charCount);
			this.numNeeded[c] = charCount + 1;
		}
		this.notesRemaining++;
		this.notes.Add (note);
		this.measureTransform = note.transform.parent;
	}

	public void RegisterMinion(Minion minion) {
		uint charCount = 0;
		this.numRemaining.TryGetValue (minion.letter, out charCount);
		this.numRemaining [minion.letter] = charCount + 1;

		this.minions.Add (minion);
	}

	public void DeregisterNote(Note note) {
		foreach (char c in note.letters) {
			this.numNeeded[c]--;
		}

		this.notes.Remove(note);
	}

	public void DeregisterMinion(Minion minion) {
		this.numRemaining [minion.letter]--;
		this.minions.Remove (minion);
	}

    //Opens hint
	public void HelpRequested () {
		Logger.Instance.LogAction ("LevelManager", "Help Requested", string.Format ("{0} Lives Remaining. Is tutorial: {1}", this.livesRemaining, this.tutorial != null));
		if (this.tutorial == null) {
			this.LoseLife();
		}
	}

    //Handles life lost behavior
	private void LoseLife() {
		this.livesRemaining--;

		GameObject lifeLost = this.lifeObjects [this.livesRemaining];
		StartCoroutine (this.DisappearLife (lifeLost));

		if (this.livesRemaining <= 0) {
			this.DimChildren (this.gameObject);
			this.DimChildren (this.measureTransform.gameObject);
			GameObject noLives = Instantiate<GameObject>(this.allLivesLostPrefab);
			noLives.GetComponentInChildren<Button>().onClick.AddListener(this.Retry);
		}
	}

    //Controls animation for losing a life gameobject
	private IEnumerator DisappearLife(GameObject life) {
		float destTime = 2.5f;
		float currentTime = 0f;

		Vector3 destScale = new Vector3 (3f, 3f, 1f);
		Vector3 initialScale = life.transform.localScale;

		SpriteRenderer rend = life.GetComponent<SpriteRenderer> ();
		Color32 color;

		while (currentTime < destTime) {
			float t = (currentTime / destTime);
			t = 1 - (t-1) * (t-1);
			life.transform.localScale = Vector3.Lerp(initialScale, destScale, t);

			float transp = 255f * (1 - t);
			color = new Color32(0xFF,0xFF,0xFF, (byte)(uint)transp);
			rend.color = color;

			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}

		Destroy (life);
	}

	public void Retry () {
		Logger.Instance.LogAction ("LevelManager", "Restart", "");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name /* Application.loadedLevelName */);
	}

	private uint notesRemaining;

	void Awake () {
		LevelManager.singleton = this;
		this.notesRemaining = 0;

		this.notes = new List<Note> ();
		this.notesAutoclicked = new List<Note> ();
		this.minions = new List<Minion> ();
		this.minionsAutoclicked = new List<Minion> ();

		this.numNeeded = new Dictionary<char, uint> ();
		this.numRemaining = new Dictionary<char, uint> ();
	}

	// Use this for initialization
	void Start () {

		this.hero = Hero.singleton;
		this.background = BackgroundClick.singleton;

		this.backgroundAudioSource = this.GetComponent<AudioSource> ();

		this.zapAudioSource = this.gameObject.AddComponent<AudioSource> ();
		this.zapAudioSource.clip = this.noteFailClip;

		this.noteNameToSource = new Dictionary<string, AudioSource> ();

		foreach (AudioClip clip in this.noteClips) {
			AudioSource src = this.gameObject.AddComponent<AudioSource>();
			src.clip = clip;
			clip.LoadAudioData();
			this.noteNameToSource[clip.name] = src;
		}

		this.InitLives ();

		AudioClip backgroundClip = buildingsBackground;
		this.backgroundAudioSource.clip = backgroundClip;
		this.backgroundAudioSource.Play ();

        completingLevel = false;

        if (this.tutorial != null)
        {
            finishedTutorial = false;
            StartCoroutine(StartTutorial());
        }
    }

    //set up references to life indicator objects
	private void InitLives() {
		GameObject[] lifePrefabs = this.lifePrefabs;

		this.livesRemaining = this.maxLives;
		this.lifeObjects = new GameObject[this.maxLives];

		for (uint i = 0; i < this.maxLives; i++) {
			GameObject lifePrefab = lifePrefabs[i % lifePrefabs.Length];
			GameObject life = Instantiate<GameObject>(lifePrefab);
			life.transform.parent = this.transform;
			life.transform.position += (i / lifePrefabs.Length) * Constants.lifeDistance * Vector3.up;
			this.lifeObjects[i] = life;
		}
	}

    public void MinionPickedUp(Minion baseMinion, HashSet<Minion> sanityCheck=null)
    {
        if (sanityCheck == null)
        {
            sanityCheck = new HashSet<Minion>();
        }

        sanityCheck.Add(baseMinion);

        // When a minion is picked up, we need to make sure
        // there were no other minions on top of it
        foreach (Minion m in baseMinion.supporting)
        {
            if (!sanityCheck.Contains(m))
            {
                m.EnableGravity();
                MinionPickedUp(m);
            }
        }
    }

    private IEnumerator StartTutorial()
    {
        //wait for references to be set
        yield return new WaitForSeconds(0.5f);

        //disable clicks on background
        this.background.DisableClicks();

        //disable clicks on minions
        foreach (SpriteRenderer s in this.minions[0].transform.parent.GetComponentsInChildren<SpriteRenderer>())
        {
            s.color = Constants.semiTransparent;
        }
        foreach (Minion m in this.minions)
        {
            m.DisableClicks();
        }

        //Disable Notes
        foreach (Note n in this.notes)
        {
            n.DisableClicks();
        }

        //call initTutorial function on tutorial script
        tutorial.InitTutorials(minions, notes);
    }

    private IEnumerator CloseTutorial()
    {
        print("closing tutorial");
        if (tutorial != null)
        {
            yield return tutorial.CleanUpTutorial();
        }

        //Enable clicks on background
        this.background.EnableClicks();
        
        //Enable clicks on minions
        foreach (Minion m in this.minions)
        {
            m.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            m.EnableClicks();
        }

        //Enable clicks on notes
        foreach (Note n in this.notes)
        {
            n.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            n.EnableClicks();
        }
    }

	// Update is called once per frame
	void Update () {
        if(tutorial != null && tutorial.IsFinishedTutorial() && !finishedTutorial)
        {
            print("finishing tutorial");
            finishedTutorial = true;
            StartCoroutine(CloseTutorial());
        }

		if (Input.GetKey (KeyCode.Q) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.Z) && !completingLevel) {
			this.CompleteLevel();
		}
	}

	void ShowDemoPopup () {
		Debug.Log("demo popup");
		this.DemoPopupGO.SetActive(true);
	}
}
