using UnityEngine;
using System.Collections;

public class LevelListItem : MonoBehaviour {

    public string LevelSceneName;
    public Texture2D comic;
    public Texture2D measure;

    public Vector2 comicPivot = new Vector2(0.5f, 0.5f);
    public float comicPixelsPerUnit = 100f;

    public Vector2 measurePivot = new Vector2(0.5f, 0.5f);
    public float measurePixelsPerUnit = 100f;

    public AudioClip clip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
