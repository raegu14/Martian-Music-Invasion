using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour {

    public Sprite[] dialogueSprites;
    public AudioClip[] dialogueClips;
    public int tutorialLength;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private bool canPlayAudio = false;

	// Use this for initialization
	void Awake ()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void updateDialogue(int boxNum)
    {
        this.setDialogueBox(boxNum);
        this.playDialogueAudio(boxNum);
    }

    public void setDialogueBox(int boxNum)
    {
        Debug.Log(dialogueSprites[boxNum]);
        spriteRenderer.sprite = dialogueSprites[boxNum];
    }

    public void playDialogueAudio(int boxNum)
    {
        audioSource.clip = dialogueClips[boxNum];

        canPlayAudio = true;
        if (canPlayAudio)
        {
            canPlayAudio = false;
            audioSource.Play();
        }
    }
}
