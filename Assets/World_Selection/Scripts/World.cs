using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public WorldSelection ws;
    public int worldNum;

    private Collider col;

	// Use this for initialization
	void Start () {
        col = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        ws.LoadWorld(worldNum);
    }
}
