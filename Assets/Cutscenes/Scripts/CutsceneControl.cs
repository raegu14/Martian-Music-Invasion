using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DIG.GBLXAPI;

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

        // GBLXAPI
        GBL_Interface.SendCutsceneChanged(levelName, !Narration.isPlaying);
	}	

    public void BeginIntegrated()
    {
        string name = NameInputField.text.ToLower();
        //Logger.Instance.UserID = name;
        GameVersion.T version = GameVersion.T.Integrated;
        //Logger.Instance.LogAction("Version", "Integrated", name);
        LevelSelection.SetVersion(version);
        Commands.AutoplayReady = true;
        Session.LoadLevel("IntroCutscene1");

        // GBLXAPI
        if (!GBLXAPI.Instance.IsInit()) {
            GBLXAPI.Instance.init(GBL_Interface.lrsURL, GBL_Interface.lrsUser, GBL_Interface.lrsPassword, GBL_Interface.standardsConfigDefault, GBL_Interface.standardsConfigUser);
        }
        GBL_Interface.userUUID = GBLXAPI.Instance.GenerateActorUUID(name);
        GBLXAPI.Instance.debugStatement = true;
        GBL_Interface.SendGameStarted();
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

        // GBLXAPI
        GBL_Interface.SendCutsceneChanged("IntroCutscene1", true);
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
