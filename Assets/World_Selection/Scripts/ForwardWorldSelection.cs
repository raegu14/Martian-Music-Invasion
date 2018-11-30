using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardWorldSelection : MonoBehaviour {

    public string nextWorld;
    public GameObject levelBackground;
    public GameObject levelSelection;
   // public uint levelOffset;

    private Vector3 wsCanvas = new Vector3(-15, 10, -10);
    private Vector3 lsCanvas = new Vector3(0, 0, -10);

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cam.transform.position = wsCanvas;
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {
        levelBackground.transform.position = new Vector3(0, 0, -1f);
	}

    public void LoadLevelSelection()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cam.transform.position = lsCanvas;
        levelSelection.SetActive(true);
    }
}
