using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour {

    public Sprite[] dialogueSprites;

    private SpriteRenderer sr;

	// Use this for initialization
	void Start ()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void setDialogueBox(int boxNum)
    {
        sr.sprite = dialogueSprites[boxNum];
    }
}
