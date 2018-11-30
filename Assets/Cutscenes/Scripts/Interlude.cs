using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Interlude : MonoBehaviour {

    public GameObject first;
    public GameObject second;

    public void FirstDone()
    {
        Debug.Log("ayy");
        first.SetActive(false);
        first.transform.position += Vector3.up * 1000;
        second.SetActive(true);
    }

    public void SecondDone()
    {
        SceneManager.LoadScene("Level 7");
    }

	// Use this for initialization
	void Start () {
        first.SetActive(true);
        second.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
