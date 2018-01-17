using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour {
	
	public uint levelNumber;
	public uint maxLives = 3;

    public bool UseTutorials = true;

	public GameObject[] lifePrefabs;
	public GameObject allLivesLostPrefab;
	public GameObject[] tutorialPrefabs;
	public GameObject tutorialLevelIndicator;
    public GameObject superDogTutorial;
    public GameObject emptyStaff;
    public GameObject firstNote;
    public GameObject glow;

    private int tutorialBoxesRemaining;
	private GameObject[] lifeObjects;

	public bool showingTutorials {
		get {
			return tutorialBoxesRemaining > 0;
		}
	}

	private uint livesRemaining;

	public static LevelManager singleton;

	private List<Minion> minions;
	private List<Minion> minionsAutoclicked;
	private List<Note> notes;
	private List<Note> notesAutoclicked;
	private Random random;

	private Hero hero;
	private BackgroundClick background;

	
	private Minion tutorialMinion;
	private Note tutorialNote;

	private Transform measureTransform;
	
	public bool isTutorialLevel {
		get {
			return this.levelNumber % 3 == 1;
		}
	}

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

    [Header("Camera")]
    public GameObject cam;
    public float zoomSpeed;
    private bool zooming;
    private Vector3 targetPos;
    private float targetSize;
    private Vector3 prevPos;
    private float prevSize;
    private float t;

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
        this.CleanUpTutorial();
        this.CleanUpNotes();
		GameManager.currentLevel = (int)(this.levelNumber + 1);

		// Move the measure to the center of the screen
		float moveTime = LevelManager.Constants.measureCenterTime;
		float currentTime = 0f;

		Vector3 measureStart = this.measureTransform.position;
		Vector3 measureEnd = new Vector3 (0, 0, -9);

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

        glow.GetComponent<Glow>().StartGlow();

		yield return new WaitForSeconds (clip.length + 0.2f);

        this.SuperdimChildren(this.gameObject);

        // @DEPRECATED
        //DontDestroyOnLoad (this.measureTransform.gameObject);
        //GameManager.SetMeasure (this.measureTransform.gameObject, this.measureTransform);
        //SceneManager.LoadScene("LevelSelection");

        LevelSelection.LevelCompleted(this.levelNumber, this.measureTransform, this);
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

	public void HelpRequested () {
		Logger.Instance.LogAction ("LevelManager", "Help Requested", string.Format ("{0} Lives Remaining. Is tutorial: {1}", this.livesRemaining, this.isTutorialLevel));
		if (!this.isTutorialLevel) {
			this.LoseLife();
		}
	}

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
		if (!this.isTutorialLevel)
			this.tutorialLevelIndicator.GetComponent<SpriteRenderer> ().enabled = false;

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

		this.InitTutorials ();
	}

	private void InitLives() {
		//Transform lives = this.transform.FindChild ("Lives");

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
	
	private void InitTutorials() {
        if (!this.UseTutorials)
        {
            return;
        }
		this.tutorialBoxesRemaining = this.tutorialPrefabs.Length;
		if (this.showingTutorials) {
			StartCoroutine( this.OpenFirstTutorialBox());
		}
	}

	private GameObject currentTutorialBox;
	private TutorialBox currentTutorialBoxScript;

	public void DisableBackground() {
		this.background.DisableClicks ();
	}

	public void EnableBackground() {
		this.background.EnableClicks ();
	}

	private void DisableMinions() {
		foreach (SpriteRenderer s in this.minions[0].transform.parent.GetComponentsInChildren<SpriteRenderer>()) {
			s.color = Constants.semiTransparent;
		}
		foreach (Minion m in this.minions) {
			m.DisableClicks();
		}
	}
	
	private void EnableMinions() {
		foreach (Minion m in this.minions) {
			m.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			m.EnableClicks();
		}
	}

	private void DisableNotes() {
		foreach (Note n in this.notes) {
			//n.gameObject.GetComponent<SpriteRenderer>().color = Constants.semiTransparent;
			n.DisableClicks();
		}
	}

	private void EnableNotes() {
		foreach (Note n in this.notes) {
			n.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
			n.EnableClicks();
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
        if(emptyStaff != null)
        {
            emptyStaff.SetActive(false);
        }
    }

    private void DisableFirstNote()
    {
        if(firstNote != null)
        {
            firstNote.SetActive(false);
        }
    }

	private IEnumerator OpenFirstTutorialBox() {
		yield return new WaitForSeconds (0.1f);
		this.DisableBackground();
		this.DisableMinions();
		this.DisableNotes();
		StartCoroutine(this.OpenNextTutorialBox ());
	}

	private IEnumerator OpenNextTutorialBox() {
		yield return new WaitForSeconds (0.1f);
		int prefabIx = this.tutorialPrefabs.Length - this.tutorialBoxesRemaining;
        if (prefabIx != tutorialPrefabs.Length - 1)
        {
            yield return Zoom(this.tutorialPrefabs[prefabIx]);
        }

        GameObject box = Instantiate<GameObject>(this.tutorialPrefabs[prefabIx]);

        Button completeButton = box.GetComponentInChildren<Button> ();
		if (completeButton != null) {
			// TODO
			completeButton.onClick.AddListener(this.CloseTutorialBox);
		}

		TutorialBox tbox = box.GetComponent<TutorialBox> ();
        this.tutorialNote = null;
		this.tutorialMinion = null;
		if (tbox != null) {
			tbox.Open(this.minions, this.notes);
			this.tutorialMinion = tbox.minion;
			this.tutorialNote = tbox.note;
        }

		this.currentTutorialBox = box;
		this.currentTutorialBoxScript = tbox;
	}

	public void NoteClickedInTutorial(Note n) {
		if (n == this.tutorialNote)
			this.CloseTutorialBox ();
	}

	public void MinionClickedInTutorial(Minion m) {
		if (m == this.tutorialMinion)
			this.CloseTutorialBox ();
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

    public void CloseTutorialBox() {
		TutorialBox tbox = this.currentTutorialBoxScript;
		if (tbox != null) {
			tbox.Close();
		}

		Destroy (this.currentTutorialBox);
		this.tutorialBoxesRemaining--;
		if (this.showingTutorials)
			StartCoroutine (this.OpenNextTutorialBox ());
		else {
            StartCoroutine(StartLevel());
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
            print(1f / zoomSpeed);
            yield return new WaitForSeconds(1f / zoomSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.5f);
            if(z.superDogTutorial)
            {
                EnableSuperDogTutorial();
            }
        }
    }

    public void CleanUpTutorial()
    {
        this.DisableSuperDogTutorial();
        this.DisableEmptyStaff();
        this.DisableFirstNote();
        
    }

    public IEnumerator StartLevel()
    {
        CleanUpTutorial();
        yield return Zoom(this.tutorialPrefabs[tutorialPrefabs.Length - 1]);
        this.EnableMinions();
        this.EnableNotes();
        this.EnableBackground();
    }

	private bool autoplay = false;

	private void AutoMatch() {
		if (this.showingTutorials) {
            if (this.currentTutorialBox != null)
			    this.CloseTutorialBox ();
			return;
		}

        if (this.hero.minionsCarrying.Count != 0)
        {
            return;
        }

		List<Note> notes = new List<Note>();
		foreach (Note n in this.notes) {
			if (!this.notesAutoclicked.Contains(n))
				notes.Add(n);
		}
		if (notes.Count == 0)
			return;
		int i = Random.Range (0, notes.Count);
		Note note = notes[i];

		List<Minion> toPickUp = new List<Minion> ();
	
		foreach (char letter in note.letters) {	
			List<Minion> minions = new List<Minion>();
			foreach (Minion m in this.minions) {
				if (!this.minionsAutoclicked.Contains(m) 
				    && !toPickUp.Contains(m)
				    && (m.letter == letter))
					minions.Add(m);
			}
			if (minions.Count == 0)
				return;
			
			i = Random.Range (0, minions.Count);
			Minion minion = minions[i];
			toPickUp.Add(minion);
		}

		foreach (Minion m in toPickUp) {
			this.hero.PickUpMinion(m, false);
			this.minionsAutoclicked.Add(m);
		}

		this.notesAutoclicked.Add(note);
		this.hero.TurnInNote(note);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q) && Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.Z)) {
			this.CompleteLevel();
		}

        if(zooming)
        {
            t += zoomSpeed;
            cam.transform.position = Vector3.Lerp(prevPos, targetPos, t);
            cam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(prevSize, targetSize, t);

            if(t >= 1)
            {
                zooming = false;
            }
        }

		if (Input.GetKeyDown (KeyCode.Return)) {
			//this.hero.Caffeinate(5f);
			//this.autoplay = true;
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			//this.autoplay = !this.autoplay;
		} 

		if (Input.GetKeyDown (KeyCode.C)) {
			//this.hero.Caffeinate();
		}

		if (this.autoplay || Input.GetKeyDown (KeyCode.M)) {
			//this.AutoMatch();
		}

        //this.autoplay = this.autoplay || LevelSelection.IsAutoplaying();
	}
}
