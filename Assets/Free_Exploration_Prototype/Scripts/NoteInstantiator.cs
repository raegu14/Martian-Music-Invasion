using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    public class NoteInstantiator : MonoBehaviour
    {
        public bool interactable;

        public GameObject Note;
        private Note _instantiatedNote;

        public Transform cloneNoteParent;

        public void Update()
        {
            if(_instantiatedNote && _instantiatedNote.Dragging)
            {
                _instantiatedNote = null;
            }
        }

        public void InstantiateNote()
        {
            _instantiatedNote = Instantiate(Note, cloneNoteParent, true).GetComponent<Note>();
            _instantiatedNote.transform.position = transform.position;
        }

        public void OnMouseEnter()
        {
            if (interactable)
            {
                if (_instantiatedNote == null)
                {
                    InstantiateNote();
                }
            }
        }
    }
}
