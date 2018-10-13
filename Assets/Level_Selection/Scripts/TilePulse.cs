using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePulse : MonoBehaviour {

	private Transform trans;

	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startPulse() { 
		StartCoroutine(pulse());
	}

	public void stopPulse() {
		StopCoroutine(pulse());
	}

	public IEnumerator pulse() {
		float speedModifier = 0.25f;
		bool isPulsing = true;
		while(isPulsing) {
			
			trans.localScale = new Vector3(1 + Mathf.PingPong(Time.time*speedModifier, 0.25f), 1 + Mathf.PingPong(Time.time*speedModifier, 0.25f), trans.localScale.z);
			yield return new WaitForEndOfFrame();
		}
	}
}
