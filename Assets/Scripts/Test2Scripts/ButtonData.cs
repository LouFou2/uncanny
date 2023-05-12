using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject to define the impact of a button on the character's mood and expression
[CreateAssetMenu(fileName = "ButtonData", menuName = "Scriptable Object/Button Data")]
public class ButtonData : ScriptableObject
{
    public string buttonName;
    public Emote emote; //set the emote to be referenced in ButtonEmote Script
    
    //add other data: button graphic, etc. 

}
