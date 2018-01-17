using UnityEngine;
using System.Collections;

public class AudioPlayer
{
    private AudioSource Source;

    public AudioPlayer(AudioClip clip, AudioSource source)
    {
        Source = source;
        LoadNewClip(clip);
    }

    public void LoadNewClip(AudioClip clip)
    {
        Source.clip = clip;
        Source.clip.LoadAudioData();
        Source.Play();
        Source.Pause();
    }

    public IEnumerator PlayBlocking(float time)
    {
        Source.time = 0f;
        Source.UnPause();
        yield return new WaitForSeconds(time);
        Source.time = 0f;
        Source.Play();
        Source.Pause();
    }

    public IEnumerator PlayBlocking()
    {
        yield return PlayBlocking(Source.clip.length + 0.2f);
    }
}
