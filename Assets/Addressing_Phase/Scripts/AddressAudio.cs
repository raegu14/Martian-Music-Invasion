using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddressAudio : MonoBehaviour {

    // Individual note AudioClips
    public AudioClip[] noteClips;
    private Dictionary<string, AudioPlayer> noteNameToPlayer;

    public AudioSource finalSource;
    private AudioPlayer finalPlayer;

    public AudioSource zapSource;
    private AudioPlayer zapPlayer;

    private static Dictionary<string, string> addressToNote = new Dictionary<string, string>()
    {
        {"1st Line", "E4" },
        {"1st Space", "F4" },
        {"2nd Line", "G4" },
        {"2nd Space", "A5" },
        {"3rd Line", "B5" },
        {"3rd Space", "C5" },
        {"4th Line", "D5" },
        {"4th Space", "E5" },
        {"5th Line", "F5" }
    };

    // Use this for initialization
    void Start () {
        this.noteNameToPlayer = new Dictionary<string, AudioPlayer>();
        this.finalPlayer = new AudioPlayer(finalSource.clip, finalSource);
        this.zapPlayer = new AudioPlayer(zapSource.clip, zapSource);

        foreach (AudioClip clip in this.noteClips)
        {
            AudioSource src = this.gameObject.AddComponent<AudioSource>();
            this.noteNameToPlayer[clip.name] = new AudioPlayer(clip, src);
        }
    }
	
    public void PlayNote(string address)
    {
        StartCoroutine(this.noteNameToPlayer[addressToNote[address]].PlayBlocking());
    }

    public IEnumerator PlayMeasure()
    {
        yield return finalPlayer.PlayBlocking();
    }

    public IEnumerator Zap()
    {
        yield return zapPlayer.PlayBlocking();
    }
}
