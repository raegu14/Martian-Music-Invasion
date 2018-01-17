using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CutsceneAudio : MonoBehaviour {

	public AudioClip orchestralMusic;
	public AudioClip alienMusic;

	public AudioSource audioSource;

	public static CutsceneAudio singleton;

	// Use this for initialization
	void Start () {
		CutsceneAudio.singleton = this;
		DontDestroyOnLoad (this);
		this.audioSource = this.gameObject.GetComponent<AudioSource> ();
		this.audioSource.clip = orchestralMusic;
		this.audioSource.Play ();
	}

	public static void ChangeScene (string sceneName) {
		CutsceneAudio ca = CutsceneAudio.singleton;
		if (sceneName == "IntroCutscene2") {
			ca.audioSource.clip = ca.alienMusic;
            ca.audioSource.volume = 0.25f;
			ca.audioSource.Play ();
		} else if (sceneName == "LevelSelection") {
			Destroy(ca.gameObject);
		}
	}

    public  void ManualChangeScene (string sceneName)
    {
        ChangeScene(sceneName);
        //Application.LoadLevel(sceneName);
        SceneManager.LoadScene(sceneName);
        /*foreach (SessionManager sm in this.gameObject.transform.root.GetComponentsInChildren<SessionManager>())
        {
            sm.LoadLevel(sceneName);
        }*/
    }
	
	// Update is called once per frame
	void Update () {
        if (LevelSelection.IsAutoplaying())
        {
            
            switch (SceneManager.GetActiveScene().name /* Application.loadedLevelName */)
            {
                case "TitleScene":
                    ManualChangeScene("IntroCutscene1");
                    break;
                case "IntroCutscene1":
                    ManualChangeScene("IntroCutscene2");
                    break;
                case "IntroCutscene2":
                    ManualChangeScene("IntroCutscene3");
                    break;
                case "IntroCutscene3":
                    ManualChangeScene("IntroCutscene4");
                    break;
                case "IntroCutscene4":
                    ManualChangeScene("LetsGoScene");
                    break;
                case "LetsGoScene":
                    ManualChangeScene("LevelSelection");
                    break;
                default:
                    //Debug.Log(SceneManager.GetActiveScene().name /* Application.loadedLevelName */);
                    break;
            }
        }
	}
}
