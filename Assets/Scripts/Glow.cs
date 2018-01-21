using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour {


    public float[] intervals;
    private int iter;

    private GameObject measure;
    private GameObject glow;
    private bool glowing;
    private float timer;

    public float speed;

	// Use this for initialization
	void Start () {
        glow = GameObject.Find("GlowParticle");
        glow.SetActive(false);
        glowing = false;
        measure = GameObject.Find("Measure");
	}
	
	// Update is called once per frame
	void Update () {
		if(glowing)
        {
            glow.SetActive(true);
            if (Time.time > timer + 1/intervals[iter] * speed)
            {
                //update glow
                iter++;
                if (iter == intervals.Length)
                {
                    glowing = false;
                    return;
                }

                var notePos = measure.transform.GetChild(iter + 1).gameObject.transform.position;
                notePos.z = -9;
                notePos.y -= 0.5f;
                print(notePos);
                glow.transform.position = notePos;
                glow.GetComponent<ParticleSystem>().Simulate(1f, true, true);
                glow.GetComponent<ParticleSystem>().Play();

                //glow.transform.GetChild(++iter).gameObject.SetActive(true);
                timer = Time.time;
            }
        }
        else
        {
            glow.SetActive(false);
        }
    }

    public void StartGlow()
    {
        iter = 0;
        var notePos = measure.transform.GetChild(iter + 1).gameObject.transform.position;
        notePos.z = -9;
        glow.transform.position = notePos;
        //glow.transform.position = measure.transform.GetChild(iter + 1).gameObject.transform.position;
        //glow.transform.GetChild(iter).gameObject.SetActive(true);
        glowing = true;
        timer = Time.time;
    }
}
