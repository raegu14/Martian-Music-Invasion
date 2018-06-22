using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Commands : MonoBehaviour {

    public static bool AutoplayReady = false;
    string press;

	// Use this for initialization
	void Start () {
        AutoplayReady = false;
        DontDestroyOnLoad(this);
        press = "";
    }

	// Update is called once per frame
	void Update () {

        string oldPress = press;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            press = "";
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            press += "a";
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            press += "c";
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            press += "d";
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            press += "e";
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            press += "i";
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            press += "l";
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            press += "n";
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            press += "o";
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            press += "p";
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            press += "s";
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            press += "t";
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            press += "u";
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            press += "y";
        }

        if (press != oldPress)
        {
            Debug.Log(press);
        }

        string message;

        switch (press)
        {
        case "auto":
                if (AutoplayReady)
                {
                    message = "Autoplay Beginning at " + System.DateTime.Now.ToUniversalTime();
                    Logger.Instance.LogAction("Command", message, "");
                    Debug.Log(message);
                    LevelSelection.BeginAutoplay();
                    press = "";
                }
                break;
        case "stop":
        message = "Autoplay Ending at " + System.DateTime.Now.ToUniversalTime();
        Logger.Instance.LogAction("Command", message, "");
        LevelSelection.EndAutoplay();
        Debug.Log(message);
                press = "";
                break;
        case "sel":
                if (LevelSelection.Instance != null)
                {
                    Destroy(LevelSelection.Instance);
                }
        SceneManager.LoadScene("LevelSelection");
                press = "";
                break;
        case "inc":
        Logger.Instance.LogAction("Session", "Level Incremented by Researcher", "");
        LevelSelection.UpOneLevel();
                press = "";
                break;
        case "dec":
        Logger.Instance.LogAction("Session", "Level Decremented by Researcher", "");
        LevelSelection.DownOneLevel();
                press = "";
                break;
        case "load":
        Logger.Instance.LogAction("Session", "Scene reloaded by researcher", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                press = "";
                break;
        case "done":
                Logger.Instance.LogAction("Session", "Terminated by researcher", "");
                HttpWriter.Flush();
                Logger.Instance.SessionEnd();
                Logger.Instance.SessionStart();
                SceneManager.LoadScene("OutroCutscene3");
                press = "";
                break;
        case "play":
        Logger.Instance.LogAction("Session", "Play level triggered by researcher", "");
        LevelSelection.Instance.PlayNextLevel();
                press = "";
                break;
        default:
        break;
        }
    }
}
