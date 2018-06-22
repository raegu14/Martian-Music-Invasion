using UnityEngine;
using System.Collections;

public class Superdog : MonoBehaviour {

	private SpriteRenderer helpRenderer;

	public static Superdog singleton;

	public void HideHelp() {
		this.helpRenderer.enabled = false;
	}

	public void ShowHelp() {
		LevelManager.singleton.HelpRequested ();
		this.helpRenderer.enabled = true;
	}

	void Awake () {
		Superdog.singleton = this;
	}

	void Start () {
		this.helpRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer> ();
		this.HideHelp ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void OnMouseDown() {
		this.ShowHelp ();
	}


}
