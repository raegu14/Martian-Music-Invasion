using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    //Handles placing notes onto the staff
    public class Staff : MonoBehaviour
    {
        public Transform _allSpawnedNotes;

        public Transform notePositionsParent;

        public Transform _noteInstantiators;

        [SerializeField]
        private AudioSource _audio;

        [SerializeField]
        private AudioClip[] _audioClips;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float timeScale;

        [SerializeField]
        private Transform _startingX;

        [SerializeField]
        private BoxCollider2D _bufferBetweenNotesX;

        [SerializeField]
        private ParticleSystem _glowParticle;

        [SerializeField]
        private GameObject _maxNumberPopup;

        [SerializeField]
        private Timer _timer;

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

        private void OnDisable()
        {
            ClearStaff();
        }

        private int GetInsertPosition(float x)
        {
            for(int i = 0; i < _notesOnStaff.Count; i++)
            {
                if(_notesOnStaff[i].transform.position.x > x)
                {
                    return i;
                }
            }
            return _notesOnStaff.Count;
        }

        public bool SetNoteOntoStaff(Note note)
        {
            if (_notesOnStaff.Count < 8)
            {
                _notesOnStaff.Remove(note);
                note.SetPitch(GetPitch(note));
                _notesOnStaff.Insert(GetInsertPosition(note.transform.position.x), note);
                ReformatStaff();
                return true;
            }
            else
            {
                _maxNumberPopup.SetActive(true);
                _timer.SetTimerActive(false);
                return false;
            }
        }

        public void UnsetNote(Note note)
        {
            _notesOnStaff.Remove(note);
            ReformatStaff();
        }

        public void StartPlayNotes()
        {
            StartCoroutine(PlayNotes());

            // GBLxAPI
            GBL_Interface.SendStaffPlayed();
        }

        public void ClearStaff()
        {
            foreach (Note note in _notesOnStaff)
            {
                Destroy(note.gameObject);
            }
            _notesOnStaff.Clear();

            // GBLxAPI
            GBL_Interface.SendStaffCleared();
        }  

        public IEnumerator PlayNotes()
        {
            _glowParticle.gameObject.SetActive(true);
            _canvasGroup.interactable = false;
            foreach(Note note in _allSpawnedNotes.GetComponentsInChildren<Note>())
            {
                note.interactable = false;
            }
            foreach(NoteInstantiator ni in _noteInstantiators.GetComponentsInChildren<NoteInstantiator>())
            {
                ni.interactable = false;
            }

            for (int i = 0; i < _notesOnStaff.Count; i++)
            {
                _glowParticle.transform.position = _notesOnStaff[i].transform.position;
                _glowParticle.Clear();
                _glowParticle.Simulate(10f);
                _glowParticle.Play();

                _audio.clip = _audioClips[(int)_notesOnStaff[i].Pitch];
                _audio.Play();
                yield return new WaitForSeconds(_notesOnStaff[i].Length * timeScale);
                _audio.Stop();
            }
            _canvasGroup.interactable = true;
            foreach (Note note in _allSpawnedNotes.GetComponentsInChildren<Note>())
            {
                note.interactable = true;
            }
            foreach (NoteInstantiator ni in _noteInstantiators.GetComponentsInChildren<NoteInstantiator>())
            {
                ni.interactable = true;
            }
            _glowParticle.gameObject.SetActive(false);
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

        private NotePitch GetPitch(Note note)
        {
            Vector3 pos = note.transform.position;
            float minDistance = Mathf.Infinity;
            NotePitch pitch = NotePitch.E4;

            for (int i = 0; i < _notePositions.Count; i++)
            {
                if (Mathf.Abs(note.transform.position.y - _notePositions[i].transform.position.y) < minDistance)
                {
                    minDistance = Mathf.Abs(note.transform.position.y - _notePositions[i].transform.position.y);
                    pitch = _notePositions[i].Pitch;
                    pos.y = _notePositions[i].transform.position.y;
                }
            }

            pos.z = -1.0f;
            note.transform.position = pos;
            return pitch;
        }
    }
}