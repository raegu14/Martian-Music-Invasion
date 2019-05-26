using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class IntEvent : UnityEvent<int> { }

[Serializable]
public class BoolEvent : UnityEvent<bool> { }

[CreateAssetMenu(fileName = "Level_Selection_Util", menuName = "Data/Level_Selection_Util")]
public class Level_Selection_Util : ScriptableObject
{
    public static IntEvent HoverOverLevel = new IntEvent();

    public static IntEvent SelectLevel = new IntEvent();

    public static IntEvent SelectWorld = new IntEvent();

    public static BoolEvent SetButtonInteractivity = new BoolEvent(); 
}
