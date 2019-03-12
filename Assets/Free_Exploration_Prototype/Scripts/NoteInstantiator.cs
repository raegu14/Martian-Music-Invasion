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
            
            // push z pos up by 1 so OnMouseDown will raycast correctly
            Vector3 pos = transform.position;
            pos.z = -2.0f;
            _instantiatedNote.transform.position = pos;
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
