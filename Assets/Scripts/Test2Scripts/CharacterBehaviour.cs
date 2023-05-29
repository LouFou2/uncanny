using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using System.Threading.Tasks;
using UnityEngine.Animations.Rigging;
using JetBrains.Annotations;
//using UnityEditor.PackageManager;
using System.Runtime.CompilerServices;

public class CharacterBehaviour : MonoBehaviour
{
    // Components involved in character Anmation
    public ExpressionControl express;   // ExpressionControl (Scriptable Object) - Controls all variables used to Animate character expressions and movement
    public MoodControl mood;            // Scriptable Object that stores variables to adjust Mood Animations
    public Emote[] Emotes;              // Scriptable Objects that adjusts expression variables to create different expressions - Animated using Tweening
    public Emote currentEmote;
    public Animator animator;
    public MoodPad moodPad;
    public HeadPad headPad;

    // Sine Wave variables
    [HideInInspector] public AnimationCurve _blinkCurve;
    [HideInInspector] public bool _headIsTurning;
    [HideInInspector] public float _headTurnMax;
    [HideInInspector] public float _headTurnMin;
    [HideInInspector] public float _headTurnSpeed;
    [HideInInspector] public AnimationCurve _headTurnCurve;
    private float _headTurnTime;

    [HideInInspector] public bool _headIsNodding;
    [HideInInspector] public float _headNodMax;
    [HideInInspector] public float _headNodMin;
    [HideInInspector] public float _headNodSpeed;
    private float _headNodTime;
    [HideInInspector] public bool _headNodPlusTurn;

    [HideInInspector] public float _headEQTime = 0f;

    [HideInInspector] public float _headTiltMax;
    [HideInInspector] public float _headTiltMin;
    [HideInInspector] public float _headTiltSpeed;
    private float _headTiltTime;

    [HideInInspector] public float _jawOpenMax;
    [HideInInspector] public float _jawOpenMin;
    [HideInInspector] public float _jawOpenSpeed;
    private float _jawOpenTime;

    [HideInInspector] public float _lookUDMax;
    [HideInInspector] public float _lookUDMin;
    [HideInInspector] public float _lookUDSpeed;
    private float _lookUDTime;

    [HideInInspector] public float _lookLRMax;
    [HideInInspector] public float _lookLRMin;
    [HideInInspector] public float _lookLRSpeed;
    private float _lookLRTime;

    [HideInInspector] public float _shoulderSpeed;

    [HideInInspector] public float _shoulder_L_Max;
    [HideInInspector] public float _shoulder_L_Min;    
    private float _shoulder_L_Time;

    [HideInInspector] public float _shoulder_R_Max;
    [HideInInspector] public float _shoulder_R_Min;
    private float _shoulder_R_Time;

    [HideInInspector] public float _lidsMeet_L;
    [HideInInspector] public float _topLid_L_Min;
    private float _topLid_L_Time;

    [HideInInspector] public float _lidsMeet_R;
    [HideInInspector] public float _topLid_R_Min;
    private float _topLid_R_Time;

    [HideInInspector] public float _botLidSpeed;

    [HideInInspector] public float _botLid_L_Max;
    private float _botLid_L_Time;

    [HideInInspector] public float _botLid_R_Max;
    private float _botLid_R_Time;

    [HideInInspector] public float _blinkDuration;
    [HideInInspector] public float _blinkPauseDuration;
    private bool _pauseBlinking = false;

    //ExpressionControl variables
    [HideInInspector] public float _moodOrEmote; // NB: To transition between Mood Animations (idle state) to Emotional Expressions Animations
    
    [HideInInspector] public float _headTurn; // Head Movement keys
    [HideInInspector] public float _headNod;
    [HideInInspector] public float _headTilt;
    [HideInInspector] public float _headLateralX;
    [HideInInspector] public float _headLateralY;

    [HideInInspector] public float _lookLR; // Body and Facial keys
    [HideInInspector] public float _lookUD;
    [HideInInspector] public float _jawOpen;

    [HideInInspector] public float _eyeLidTop_L;
    [HideInInspector] public float _eyeLidTop_R;
    [HideInInspector] public float _eyeLidBot_L;
    [HideInInspector] public float _eyeLidBot_R;
    [HideInInspector] public float _shoulder_L;
    [HideInInspector] public float _shoulder_R;

    [HideInInspector] public float _tongueStretch; // May or may not add tongue
    [HideInInspector] public float _tongueUpDown;

    [HideInInspector] public float _pleasure; // Mood Parameters
    [HideInInspector] public float _arousal;
    [HideInInspector] public float _subDom;

    private void OnEnable()
    {
        express.updated.AddListener(ExpressControlUpdates);
        mood.updated.AddListener(ExpressControlUpdates);
    }
    private void OnDisable()
    {
        express.updated.RemoveListener(ExpressControlUpdates);
        mood.updated.RemoveListener(ExpressControlUpdates);
    }
    
    void Start()
    {
        CheckParams();
    }

    void Update()
    {
        UserInputs();
        AutoMovements();     
        UpdateParams(); //updates ExpressionControl + MoodControl parameters (main data container for all expressions)
        Animate();
    }
    void CheckParams()
    {
        _subDom = moodPad.xValue;
        _pleasure = moodPad.yValue;

        _blinkCurve = express.blinkCurve;

        //Head Movements (Expression Control)
        _headTurn = express.headTurn;
        _headNod = express.headNod;
        _headTilt = express.headTilt;

        //Mood (Idle) or Emote (Emotional expression or Reaction)
        _moodOrEmote = express.moodOrEmote;

        //Sine Wave variables (Expression Control)
        _headIsTurning = express.headIsTurning;
        _headTurnMax = express.headTurnMax;
        _headTurnMin = express.headTurnMin;
        _headTurnSpeed = express.headTurnSpeed;
        //_headTurnTime = express.headTurnTime = 0f;
        _headTurnCurve = express.headTurnCurve;

        _headIsNodding = express.headIsNodding;
        _headNodMax = express.headNodMax;
        _headNodMin = express.headNodMin;
        _headNodSpeed = express.headNodSpeed;
        _headNodTime = express.headNodTime = 0f;
        _headNodPlusTurn = express.headNodPlusTurn;

        _headTiltMax = express.headTiltMax;
        _headTiltMin = express.headTiltMin;
        _headTiltSpeed = express.headTiltSpeed;
        _headTiltTime = express.headTiltTime = 0f;

        _jawOpenMax = express.jawOpenMax;
        _jawOpenMin = express.jawOpenMin;
        _jawOpenSpeed = express.jawOpenSpeed;
        _jawOpenTime = express.jawOpenTime = 0f;

        _lookUDMax = express.lookUDMax;
        _lookUDMin = express.lookUDMin;
        _lookUDSpeed = express.lookUDSpeed;
        _lookUDTime = express.lookUDTime = 0f;

        _lookLRMax = express.lookLRMax;
        _lookLRMin = express.lookLRMin;
        _lookLRSpeed = express.lookLRSpeed;
        _lookLRTime = express.lookLRTime = 0f;

        _shoulderSpeed = express.shoulderSpeed;

        _shoulder_L_Max = express.shoulder_L_Max;
        _shoulder_L_Min = express.shoulder_L_Min;
        _shoulder_L_Time = express.shoulder_L_Time = 0f;

        _shoulder_R_Max = express.shoulder_R_Max;
        _shoulder_R_Min = express.shoulder_R_Min;
        _shoulder_R_Time = express.shoulder_R_Time = 0f;

        _lidsMeet_L = express.lidsMeet_L;
        _topLid_L_Min = express.topLid_L_Min;
        _topLid_L_Time = express.topLid_L_Time = 0f;

        _lidsMeet_R = express.lidsMeet_R;
        _topLid_R_Min = express.topLid_R_Min;
        _topLid_R_Time = express.topLid_R_Time = 0f;

        _botLidSpeed = express.botLidSpeed;

        _botLid_L_Max = express.botLid_L_Max;
        _botLid_L_Time = express.botLid_L_Time = 0f;

        _botLid_R_Max = express.botLid_R_Max;
        _botLid_R_Time = express.botLid_R_Time = 0f;

        _blinkDuration = express.blinkDuration = 0.2f;
        _blinkPauseDuration = express.blinkPauseDuration = 2f;

        // Expression parameters used to form all expressions (Expression Control)
        _shoulder_L = express.shoulder_L;
        _shoulder_R = express.shoulder_R;
        _lookLR = express.lookLR;
        _lookUD = express.lookUD;
        _jawOpen = express.jawOpen;
        _eyeLidTop_L = express.eyeLidTop_L;
        _eyeLidTop_R = express.eyeLidTop_R;
        _eyeLidBot_L = express.eyeLidBot_L;
        _eyeLidBot_R = express.eyeLidBot_R;
        _tongueStretch = express.tongueStretch;
        _tongueUpDown = express.tongueUpDown;

        /*
        //Mood Control parameters (Expression Control)
        //_pleasure = express.pleasure;
        //_arousal = express.arousal;
        //_subDom = express.subDom;
        */
    }
    
    void ExpressControlUpdates() // Can remove this once game is finished (only used for changeing parameters in runtime)
    {
        //Head Movements (Expression Control)
        _headTurn = express.headTurn;
        _headNod = express.headNod;
        _headTilt = express.headTilt;

        //Mood (Idle) or Emote (Emotional expression or Reaction)
        _moodOrEmote = express.moodOrEmote;

        //Sine Wave variables (Expression Control)
        _headIsTurning = express.headIsTurning;
        _headTurnMax = express.headTurnMax;
        _headTurnMin = express.headTurnMin;
        _headTurnSpeed = express.headTurnSpeed;
        //_headTurnTime = express.headTurnTime;

        _headIsNodding = express.headIsNodding;
        _headNodMax = express.headNodMax;
        _headNodMin = express.headNodMin;
        _headNodSpeed = express.headNodSpeed;
        _headNodTime = express.headNodTime;
        _headNodPlusTurn = express.headNodPlusTurn;

        _headTiltMax = express.headTiltMax;
        _headTiltMin = express.headTiltMin;
        _headTiltSpeed = express.headTiltSpeed;
        _headTiltTime = express.headTiltTime;

        _jawOpenMax = express.jawOpenMax;
        _jawOpenMin = express.jawOpenMin;
        _jawOpenSpeed = express.jawOpenSpeed;
        _jawOpenTime = express.jawOpenTime;

        _lookUDMax = express.lookUDMax;
        _lookUDMin = express.lookUDMin;
        _lookUDSpeed = express.lookUDSpeed;
        _lookUDTime = express.lookUDTime;

        _lookLRMax = express.lookLRMax;
        _lookLRMin = express.lookLRMin;
        _lookLRSpeed = express.lookLRSpeed;
        _lookLRTime = express.lookLRTime;

        _shoulderSpeed = express.shoulderSpeed;

        _shoulder_L_Max = express.shoulder_L_Max;
        _shoulder_L_Min = express.shoulder_L_Min;
        _shoulder_L_Time = express.shoulder_L_Time;

        _shoulder_R_Max = express.shoulder_R_Max;
        _shoulder_R_Min = express.shoulder_R_Min;
        _shoulder_R_Time = express.shoulder_R_Time;

        _lidsMeet_L = express.lidsMeet_L;
        _topLid_L_Min = express.topLid_L_Min;
        _topLid_L_Time = express.topLid_L_Time;

        _lidsMeet_R = express.lidsMeet_R;
        _topLid_R_Min = express.topLid_R_Min;
        _topLid_R_Time = express.topLid_R_Time;

        _botLidSpeed = express.botLidSpeed;

        _botLid_L_Max = express.botLid_L_Max;
        _botLid_L_Time = express.botLid_L_Time;

        _botLid_R_Max = express.botLid_R_Max;
        _botLid_R_Time = express.botLid_R_Time;

        _blinkDuration = express.blinkDuration;
        _blinkPauseDuration = express.blinkPauseDuration;

        // Expression parameters used to form all expressions (Expression Control)

        _shoulder_L = express.shoulder_L;
        _shoulder_R = express.shoulder_R;
        _lookLR = express.lookLR;
        _lookUD = express.lookUD;
        _jawOpen = express.jawOpen;
        _eyeLidTop_L = express.eyeLidTop_L;
        _eyeLidTop_R = express.eyeLidTop_R;
        _eyeLidBot_L = express.eyeLidBot_L;
        _eyeLidBot_R = express.eyeLidBot_R;

        _tongueStretch = express.tongueStretch;
        _tongueUpDown = express.tongueUpDown;

        //Mood Control parameters (Expression Control)
        //_pleasure = express.pleasure;
        //_arousal = express.arousal;
        //_subDom = express.subDom;

        //MoodControl
        /*
        _pleasureDefault = mood.pleasureDefault;
        _arousalDefault = mood.arousalDefault;
        _dominanceDefault = mood.dominanceDefault;
        _moodBlinkRate = mood.moodBlinkRate;
        _moodMouthMoves = mood.moodMouthMoves;
        _moodHeadMoves = mood.moodHeadMoves;
        _moodEqualiseSpeed = mood.moodEqualiseSpeed;
        */
    }


    void UserInputs() 
    {
        if (moodPad.moodPadActive == true) 
        {
            _subDom = moodPad.xValue;
            _pleasure = moodPad.yValue;
        }
        
        if (headPad.headPadActive == true)
        {
            _headIsTurning = false;
            _headIsNodding = false;
            if (_headTurn != headPad.xValue || _headNod != headPad.yValue) 
            {
                HeadReturnToButton();
            }
            else 
            {
                _headTurn = headPad.xValue;
                _headNod = headPad.yValue;
                _headEQTime = 0f;
            }            
        }
        else if (headPad.headPadActive == false && !_headIsTurning && !_headIsNodding) // should I add code to make button return?
        {
            HeadReturnToAuto();
        }
    }
    void HeadReturnToButton() 
    {
        float _headAutoPointX = _headTurn;
        float _headAutoPointY = _headNod;
        float _backToButtonDuration = 1f;
        if (_headEQTime < _backToButtonDuration) 
        {
            _headTurn = Mathf.Lerp(_headAutoPointX, headPad.xValue, _headTurnCurve.Evaluate(_headEQTime / _backToButtonDuration));
            _headNod = Mathf.Lerp(_headAutoPointY, headPad.yValue, _headTurnCurve.Evaluate(_headEQTime / _backToButtonDuration));
            _headEQTime += Time.deltaTime * Mathf.Max(_headTurnSpeed, _headNodSpeed);
        }
        else 
        {
            _headTurn = headPad.xValue;
            _headNod = headPad.yValue;
        }        
    }
    void HeadReturnToAuto() 
    {
        _headIsTurning = false;
        _headIsNodding = false;
        float _headReturnDuration = 1f;
        float _headTurnMidPoint = (_headTurnMax + _headTurnMin) / 2; // find midpoint of the head turn range
        float _headNodMidPoint = (_headNodMax + _headNodMin) / 2; // find midpoint of the head nod range
        float _headOutBoundX = headPad.xValue;
        float _headOutBoundY = headPad.yValue;
        if (_headEQTime < _headReturnDuration) 
        {
            _headTurn = Mathf.Lerp(_headOutBoundX, _headTurnMidPoint, _headTurnCurve.Evaluate(_headEQTime / _headReturnDuration));
            _headNod = Mathf.Lerp(_headOutBoundY, _headNodMidPoint, _headTurnCurve.Evaluate(_headEQTime / _headReturnDuration));
            _headEQTime += Time.deltaTime * Mathf.Max(_headTurnSpeed, _headNodSpeed);
            _headTurnTime = 0f;
            _headNodTime = 0f;
        }
        else 
        {
            _headTurn = _headTurnMidPoint;
            _headNod = _headNodMidPoint;            
            _headIsTurning = true;
            _headIsNodding = true;
        }
    }
    //# END HEAD TOUCHPAD FUNCTIONALITY

    void AutoMovements() //***TODO - Add a way to reset time to zero for each sine wave (resets to zero in start method.)
    {
        if (_headTurnSpeed != 0f && _headIsTurning)
        {
            float headTurnValue = Mathf.Sin(_headTurnTime);// *** This is the sine wave code
            float headTurnRange = _headTurnMax - _headTurnMin;
            float headTurnFinalValue = (headTurnValue + 1f) / 2f * headTurnRange + _headTurnMin;
            _headTurn = headTurnFinalValue;
            _headTurnTime += Time.deltaTime * _headTurnSpeed;
            _headEQTime = 0f;
        }

        if (_headNodSpeed != 0f && _headIsNodding)
        {              
            float headNodValue = Mathf.Sin(_headNodTime);
            float headNodRange = _headNodMax - _headNodMin;
            float headNodFinalValue = (headNodValue + 1f) / 2f * headNodRange + _headNodMin;
            _headNod = headNodFinalValue;
            _headNodTime += Time.deltaTime * _headNodSpeed;
            _headEQTime = 0;
        }

        if (_headTiltSpeed != 0f)
        {
            _headTiltTime += Time.deltaTime * _headTiltSpeed;
            float headTiltValue = Mathf.Sin(_headTiltTime);// * _headTurnAmplitude;
            float headTiltRange = _headTiltMax - _headTiltMin;
            float headTiltFinalValue = (headTiltValue + 1f) / 2f * headTiltRange + _headTiltMin;
            _headTilt = headTiltFinalValue;
        }

        if (_jawOpenSpeed != 0f)
        {
            _jawOpenTime += Time.deltaTime * _jawOpenSpeed;
            float jawOpenValue = Mathf.Sin(_jawOpenTime);// * _headTurnAmplitude;
            float jawOpenRange = _jawOpenMax - _jawOpenMin;
            float jawOpenFinalValue = (jawOpenValue + 1f) / 2f * jawOpenRange + _jawOpenMin;
            _jawOpen = jawOpenFinalValue;
        }

        if (_lookLRSpeed != 0f) 
        {
            _lookLRTime += Time.deltaTime * _lookLRSpeed;
            float lookLRValue = Mathf.Sin(_lookLRTime);// * _lookLRAmplitude;
            float lookLRRange = _lookLRMax - _lookLRMin;
            float lookLRFinalValue = (lookLRValue + 1f) / 2f * lookLRRange + _lookLRMin;
            _lookLR = lookLRFinalValue;
        }
        
        if (_lookUDSpeed != 0f)
        {
            _lookUDTime += Time.deltaTime * _lookUDSpeed;
            float lookUDValue = Mathf.Sin(_lookUDTime);// * _lookLRAmplitude;
            float lookUDRange = _lookUDMax - _lookUDMin;
            float lookUDFinalValue = (lookUDValue + 1f) / 2f * lookUDRange + _lookUDMin;
            _lookUD = lookUDFinalValue;
        }

        if (_shoulderSpeed != 0f)
        {
            _shoulder_L_Time += Time.deltaTime * _shoulderSpeed;
            float shoulder_L_Value = Mathf.Sin(_shoulder_L_Time);// * _lookLRAmplitude;
            float shoulder_L_Range = _shoulder_L_Max - _shoulder_L_Min;
            float shoulder_L_FinalValue = (shoulder_L_Value + 1f) / 2f * shoulder_L_Range + _shoulder_L_Min;
            _shoulder_L = shoulder_L_FinalValue;

            _shoulder_R_Time += Time.deltaTime * _shoulderSpeed;
            float shoulder_R_Value = Mathf.Sin(_shoulder_R_Time);// * _lookLRAmplitude;
            float shoulder_R_Range = _shoulder_R_Max - _shoulder_R_Min;
            float shoulder_R_FinalValue = (shoulder_R_Value + 1f) / 2f * shoulder_R_Range + _shoulder_R_Min;
            _shoulder_R = shoulder_R_FinalValue;
        }

        // *** note that eyelids are different from other sine wave movements      
        
        if (_topLid_L_Time < _blinkDuration && !_pauseBlinking)
        {
            _eyeLidTop_L = Mathf.Lerp(_topLid_L_Min, _eyeLidBot_L, _blinkCurve.Evaluate(_topLid_L_Time / _blinkDuration));
            _eyeLidTop_R = Mathf.Lerp(_topLid_R_Min, _eyeLidBot_R, _blinkCurve.Evaluate(_topLid_L_Time / _blinkDuration));
            _topLid_L_Time += Time.deltaTime;
        }

        if (_topLid_L_Time >= _blinkDuration && !_pauseBlinking)
        {
            StartCoroutine(BlinkPause(_blinkPauseDuration));
        }

        if (_botLidSpeed > 0) 
        {
            _botLid_L_Time += Time.deltaTime * _botLidSpeed;
            float botLid_L_Value = Mathf.Sin(_botLid_L_Time);
            float botLid_L_Range = _botLid_L_Max - _lidsMeet_L;
            float botLid_L_FinalValue = (botLid_L_Value + 1f) / 2f * botLid_L_Range + _lidsMeet_L;
            _eyeLidBot_L = botLid_L_FinalValue;

            _botLid_R_Time += Time.deltaTime * _botLidSpeed;
            float botLid_R_Value = Mathf.Sin(_botLid_R_Time);
            float botLid_R_Range = _botLid_R_Max - _lidsMeet_R;
            float botLid_R_FinalValue = (botLid_R_Value + 1f) / 2f * botLid_R_Range + _lidsMeet_R;
            _eyeLidBot_R = botLid_R_FinalValue;
        }        
        // *** note that eyelids are different from other sine wave movements (dont copy for other movements)

    }
    private IEnumerator BlinkPause(float duration)
    {
        _pauseBlinking = true;
        _eyeLidTop_L = _topLid_L_Min;
        _eyeLidTop_R = _topLid_R_Min;

        yield return new WaitForSeconds(duration);

        _topLid_L_Time = 0f;
        _topLid_R_Time = 0f;
        _pauseBlinking = false;
    }
    void UpdateParams() //send updated variable values back to ExpressionControl
    {
        express.headTurn = _headTurn;
        express.headNod = _headNod;
        express.headTilt = _headTilt;

        express.moodOrEmote = _moodOrEmote;

        // *** Sine Waves ***
        express.headIsTurning = _headIsTurning;
        express.headTurnMax = _headTurnMax;
        express.headTurnMin = _headTurnMin;
        express.headTurnSpeed = _headTurnSpeed;
        //express.headTurnTime = _headTurnTime;
       // express.headTurnCurve = _headTurnCurve;

        express.headNodMax = _headNodMax;
        express.headNodMin = _headNodMin;
        express.headNodSpeed = _headNodSpeed;
        express.headNodTime = _headNodTime;

        express.headTiltMax = _headTiltMax;
        express.headTiltMin = _headTiltMin;
        express.headTiltSpeed = _headTiltSpeed;
        express.headTiltTime = _headTiltTime;

        express.jawOpenMax = _jawOpenMax;
        express.jawOpenMin = _jawOpenMin;
        express.jawOpenSpeed = _jawOpenSpeed;
        express.jawOpenTime = _jawOpenTime;

        express.lookUDMax = _lookUDMax;
        express.lookUDMin = _lookUDMin;
        express.lookUDSpeed = _lookUDSpeed;
        express.lookUDTime = _lookUDTime;

        express.lookLRMax = _lookLRMax;
        express.lookLRMin = _lookLRMin;
        express.lookLRSpeed = _lookLRSpeed;
        express.lookLRTime = _lookLRTime;

        express.shoulderSpeed = _shoulderSpeed;

        express.shoulder_L_Max = _shoulder_L_Max;
        express.shoulder_L_Min = _shoulder_L_Min;        
        express.shoulder_L_Time = _shoulder_L_Time;

        express.shoulder_R_Max = _shoulder_R_Max;
        express.shoulder_R_Min = _shoulder_R_Min;
        express.shoulder_R_Time = _shoulder_R_Time;

        // *** vv DONT copy the lids for other things vv
        express.lidsMeet_L = _lidsMeet_L;
        express.topLid_L_Min = _topLid_L_Min;
        express.topLid_L_Time = _topLid_L_Time;

        express.lidsMeet_R = _lidsMeet_R;
        express.topLid_R_Min = _topLid_R_Min;
        express.topLid_R_Time = _topLid_R_Time;

        express.botLidSpeed = _botLidSpeed;

        express.botLid_L_Max = _botLid_L_Max;
        express.botLid_L_Time = _botLid_L_Time;

        express.botLid_R_Max = _botLid_R_Max;
        express.botLid_R_Time = _botLid_R_Time;

        express.blinkDuration = _blinkDuration;
        express.blinkPauseDuration = _blinkPauseDuration;
        // *** ^^ DONT copy the lids for other things ^^

        express.shoulder_L = _shoulder_L;
        express.shoulder_R = _shoulder_R;
        express.lookUD = _lookUD;
        express.lookLR = _lookLR;
        express.jawOpen = _jawOpen;
        express.eyeLidTop_L = _eyeLidTop_L;
        express.eyeLidTop_R = _eyeLidTop_R;
        express.eyeLidBot_L = _eyeLidBot_L;
        express.eyeLidBot_R = _eyeLidBot_R;
        express.tongueStretch = _tongueStretch;
        express.tongueUpDown = _tongueUpDown;
        /*
        express.squint = _squint;
        express.whaleEye = _whaleEye;
        express.browLift = _browLift;
        express.frown = _frown;        
        express.smile = _smile;
        express.lipStretch = _lipStretch;
        express.lipTight = _lipTight;
        express.pout = _pout;
        express.speak = _speak;
        express.lipCnrs = _lipCnrs;
        express.sneer = _sneer;
        */
        express.pleasure = _pleasure;
        express.arousal = _arousal;
        express.subDom = _subDom;
        
    }
    void Animate()
    {
        animator.SetFloat("HeadTurn", _headTurn);
        animator.SetFloat("HeadNod", _headNod);
        animator.SetFloat("HeadTilt", _headTilt);
        animator.SetFloat("MoodOrEmote", _moodOrEmote);
        animator.SetFloat("LookLR", _lookLR);
        animator.SetFloat("LookUD", _lookUD);
        animator.SetFloat("Shoulder_L", _shoulder_L);
        animator.SetFloat("Shoulder_R", _shoulder_R);
        animator.SetFloat("EyeLidTop_L", _eyeLidTop_L);
        animator.SetFloat("EyeLidTop_R", _eyeLidTop_R);
        animator.SetFloat("EyeLidBot_L", _eyeLidBot_L);
        animator.SetFloat("EyeLidBot_R", _eyeLidBot_R);
        animator.SetFloat("TongueStretch", _tongueStretch);
        animator.SetFloat("TongueUpDown", _tongueUpDown);
        animator.SetFloat("JawOpen", _jawOpen);
        /*
        animator.SetFloat("EarFlap_L", _earFlap_L);
        animator.SetFloat("EarFlap_R", _earFlap_R);
        animator.SetFloat("Squint", _squint);
        animator.SetFloat("WhaleEye", _whaleEye);
        animator.SetFloat("BrowLift", _browLift);
        animator.SetFloat("Frown", _frown);
        animator.SetFloat("Smile", _smile);
        animator.SetFloat("LipStretch", _lipStretch);
        animator.SetFloat("LipTight", _lipTight);
        animator.SetFloat("Pout", _pout);
        animator.SetFloat("Speak", _speak);
        animator.SetFloat("LipCnrs", _lipCnrs);
        animator.SetFloat("Sneer", _sneer);
        */
        animator.SetFloat("Pleasure", _pleasure);
        animator.SetFloat("Arousal", _arousal);
        animator.SetFloat("SubDom", _subDom);
        
    }
}
