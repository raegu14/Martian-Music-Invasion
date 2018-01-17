using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelSelectionGrid : MonoBehaviour {

	// Setting the boundaries of the three stages
	private int stageOneEnd = 6;
	private int stageTwoEnd = 12;
	private int stageThreeEnd = GameManager.numOfLevels;

	// unlockTiles is set equal to the version of the game
	private GameObject[] unlockTiles;
	private GameObject playLevelButton;
	private List<int> tutorialLevels = new List<int> {1, 4, 7, 10, 13, 16};

	// The two arrays of unlock tiles dependent on version
	public GameObject[] musicUnlockTiles;
	public GameObject[] comicUnlockTiles;
	
	// The two play buttons dependent on verison
	public GameObject musicPlayButton;
	public GameObject comicPlayButton;

	// Audio
	private AudioSource audioSource;
	public AudioClip[] songClips;
    public AudioClip[] stageClips;
	// Set this to either the individual comic tiles or the song measure tiles to follow along
	private GameObject[] audioBackgroundPopUpTiles;
	private GameObject[] audioLockPopUpTiles;
	private List<int> audioFullLevels = new List<int> {1, 7, 13, 19};

	// First time on Level Selection Page dialogue box items
	public GameObject outlinedBox;
	public GameObject startPlayingButton;
	public GameObject DialogueText;

	// Array of individual comic tiles or individual measure song tiles for audio following along
	public GameObject[] songMeasureTiles;
    public GameObject[] comicTiles;

	public static LevelSelectionGrid singleton;

	protected void Awake () {
		LevelSelectionGrid.singleton = this;
	}


	// Use this for initialization
	void Start () {
		VersionSetup();
		RemoveUnlockedLevelTiles ();
		DisableTilesFor ("tutorial");

		// Dialogue box set up for first time on level selection page
		if (GameManager.currentLevel != 1) {
			DisableDialogueBoxItems();
		}

		// Move the play button when the player returns to the level select screen after completing a level
		if (GameManager.currentLevel != GameManager.numOfLevels + 1) {
			playLevelButton.transform.position = unlockTiles [GameManager.currentLevel - 1].transform.position;
		} else if (GameManager.currentLevel == GameManager.numOfLevels + 1) {
			playLevelButton.gameObject.SetActive(false);
		}

		// set up audio files
		this.audioSource = this.GetComponent<AudioSource> ();

		// For each return to the level selection page, play the unlocked song
		StartCoroutine(playUnlockedSongAudio() );
		// As the song plays, the comics or the measures enlarge along with the music
		StartCoroutine (followAlongWithTiles ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void VersionSetup() {
		// set unlockTiles array equal to the right array set
		if (GameManager.integratedVersion == true) {
			Logger.Instance.LogAction ("Version", "Set", "Integrated");
			// disable the tiles from other version
			unlockTiles = musicUnlockTiles;
			DisableTilesFor("comicLocks");
			// disable comic play button
			playLevelButton = musicPlayButton;

			// set popping up tiles array equal to the songMeasureTiles array
			audioBackgroundPopUpTiles = songMeasureTiles;
			audioLockPopUpTiles = musicUnlockTiles;

			comicPlayButton.SetActive (false);
		} else {
			Logger.Instance.LogAction ("Version", "Set", "Non-Integrated");
			// disable the tiles from other version
			unlockTiles = comicUnlockTiles;
			DisableTilesFor("musicLocks");
			// disable music play button
			playLevelButton = comicPlayButton;

            audioBackgroundPopUpTiles = comicTiles;
            audioLockPopUpTiles = comicUnlockTiles;
		
			musicPlayButton.SetActive(false);
		}

	}

	private void DisableTilesFor(string version) {
		if (version == "comicLocks") {
			// disable all of the music lock tiles 
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				comicUnlockTiles [i].GetComponent<SpriteRenderer> ().enabled = false;
			}
		} else if (version == "musicLocks") {
			// disable all of the comic lock tiles
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				musicUnlockTiles [i].GetComponent<SpriteRenderer> ().enabled = false;
			}
		} else if (version == "tutorial") {
			for (int i = 0; i < tutorialLevels.Count; i++) {
				unlockTiles[tutorialLevels[i]-1].GetComponent<SpriteRenderer> ().enabled = false;
			}
		}
	}

	// Function to disable all of the unlocked levels
	private void RemoveUnlockedLevelTiles() {
		for (int i = 0; i < GameManager.currentLevel-1; i++) {
			unlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	// function to load the level play 
	public void PlayLevel() {
		playLevelButton.transform.Translate (playLevelButton.transform.position.x + 150, playLevelButton.transform.position.y, playLevelButton.transform.position.z);
        SceneManager.LoadScene("Level" + GameManager.currentLevel);
	}

	// plays the unlocked song according to the unlocked levels
	private IEnumerator playUnlockedSongAudio () {
        AudioClip levelClip;
        if (GameManager.integratedVersion) {
            levelClip = songClips[GameManager.currentLevel - 1];
        } else {
            if (audioFullLevels.Contains(GameManager.currentLevel))
                // Play the entire song
                levelClip = songClips[18];
            else if (GameManager.currentLevel < 7)
                // Play the first stage
                levelClip = stageClips[0];
            else if (GameManager.currentLevel < 13)
                // Play the second stage
                levelClip = stageClips[1];
            else
                // Play the third stage
                levelClip = stageClips[2];

        }
		this.audioSource.clip = levelClip;
		this.audioSource.Play ();
		yield return new WaitForSeconds(levelClip.length);

		// When the all levels have been unlocked, transition to outro cutscenes
		if (GameManager.currentLevel == GameManager.numOfLevels + 1) {
            SceneManager.LoadScene("OutroCutscene1");
		}
	}

	// Disables the dialogue box items after button clicked or on all levels beyond level 1
	public void DisableDialogueBoxItems() {
		outlinedBox.GetComponent<SpriteRenderer> ().enabled = false;
		DialogueText.gameObject.SetActive(false);
		startPlayingButton.gameObject.SetActive(false);
	}

	// Follows along with the song audio with either comic tiles or measure tiles
	private IEnumerator followAlongWithTiles() {
		int startIndex = 0;
		int endIndex = 0;

		// play the full audio at the end of each stage for levels 1, 7, 13, 18 
		if (audioFullLevels.Contains (GameManager.currentLevel)) {
			startIndex = 0;
			endIndex = audioBackgroundPopUpTiles.Length;
		} else if (GameManager.currentLevel > 0 && GameManager.currentLevel <= stageOneEnd) {
			// if current level is only in stage one, only pop out stage one tiles
			startIndex = 0;
			endIndex = stageOneEnd;
		} else if (GameManager.currentLevel > stageOneEnd && GameManager.currentLevel <= stageTwoEnd) {
			// pop out stage two tiles only
			startIndex = stageOneEnd;
			endIndex = stageTwoEnd;
		} else {
			// pop out stage three tiles
			startIndex = stageTwoEnd;
			endIndex = stageThreeEnd;
		}

		for (int i = startIndex; i < endIndex; i++) {
			popOut(audioBackgroundPopUpTiles[i]);
			popOut (audioLockPopUpTiles[i]);
			
			float numOfSeconds = popOutLengthInSeconds(i);
			yield return new WaitForSeconds(numOfSeconds);
			
			popIn(audioBackgroundPopUpTiles[i]);
			popIn (audioLockPopUpTiles[i]);
		}
		
	}
	
	private void popOut(GameObject tile) {
		tile.transform.localScale *= 1.2f;
		tile.transform.position -= Vector3.forward * 2;

	}

	private void popIn(GameObject tile) {
		tile.transform.localScale *= 0.83f;
		tile.transform.position -= Vector3.back * 2;
	}

	private float popOutLengthInSeconds(int i) {
		if (tutorialLevels.Contains(i+1)) {
			// tutorial levels always have no zaps
			return 2.0f;
		} else if (i+1 < GameManager.currentLevel || !GameManager.integratedVersion) {
			// unlocked tiles have longer audio than the zaps
			return 2.0f;
		} else {
			// zaps are for locked tiles and are shorter
			return 1.1f;
		}
	}

}







