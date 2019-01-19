using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MartianMusicInvasion.FreeExploration
{
    public class Timer : MonoBehaviour
    {
                [SerializeField]
        private Free_Exploration_Level_Manager _manager;

        [SerializeField]
        private GameObject _tutorialObj;

        [SerializeField]
        private GameObject _quitPopup;

        [SerializeField]
        private float _startTime;

        private bool countTimer;

        private float _remainingTime;

        private void OnEnable()
        {
            _tutorialObj.SetActive(true);
            _remainingTime = _startTime;
            countTimer = false;
        }

        public void StartTimer()
        {
            _remainingTime = _startTime;
            countTimer = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (countTimer)
            {
                if (_remainingTime > 0)
                {
                    _remainingTime -= Time.deltaTime;
                }
                else
                {
                    QuitPopup();
                }
            }
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
            _manager.FinishFreeExploration();
        }
    }
}