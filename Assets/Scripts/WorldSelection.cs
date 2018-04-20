using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldSelection : MonoBehaviour {

    [Header("World Image")]
    public Sprite[] worldImages;
    public Image selectionImage;


    [Header("Levels")]
    public GameObject[] worlds;
    public Sprite[] worldSprites;
    public Sprite lockedWorldSprite;

    private int worldsUnlocked;

	// Use this for initialization
	void Start () {
        worldsUnlocked = 0;
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void UnlockWorld()
    {
        if (worldsUnlocked < worlds.Length)
        {
            worldsUnlocked++;
            selectionImage.sprite = worldImages[worldsUnlocked];
            worlds[worldsUnlocked].GetComponent<Image>().sprite = worldSprites[worldsUnlocked];
            worlds[worldsUnlocked].GetComponent<Button>().onClick.AddListener(delegate { LoadWorld(worldsUnlocked); });
        }
    }

    public void LoadWorld(int i)
    {
        SceneManager.LoadScene(worlds[i].name);
    }
}
