using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{

    public GameObject ResetCanvas;
    public Button button;

    public void Awake()
    {
        DontDestroyOnLoad(ResetCanvas);
    }

    public void LoadMainScene()
    {
        Object[] allGameObjects = FindObjectsOfType(typeof(GameObject));
        foreach (Object obj in allGameObjects)
        {
            if (obj != ResetCanvas)
            {
                Destroy(obj);
            }
        }
        SceneManager.LoadSceneAsync("TitleScene");
        Destroy(ResetCanvas);
    }
}
