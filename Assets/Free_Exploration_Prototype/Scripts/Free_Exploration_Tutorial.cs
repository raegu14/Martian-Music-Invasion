using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MartianMusicInvasion.FreeExploration {
    public class Free_Exploration_Tutorial : MonoBehaviour {

        [SerializeField]
        private Text _tutorialText;

        [SerializeField]
        private string[] _firstTimeTutorialText;

        [SerializeField]
        private string[] _otherTimeTutorialText;

        private int _tutorialTextIdx;

        private bool _firstTime = true;

        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private Button _lastButton;

        [SerializeField]
        private Button _skipButton;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            InitializeTutorial();
        }

        public void InitializeTutorial()
        {
            if (_firstTime)
            {
                _skipButton.gameObject.SetActive(false);
            }
            else {
                _skipButton.gameObject.SetActive(true);
            }
            _lastButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(true);
            _tutorialTextIdx = 0;
            NextTutorialText();
        }

        public void NextTutorialText()
        {
            if (_firstTime)
            {
                if (_tutorialTextIdx == _firstTimeTutorialText.Length)
                {
                    CloseTutorial();
                }
                else
                {
                    if (_tutorialTextIdx == _firstTimeTutorialText.Length - 1)
                    {
                        _nextButton.gameObject.SetActive(false);
                        _lastButton.gameObject.SetActive(true);
                    }
                    _tutorialText.text = _firstTimeTutorialText[_tutorialTextIdx];
                    _tutorialTextIdx++;
                }
            }
            else
            {
                if (_tutorialTextIdx == _otherTimeTutorialText.Length)
                {
                    CloseTutorial();
                }
                else
                {
                    if (_tutorialTextIdx == _otherTimeTutorialText.Length - 1)
                    {
                        _nextButton.gameObject.SetActive(false);
                        _lastButton.gameObject.SetActive(true);
                    }
                    _tutorialText.text = _otherTimeTutorialText[_tutorialTextIdx];
                    _tutorialTextIdx++;
                }
            }
        }

        public void SkipTutorial()
        {
            CloseTutorial();
        }

        private void CloseTutorial()
        {
            gameObject.SetActive(false);
            _firstTime = false;
        }
    }
}