using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    public class Free_Exploration_Level_Manager : MonoBehaviour
    {
        private bool _complete = false;

        public IEnumerator PlayFreeExploration(bool first)
        {
            _complete = false;
            gameObject.SetActive(true);
            while (!_complete)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public void FinishFreeExploration()
        {
            gameObject.SetActive(false);
            _complete = true;
        }
    }
}