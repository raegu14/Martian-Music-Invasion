using UnityEngine;
using System.Collections;

public class Superdog : MonoBehaviour {

	[SerializeField]
	private SpriteRenderer helpRenderer;

	private SpriteRenderer sr;

	public static Superdog singleton;

	public void HideSuperdog() {
		this.sr.enabled = false;
	}

	public void ShowSuperdog() {
		this.sr.enabled = true;
	}

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

		this.sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.HideHelp ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void OnMouseDown() {
		this.ShowHelp ();
	}


}
