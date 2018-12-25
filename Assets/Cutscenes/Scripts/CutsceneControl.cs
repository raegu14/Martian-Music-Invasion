using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneControl : MonoBehaviour {

    public Button PlayButton;
    public GameObject IntButtonGO;
    public GameObject NonButtonGO;
    public GameObject DemoPopup;
    public InputField NameInputField;
    public SessionManager Session;

    public AudioSource Narration;
    public Button NextButton;

    public void OnInputChange()
    {
        if (GameVersion.ValidID(NameInputField.text))
        {
            PlayButton.enabled = true;
            PlayButton.GetComponentInChildren<Text>().text = "Play";
        } else
        {
            PlayButton.enabled = false;
            PlayButton.GetComponentInChildren<Text>().text = "Enter ID Above";
        }
    }

	// Use this for initialization
	void Start () {
        if (PlayButton != null)
        {
            PlayButton.enabled = false;
        }

        if(NextButton && Narration)
        {
            NextButton.interactable = false;
        }
    }

    private void Update()
    {
        if(NextButton)// && !NextButton.interactable && !Narration.isPlaying)
        {
            NextButton.interactable = true;
        }
    }

    public void ChangeScene(string levelName) 
	{
		CutsceneAudio.ChangeScene (levelName);
		SceneManager.LoadScene(levelName);
	}	

    public void BeginIntegrated()
    {
        string name = NameInputField.text.ToLower();
        Logger.Instance.UserID = name;
        GameVersion.T version = GameVersion.T.Integrated;
        Logger.Instance.LogAction("Version", "Integrated", name);
        LevelSelection.SetVersion(version);
        Commands.AutoplayReady = true;
        Session.LoadLevel("IntroCutscene1");
    }

    public void BeginNonIntegrated()
    {
        string name = NameInputField.text.ToLower();
        Logger.Instance.UserID = name;
        GameVersion.T version = GameVersion.T.NotIntegrated;
        Logger.Instance.LogAction("Version", "Non-Integrated", name);
        LevelSelection.SetVersion(version);
        Commands.AutoplayReady = true;
        Session.LoadLevel("IntroCutscene1");
    }

    public void Begin ()
    {
        string name = NameInputField.text.ToLower();
        Logger.Instance.UserID = name;
        GameVersion.T version = GameVersion.GetVersion(name);
        if (version == GameVersion.T.Integrated)
        {
            Logger.Instance.LogAction("Version", "Integrated", name);
        } else
        {
            Logger.Instance.LogAction("Version", "Non-Integrated", name);
        }
        LevelSelection.SetVersion(version);
        Commands.AutoplayReady = true;
        Session.LoadLevel("IntroCutscene1");
    }

    // Function for demo release popup
    // Hides the popup and unhides the play buttons
    public void CloseDemoPopup ()
    {
        DemoPopup.SetActive(false);
        IntButtonGO.SetActive(true);
        NonButtonGO.SetActive(true);
    }
}
