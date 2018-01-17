using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneControl : MonoBehaviour {

    public Button PlayButton;
    public InputField NameInputField;
    public SessionManager Session;

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
}
