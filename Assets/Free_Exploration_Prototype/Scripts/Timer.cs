using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    [SerializeField]
    private GameObject _quitPopup;

    [SerializeField]
    private Text _timerText;

    [SerializeField]
    private float _startTime;

    private bool countTimer;

    private float _remainingTime;

    private void OnEnable()
    {
        _remainingTime = _startTime;
        countTimer = true;
    }

    // Update is called once per frame
    private void Update () {
        if (countTimer) {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                UpdateTextDisplay();
            }
            else
            {
                QuitPopup();
            }
        }
	}

    private void UpdateTextDisplay()
    {
        _timerText.text = "Time Remaining: " + (int)_remainingTime / 60 + ":" + ((int)_remainingTime % 60 < 10 ? "0" : "") + (int)_remainingTime % 60;
            }

    public void SetTimerActive(bool on)
    {
        countTimer = on;
    }

    public void QuitPopup()
    {
        _quitPopup.SetActive(true);
    }

    public void Quit()
    {
        //closes the session
    }
}
