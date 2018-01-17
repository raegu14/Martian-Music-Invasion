using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class TutorialBox : MonoBehaviour {

	public string minionName;
	public string noteName;

	public Minion minion;
	public Note note;

	// Use this for initialization
	void Start () {

	}

	public void Open(List<Minion> minions, List<Note> notes) {
		//Transform parent = this.transform.parent;

		this.minion = null;
		foreach (Minion m in minions) {
			if (m.gameObject.name == this.minionName) {
				this.minion = m;
				break;
			}
		}

		this.note = null;
		foreach (Note n in notes) {
			if (n.gameObject.name == this.noteName) {
				this.note = n;
				break;
			}
		}
	
		if (this.minion != null) {
			this.minion.EnableClicks();
			this.minion.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		}
		if (this.note != null) {
			this.note.EnableClicks();
			this.note.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	public void Close() {
		if (this.minion != null) {
			this.minion.DisableClicks();
		}
		if (this.note != null) {
			this.note.DisableClicks();
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
