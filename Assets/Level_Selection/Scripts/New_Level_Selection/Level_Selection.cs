using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Selection : MonoBehaviour {

    [SerializeField]
    private Transform _levelButtonParent = null;

    [SerializeField]
    private GameObject _levelButtonPrefab = null;

    [SerializeField]
    private Level_Info[] _levelInfoList = null;

    [SerializeField]
    private Gameplay_Progress _progress = null;

    [SerializeField]
    private AudioSource _audioSource = null;

    [SerializeField]
    private AudioClip _lockedAudio = null;

    //instantiate level prefabs
    public void InstantiateLevelSelection()
    {
        foreach(Level_Info level in _levelInfoList)
        {
            level.SetUnlocked(_progress.LevelsUnlocked);
            level.SetAvailable(_progress.LevelAvailable);
        }

        for(int i = 0; i < _levelInfoList.Length; i++)
        {
            GameObject obj = Instantiate(_levelButtonPrefab, _levelButtonParent);
            obj.GetComponent<Level_Button>().Display(_levelInfoList[i]);
        }

        StartCoroutine(PlayAudio(0));
    }

    public IEnumerator PlayAudio(int clipNum)
    {
        if (clipNum < _levelInfoList.Length)
        {
            if (_levelInfoList[clipNum].Unlocked)
            {
                _audioSource.clip = _levelInfoList[clipNum].Audio;
            }
            else
            {
                _audioSource.clip = _lockedAudio;
            }
            _audioSource.Play();
            yield return new WaitWhile(() => _audioSource.isPlaying);

            StartCoroutine(PlayAudio(clipNum + 1));
        }
    }
}