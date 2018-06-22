using UnityEngine;
using System.Collections;

public class AddressingStep : MonoBehaviour
{

    public string StartNote;
    public CircleCollider2D FirstNoteCollider;

    public string[] Notes;
    public int CorrectIndex;
    public BoxCollider2D NotesBox;

    public GameObject TutorialObject = null;
    public GameObject TutorialAlpha = null;
    public string TutorialSuperdogText = null;

}
