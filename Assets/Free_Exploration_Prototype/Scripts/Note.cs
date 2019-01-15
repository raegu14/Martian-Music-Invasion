using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartianMusicInvasion.FreeExploration
{
    public class Note : MonoBehaviour
    {
        [SerializeField]
        private Sprite _nearBottom;
        [SerializeField]
        private Sprite _nearTop;

        [SerializeField]
        private float _length;
        public float Length => _length;

        [SerializeField]
        private NotePitch _pitch;
        public NotePitch Pitch => _pitch;

        [SerializeField]
        private Vector3 _center;
        public Vector3 Center => _center;

        public bool interactable;

        public bool Dragging = false;

        private SpriteRenderer _sr;

        private BoxCollider2D _collider;

        private Camera _cam;

        private Staff _staff;

        public void Awake()
        {
            _cam = Camera.main;
            _sr = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        public void Update()
        {
            if(Dragging)
            {
                Vector3 pos = _cam.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                transform.position = pos;
            }
        }

        public void SetNote()
        {
            if (_staff == null || !_staff.SetNoteOntoStaff(this))
            {
                Destroy(gameObject);
            }
        }

        public void SetPitch(NotePitch pitch)
        {
            _pitch = pitch;
            if(_pitch < NotePitch.B5)
            {
                SwapToBottom();
            }
            else
            {
                SwapToTop();
            }
        }

        //Lets player now how long this note should play for
        public float PlayNote()
        {
            //play note for _length
            return _length;
        }

        public void OnMouseDown()
        {
            if (interactable)
            {
                Dragging = true;
                if (_staff)
                {
                    _staff.UnsetNote(this);
                }
            }
        }

        public void OnMouseUp()
        {
            Dragging = false;
            SetNote();
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.GetComponent<Staff>())
            {
                _staff = collider.GetComponent<Staff>();
            }
        }

        public void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.GetComponent<Staff>() == _staff)
            {
                _staff = null;
            }
        }

        public void SwapToBottom()
        {
            _sr.sprite = _nearBottom;
            ResetCollider();
        }

        public void SwapToTop()
        {
            _sr.sprite = _nearTop;
            ResetCollider();
        }

        public float GetWidth()
        {
            return _collider.size.x * transform.localScale.x;
        }

        private void ResetCollider()
        {
            _collider.size = _sr.sprite.bounds.size;
            _collider.offset = _sr.sprite.bounds.center;
        }
    }

    public enum NotePitch
    {
        E4 = 0,
        F4 = 1,
        G4 = 2,
        A5 = 3,
        B5 = 4,
        C5 = 5,
        D5 = 6,
        E5 = 7,
        F5 = 8,
    }
}