using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    //Handles placing notes onto the staff
    public class Staff : MonoBehaviour
    {
        public Transform notePositionsParent;

        [SerializeField]
        private Transform _startingX;

        [SerializeField]
        private BoxCollider2D _bufferBetweenNotesX;

        private List<Note> _notePositions;

        private List<Note> _notesOnStaff;

        private void Awake()
        {
            _notePositions = new List<Note>();
            for (int i = 0; i < notePositionsParent.childCount; i++)
            {
                if (notePositionsParent.GetChild(i).GetComponent<Note>())
                {
                    _notePositions.Add(notePositionsParent.GetChild(i).GetComponent<Note>());
                }
            }

            _notesOnStaff = new List<Note>();
        }

        public bool SetNoteOntoStaff(Note note)
        {
            _notesOnStaff.Remove(note);
            note.SetPitch(GetPitch(note.transform.position.y));
            _notesOnStaff.Insert(0, note);
            ReformatStaff();
            return true;
        }

        private void ReformatStaff()
        {
            float xPos = _startingX.position.x;
            Vector3 tmpPositionHolder = _startingX.position;
            for (int i = 0; i < _notesOnStaff.Count; i++)
            {
                tmpPositionHolder = _notesOnStaff[i].transform.position;
                tmpPositionHolder.x = xPos;
                _notesOnStaff[i].transform.position = tmpPositionHolder;

                xPos += _bufferBetweenNotesX.size.x + _notesOnStaff[i].GetWidth();
            }
        }

        private NotePitch GetPitch(float y)
        {
            float minDistance = Mathf.Infinity;
            NotePitch pitch = NotePitch.E1;

            for (int i = 0; i < _notePositions.Count; i++)
            {
                if (Mathf.Abs(y - _notePositions[i].transform.position.y) < minDistance)
                {
                    minDistance = Mathf.Abs(y - _notePositions[i].transform.position.y);
                    pitch = _notePositions[i].Pitch;
                }
            }

            return pitch;
        }
    }
}