using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    [SerializeField]
    private Gameplay_Progress _progress;

    [SerializeField]
    private GameObject _incorrectPassword;

    public void SubmitPassword()
    {
        StartCoroutine(SubmitPasswordAsync());
    }

    private IEnumerator SubmitPasswordAsync()
    {
        yield return new WaitForEndOfFrame();
        if(_progress.EnteredCorrectPassword)
        {
            _incorrectPassword.SetActive(false);
        }
        else
        {
            _incorrectPassword.SetActive(true);
        }
    }
}
