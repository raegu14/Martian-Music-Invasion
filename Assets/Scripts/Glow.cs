using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowInfo
{
    public GameObject note;
    public Vector3 pos;
    public Vector3 bounds;
    public float interval;
    public bool rest;
}

public class Glow : MonoBehaviour {

    public int measureLowerBound;
    public int measureUpperBound;

    public float[] intervals;
    public bool[] rest;
    private int iter;

    private GlowInfo[] info;
    private GameObject measure;
    private GameObject glow;
    private bool glowing;
    private float timer;

    public float speed;

	// Use this for initialization
	void Start () {
        glow = transform.GetChild(0).gameObject;
        SimulateParticles(10, true);
        glow.SetActive(false);
        glowing = false;
        measure = GameObject.Find("Measure");
        GetGlowInfo();        
	}
	
	// Update is called once per frame
	void Update () {
		if(glowing)
        {
            glow.SetActive(true);
            if (Time.time > timer + 1/info[iter].interval * speed)
            {
                //update glow
                iter++;
                if (iter == info.Length)
                {
                    glowing = false;
                    return;
                }
                timer = Time.time;

                if (!info[iter].rest)
                {
                    var notePos = info[iter].pos;
                    notePos.z = -9;
                    notePos.y -= 0.5f;
                    glow.transform.position = notePos;
                    var shapeMod = glow.GetComponent<ParticleSystem>().shape;
                    Vector3 tmpShape = info[iter].bounds * 15;
                    tmpShape.y /= 5;
                    shapeMod.scale = tmpShape;
                    SimulateParticles(0, false);
                }
                else
                {
                    glow.GetComponent<ParticleSystem>().Clear();
                    glow.GetComponent<ParticleSystem>().Stop();
                }
            }
        }
        else
        {
            glow.SetActive(false);
        }
    }

    void GetGlowInfo()
    {
        info = new GlowInfo[measureUpperBound - measureLowerBound];
        for(int i = measureLowerBound; i < measureUpperBound; i++)
        {
            info[i - measureLowerBound] = new GlowInfo();
            info[i - measureLowerBound].note = measure.transform.GetChild(i).gameObject;
            info[i - measureLowerBound].interval = intervals[i - measureLowerBound];
            info[i - measureLowerBound].rest = rest[i - measureLowerBound];

            if (!rest[i - measureLowerBound])
            {
                Vector3 tmpPos = measure.transform.GetChild(i).gameObject.transform.localPosition;
                Vector2 offset = measure.transform.GetChild(i).gameObject.GetComponent<Collider2D>().offset;
                tmpPos.x -= offset.x;
                tmpPos.y -= offset.y;
                tmpPos /= measure.transform.GetChild(i).gameObject.transform.localScale.x;
                info[i - measureLowerBound].pos = tmpPos;

                Vector3 box = measure.transform.GetChild(i).gameObject.GetComponent<Collider2D>().bounds.size;
                box.z = box.y;
                info[i - measureLowerBound].bounds = box;
            }

        }
    }

    void UpdateGlowPos()
    {
        for (int i = 0; i < info.Length; i++)
        {
            info[i].pos = info[i].note.transform.position;   
        }
    }

    public void StartGlow()
    {
        UpdateGlowPos();
        iter = 0;
        var notePos = info[iter].pos;
        notePos.z = -9;
        notePos.y -= 0.5f;
        glow.transform.position = notePos;
        var shapeMod = glow.GetComponent<ParticleSystem>().shape;
        Vector3 tmpShape = info[iter].bounds * 15;
        tmpShape.y /= 5;
        shapeMod.scale = tmpShape;
        SimulateParticles(10, true);
        glowing = true;
        timer = Time.time;
    }

    void SimulateParticles(float seconds, bool emit)
    {
        glow.GetComponent<ParticleSystem>().Clear();
        glow.GetComponent<ParticleSystem>().Simulate(seconds, true, true);
        if (emit)
        {
            glow.GetComponent<ParticleSystem>().Emit(10);
        }
        glow.GetComponent<ParticleSystem>().Play();
    }
}
