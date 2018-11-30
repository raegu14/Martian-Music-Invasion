using UnityEngine;
using System.Collections;

public class OutroCutsceneAudio : MonoBehaviour {

	public AudioSource audioSource;
	
	public static OutroCutsceneAudio singleton;
	
	// Use this for initialization
	void Start () {
		OutroCutsceneAudio.singleton = this;
		DontDestroyOnLoad (this);
		this.audioSource = this.gameObject.GetComponent<AudioSource> ();
		this.audioSource.clip = AudioManagerUtility.StraussOrchestraClip;
		this.audioSource.Play ();
	}
	
	public static void ChangeScene (string sceneName) {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
