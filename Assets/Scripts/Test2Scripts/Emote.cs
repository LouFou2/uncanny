using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Emote", menuName = "Scriptable Object/Emote")]
public class Emote : ScriptableObject
{
    public string emoteName;

    [Range(0.0f, 1.0f)] public float shoulder_L_Amount;
    [Range(0.0f, 1.0f)] public float shoulder_R_Amount;
    [Range(0.0f, 1.0f)] public float jawOpenAmount;
    [Range(0.0f, 1.0f)] public float squintAmount;
    [Range(0.0f, 1.0f)] public float whaleEyeAmount;
    [Range(0.0f, 1.0f)] public float browLiftAmount;
    [Range(0.0f, 1.0f)] public float frownAmount;
    [Range(0.0f, 1.0f)] public float eyeLidTop_L_Amount;
    [Range(0.0f, 1.0f)] public float eyeLidTop_R_Amount;
    [Range(0.0f, 1.0f)] public float eyeLidBot_L_Amount;
    [Range(0.0f, 1.0f)] public float eyeLidBot_R_Amount;
    [Range(0.0f, 1.0f)] public float smileAmount;
    [Range(0.0f, 1.0f)] public float lipStretchAmount;
    [Range(0.0f, 1.0f)] public float lipTightAmount;
    [Range(0.0f, 1.0f)] public float poutAmount;
    [Range(0.0f, 1.0f)] public float speakAmount;
    [Range(0.0f, 1.0f)] public float lipCnrsAmount;
    [Range(0.0f, 1.0f)] public float sneerAmount;

    [Range(0.0f, 1.0f)] public float emoteBlinkRate;
    [Range(0.0f, 1.0f)] public float tongueStretchAmount;
    [Range(0.0f, 1.0f)] public float tongueUpDownAmount;

    [Range(-1.0f, 1.0f)] public float pleasureImpact;
    [Range(-1.0f, 1.0f)] public float arousalImpact;
    [Range(-1.0f, 1.0f)] public float dominanceImpact;

    //Other variables to be used in Tweenings:

    [Range(0.0f, 5.0f)] public float attack;
    [HideInInspector] public string easeCurve = "Linear";
    public Ease easeType = Ease.Linear; // Initialize to a default value
    [Range(0.0f, 5.0f)] public float sustain;
    [Range(0.0f, 1.0f)] public float decay;
 
    private void SetEaseType()
    {
        switch (easeCurve)
        {
            case "Linear":
                easeType = Ease.Linear;
                break;
            case "InSine":
                easeType = Ease.InSine;
                break;
            case "OutSine":
                easeType = Ease.OutSine;
                break;
            case "InQuad":
                easeType = Ease.InQuad;
                break;
            case "OutQuad":
                easeType = Ease.OutQuad;
                break;
            case "InOutQuad":
                easeType = Ease.InOutQuad;
                break;
            case "InCubic":
                easeType = Ease.InCubic;
                break;
            case "OutCubic":
                easeType = Ease.OutCubic;
                break;
            case "InOutCubic":
                easeType = Ease.InOutCubic;
                break;
            case "InQuart":
                easeType = Ease.InQuart;
                break;
            case "OutQuart":
                easeType = Ease.OutQuart;
                break;
            case "InOutQuart":
                easeType = Ease.InOutQuart;
                break;
            case "InQuint":
                easeType = Ease.InQuint;
                break;
            case "OutQuint":
                easeType = Ease.OutQuint;
                break;
            case "InOutQuint":
                easeType = Ease.InOutQuint;
                break;
            case "InExpo":
                easeType = Ease.InExpo;
                break;
            case "OutExpo":
                easeType = Ease.OutExpo;
                break;
            case "InOutExpo":
                easeType = Ease.InOutExpo;
                break;
            case "InCirc":
                easeType = Ease.InCirc;
                break;
            case "OutCirc":
                easeType = Ease.OutCirc;
                break;
            case "InOutCirc":
                easeType = Ease.InOutCirc;
                break;
            case "InBack":
                easeType = Ease.InBack;
                break;
            case "OutBack":
                easeType = Ease.OutBack;
                break;
            case "InOutBack":
                easeType = Ease.InOutBack;
                break;
            case "InElastic":
                easeType = Ease.InElastic;
                break;
            case "OutElastic":
                easeType = Ease.OutElastic;
                break;
            case "InOutElastic":
                easeType = Ease.InOutElastic;
                break;
            case "InBounce":
                easeType = Ease.InBounce;
                break;
            case "OutBounce":
                easeType = Ease.OutBounce;
                break;
            case "InOutBounce":
                easeType = Ease.InOutBounce;
                break;
            default:
                easeType = Ease.Linear;
                break;
        }
    }

}


