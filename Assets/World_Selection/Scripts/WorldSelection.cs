using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldSelection : MonoBehaviour {

    [Header("World Image")]
    public Sprite[] worldImages;
    public Image selectionImage;

    private Vector3 wsCanvasPos = new Vector3(-15, 10, -10);

    [Header("Levels")]
    public GameObject[] worlds;
    public Sprite[] worldSprites;
    public Sprite lockedWorldSprite;

    private int worldsUnlocked;

    private Button selectBtn;

    private Camera cam;

    // Use this for independent parameters (don't rely on other scripts)
    private void Awake()
    {
        selectBtn = worlds[0].GetComponentInChildren<Button>();
        selectBtn.onClick.AddListener(delegate { LoadWorld(worldsUnlocked); });

    }

    // Use this for initialization
    void Start () {
        worldsUnlocked = 0;
        DontDestroyOnLoad(this);

        SetUpWorld();
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void SetUpWorld()
    {
        selectionImage.sprite = worldImages[worldsUnlocked];
        worlds[worldsUnlocked].GetComponent<Image>().sprite = worldSprites[worldsUnlocked];
        selectBtn.transform.parent = worlds[worldsUnlocked].transform;
        selectBtn.transform.localPosition = new Vector3(0, -137, 0);
    }

    public void UnlockWorld()
    {
        Debug.Log("unlock");
        if (worldsUnlocked < worlds.Length)
        {
            worldsUnlocked++;
            SetUpWorld();
        }
    }

    public void LoadWorld(int i)
    {
        SceneManager.LoadScene(worlds[i].name);
    }

    public void LoadWorldSelection()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cam.transform.position = wsCanvasPos;
    }
}
