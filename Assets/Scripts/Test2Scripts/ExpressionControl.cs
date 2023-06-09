using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;


// Script To Contain all Parameters for Expression Controls
// to be referenced by other scripts

[CreateAssetMenu(fileName = "ExpressionControl", menuName = "Scriptable Object/ExpressionControl")]
public class ExpressionControl : ScriptableObject
{
    [HideInInspector] public UnityEvent updated;

    // *** Transition between Idle and Expressing ***
    
    [Range(0.0f, 1.0f)] public float moodOrEmote;

    // *** HEAD MOVEMENT ***

    public AnimationCurve blinkCurve;

    public bool headIsTurning = true;
    [Range(-1.0f, 1.0f)] public float headTurn;
    [Range(-1.0f, 1.0f)] public float headTurnMax;
    [Range(-1.0f, 1.0f)] public float headTurnMin;
    [Range(0.0f, 20.0f)] public float headTurnSpeed;
    [HideInInspector] public float headTurnTime = 0f;
    public AnimationCurve headTurnCurve;

    public bool headIsNodding = true;
    [Range(-1.0f, 1.0f)] public float headNod;
    [Range(-1.0f, 1.0f)] public float headNodMax;
    [Range(-1.0f, 1.0f)] public float headNodMin;
    [Range(0.0f, 20.0f)] public float headNodSpeed;
    [HideInInspector] public float headNodTime = 0f;
    public bool headNodPlusTurn = false;

    [Range(-1.0f, 1.0f)] public float headTilt;
    [Range(-1.0f, 1.0f)] public float headTiltMax;
    [Range(-1.0f, 1.0f)] public float headTiltMin;
    [Range(0.0f, 20.0f)] public float headTiltSpeed;
    [HideInInspector] public float headTiltTime = 0f;

    [Range(0.0f, 1.0f)] public float jawOpen;
    [Range(0.0f, 1.0f)] public float jawOpenMax;
    [Range(0.0f, 1.0f)] public float jawOpenMin;
    [Range(0.0f, 20.0f)] public float jawOpenSpeed;
    [HideInInspector] public float jawOpenTime = 0f;

    [Range(-1.0f, 1.0f)] public float lookUD; 
    [Range(-1.0f, 1.0f)] public float lookUDMax;
    [Range(-1.0f, 1.0f)] public float lookUDMin;
    [Range(0.0f, 20.0f)] public float lookUDSpeed;
    [HideInInspector] public float lookUDTime = 0f;

    [Range(-1.0f, 1.0f)] public float lookLR;
    [Range(-1.0f, 1.0f)] public float lookLRMax;
    [Range(-1.0f, 1.0f)] public float lookLRMin;
    [Range(0.0f, 20.0f)] public float lookLRSpeed;
    [HideInInspector] public float lookLRTime = 0f;

    [Range(0.0f, 20.0f)] public float shoulderSpeed;

    [Range(0.0f, 1.0f)] public float shoulder_L;
    [Range(0.0f, 1.0f)] public float shoulder_L_Max;
    [Range(0.0f, 1.0f)] public float shoulder_L_Min;
    [HideInInspector] public float shoulder_L_Time = 0f;

    [Range(0.0f, 1.0f)] public float shoulder_R;
    [Range(0.0f, 1.0f)] public float shoulder_R_Max;
    [Range(0.0f, 1.0f)] public float shoulder_R_Min;
    [HideInInspector] public float shoulder_R_Time = 0f;

    // *** BLINKING *** [this works different from everything else]
    [Range(0.0f, 1.0f)] public float eyeLidTop_L;
    [Range(0.0f, 1.0f)] public float eyeLidTop_R;
    [Range(0.0f, 1.0f)] public float topLid_L_Min;
    [Range(0.0f, 1.0f)] public float topLid_R_Min;
     
    [HideInInspector] public float topLid_L_Time = 0f;    
    [HideInInspector] public float topLid_R_Time = 0f;

    [Range(0.0f, 10f)] public float blinkDuration;
    [Range(0.0f, 10f)] public float blinkPauseDuration;

    [Range(0.0f, 1.0f)] public float eyeLidBot_L;
    [Range(0.0f, 1.0f)] public float eyeLidBot_R;
    [Range(0.0f, 1.0f)] public float lidsMeet_L; //this is the min for bottom lids (*NB* min is actually highest position of lids, max is lowest)
    [Range(0.0f, 1.0f)] public float lidsMeet_R;
    [Range(0.0f, 1.0f)] public float botLid_L_Max;
    [Range(0.0f, 1.0f)] public float botLid_R_Max;
    [HideInInspector] public float botLid_L_Time = 0f;
    [HideInInspector] public float botLid_R_Time = 0f;
    [Range(0.0f, 20.0f)] public float botLidSpeed;

    // *** FACIAL ARTICULATION ***
    /*
    [HideInInspector][Range(0.0f, 1.0f)] public float squint;
    [HideInInspector][Range(0.0f, 1.0f)] public float browLift;
    [HideInInspector][Range(0.0f, 1.0f)] public float frown;    
    [HideInInspector][Range(0.0f, 1.0f)] public float smile;
    [HideInInspector][Range(0.0f, 1.0f)] public float lipStretch;
    [HideInInspector][Range(0.0f, 1.0f)] public float lipTight;
    [HideInInspector][Range(0.0f, 1.0f)] public float pout;
    [HideInInspector][Range(0.0f, 1.0f)] public float speak;
    [HideInInspector][Range(0.0f, 1.0f)] public float lipCnrs;
    [HideInInspector][Range(0.0f, 1.0f)] public float sneer;
    */
    [Range(0.0f, 1.0f)] public float tongueStretch;
    [Range(0.0f, 1.0f)] public float tongueUpDown;
    

    // *** MOOD PARAMETERS ***

    [Range(-1.0f, 1.0f)] public float pleasure;
    [Range(-1.0f, 1.0f)] public float arousal;
    [Range(-1.0f, 1.0f)] public float subDom;

    private void OnEnable()
    {
        // called when the instance is setup

        if (updated == null)
            updated = new UnityEvent();
    }

    private void OnValidate()
    {
        // called when any value is changed
        // in the inspector

        updated.Invoke();
    }
}
