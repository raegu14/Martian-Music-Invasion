using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour {

    public GameObject lineHint;
    public GameObject spaceHint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowHint(AddressingStep step)
    {
        if (step.Notes[step.CorrectIndex].Contains("Space"))
        {
            spaceHint.SetActive(true);
        }
        else if (step.Notes[step.CorrectIndex].Contains("Line"))
        {
            lineHint.SetActive(true);
        }
        else
        {
            Debug.Log("neither space nor line tf");
        }
    }

    public void HideHint()
    {
        lineHint.SetActive(false);
        spaceHint.SetActive(false);
    }
}
