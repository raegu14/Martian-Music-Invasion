using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour {

    public GameObject[] glows;
    public float[] intervals;
    private int iter;

    private bool glowing;
    private float timer;

    public float speed;

	// Use this for initialization
	void Start () {
        glowing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(glowing)
        {
            if (Time.time > timer + 1/intervals[iter] * speed)
            {
                //update glow
                glows[iter].SetActive(false);
                if(iter == glows.Length - 1)
                {
                    glowing = false;
                    return;
                }
                glows[++iter].SetActive(true);
                timer = Time.time;
            }
        }
	}

    public void StartGlow()
    {
        iter = 0;
        glows[iter].SetActive(true);
        glowing = true;
        timer = Time.time;
    }
}
