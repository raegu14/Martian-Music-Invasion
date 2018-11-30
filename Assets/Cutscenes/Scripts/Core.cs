using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour {

	public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
