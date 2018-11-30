using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    //Gathers notes in the note object and then plays them
    public class NotePlayer : MonoBehaviour
    {
        [SerializeField]
        private Transform _noteParent;

        private Note currNote;

        public void Play()
        {
            StartCoroutine(PlayNotes());
        }

        private IEnumerator PlayNotes()
        {
            for (int i = 0; i < _noteParent.childCount; i++)
            {
                currNote = _noteParent.GetChild(i).GetComponent<Note>();
                if (currNote != null)
                {
                    float length = currNote.PlayNote();
                    yield return new WaitForSeconds(length);
                }
            }
        }
    }
}