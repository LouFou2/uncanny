using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using System.Threading.Tasks;
using UnityEngine.Animations.Rigging;
using JetBrains.Annotations;
using UnityEditor.PackageManager;
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
    [HideInInspector] public AnimationCurve _testAnimCurve;
    [HideInInspector] public bool _headIsTurning;
    [HideInInspector] public float _headTurnMax;
    [HideInInspector] public float _headTurnMin;
    //private float _headTurnTarget = 1f;
    [HideInInspector] public float _headTurnSpeed;
    [HideInInspector] public AnimationCurve _headTurnCurve;
    private float _headTurnTime;

    [HideInInspector] public bool _headIsNodding;
    [HideInInspector] public float _headNodMax;
    [HideInInspector] public float _headNodMin;
    [HideInInspector] public float _headNodSpeed;
    private float _headNodTime;
    [HideInInspector] public bool _headNodPlusTurn;

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
    [HideInInspector] public float _squint;
    [HideInInspector] public float _whaleEye;
    [HideInInspector] public float _browLift;
    [HideInInspector] public float _frown;
    [HideInInspector] public float _eyeLidTop_L;
    [HideInInspector] public float _eyeLidTop_R;
    [HideInInspector] public float _eyeLidBot_L;
    [HideInInspector] public float _eyeLidBot_R;
    [HideInInspector] public float _smile;
    [HideInInspector] public float _lipStretch;
    [HideInInspector] public float _lipTight;
    [HideInInspector] public float _pout;
    [HideInInspector] public float _speak;
    [HideInInspector] public float _lipCnrs;
    [HideInInspector] public float _sneer;
    [HideInInspector] public float _tongueStretch;
    [HideInInspector] public float _tongueUpDown;
    [HideInInspector] public float _shoulder_L;
    [HideInInspector] public float _shoulder_R;
    [HideInInspector] public float _earFlap_L;
    [HideInInspector] public float _earFlap_R;

    [HideInInspector] public float _pleasure; // Mood Parameters
    [HideInInspector] public float _arousal;
    [HideInInspector] public float _subDom;
    
    //MoodControl variables
    [HideInInspector] public float _pleasureChange;     // adjust pleasure parameter
    [HideInInspector] public float _pleasureDefault;   // mood parameters slowly return to default. Can be changed
    [HideInInspector] public float _arousalChange;      // adjust arousal parameter
    [HideInInspector] public float _arousalDefault;
    [HideInInspector] public float _dominanceChange;    // adjust dominance parameter
    [HideInInspector] public float _dominanceDefault;
    [HideInInspector] public float _moodBlinkRate;     // used for calculating blink rates for different moods
    [HideInInspector] public float _moodMouthMoves;    // to change mouth movement variations in idle
    [HideInInspector] public float _moodHeadMoves;     // to change head movement variations in idle
    [HideInInspector] public float _moodEqualiseSpeed;

    //Emote, to be used in Tweening
    [HideInInspector] public float _shoulder_L_Amount;
    [HideInInspector] public float _shoulder_R_Amount;
    [HideInInspector] public float _jawOpenAmount;
    [HideInInspector] public float _squintAmount;
    [HideInInspector] public float _whaleEyeAmount;
    [HideInInspector] public float _browLiftAmount;
    [HideInInspector] public float _frownAmount;
    [HideInInspector] public float _eyeLidTop_L_Amount;
    [HideInInspector] public float _eyeLidTop_R_Amount;
    [HideInInspector] public float _eyeLidBot_L_Amount;
    [HideInInspector] public float _eyeLidBot_R_Amount;
    [HideInInspector] public float _smileAmount;
    [HideInInspector] public float _lipStretchAmount;
    [HideInInspector] public float _lipTightAmount;
    [HideInInspector] public float _poutAmount;
    [HideInInspector] public float _speakAmount;
    [HideInInspector] public float _lipCnrsAmount;
    [HideInInspector] public float _sneerAmount;

    [HideInInspector] public float _emoteBlinkRate;
    [HideInInspector] public float _tongueStretchAmount;
    [HideInInspector] public float _tongueUpDownAmount;
 
    [HideInInspector] public float _pleasureImpact;
    [HideInInspector] public float _arousalImpact;
    [HideInInspector] public float _dominanceImpact;

    [HideInInspector] public float _attack;
    [HideInInspector] public Ease _easeType;
    [HideInInspector] public float _sustain;
    [HideInInspector] public float _decay;

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
        //ControlMood();        
        UpdateParams(); //updates ExpressionControl + MoodControl parameters (main data container for all expressions)
        Animate();
    }
    void CheckParams()
    {
        _pleasure = moodPad.xValue;
        _arousal = moodPad.yValue;

        _testAnimCurve = express.testAnimCurve;

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
        _squint = express.squint;
        _whaleEye = express.whaleEye;
        _browLift = express.browLift;
        _frown = express.frown;
        _eyeLidTop_L = express.eyeLidTop_L;
        _eyeLidTop_R = express.eyeLidTop_R;
        _eyeLidBot_L = express.eyeLidBot_L;
        _eyeLidBot_R = express.eyeLidBot_R;
        _smile = express.smile;
        _lipStretch = express.lipStretch;
        _lipTight = express.lipTight;
        _pout = express.pout;
        _speak = express.speak;
        _lipCnrs = express.lipCnrs;
        _sneer = express.sneer;
        _tongueStretch = express.tongueStretch;
        _tongueUpDown = express.tongueUpDown;

        //Mood Control parameters (Expression Control)
        //_pleasure = express.pleasure;
        //_arousal = express.arousal;
        _subDom = express.subDom;

        //MoodControl
        _pleasureDefault = mood.pleasureDefault;
        _arousalDefault = mood.arousalDefault;
        _dominanceDefault = mood.dominanceDefault;
        _moodBlinkRate = mood.moodBlinkRate;
        _moodMouthMoves = mood.moodMouthMoves;
        _moodHeadMoves = mood.moodHeadMoves;
        _moodEqualiseSpeed = mood.moodEqualiseSpeed;
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
        _squint = express.squint;
        _whaleEye = express.whaleEye;
        _browLift = express.browLift;
        _frown = express.frown;
        _eyeLidTop_L = express.eyeLidTop_L;
        _eyeLidTop_R = express.eyeLidTop_R;
        _eyeLidBot_L = express.eyeLidBot_L;
        _eyeLidBot_R = express.eyeLidBot_R;
        _smile = express.smile;
        _lipStretch = express.lipStretch;
        _lipTight = express.lipTight;
        _pout = express.pout;
        _speak = express.speak;
        _lipCnrs = express.lipCnrs;
        _sneer = express.sneer;
        _tongueStretch = express.tongueStretch;
        _tongueUpDown = express.tongueUpDown;

        //Mood Control parameters (Expression Control)
        //_pleasure = express.pleasure;
        //_arousal = express.arousal;
        _subDom = express.subDom;

        //MoodControl
        _pleasureDefault = mood.pleasureDefault;
        _arousalDefault = mood.arousalDefault;
        _dominanceDefault = mood.dominanceDefault;
        _moodBlinkRate = mood.moodBlinkRate;
        _moodMouthMoves = mood.moodMouthMoves;
        _moodHeadMoves = mood.moodHeadMoves;
        _moodEqualiseSpeed = mood.moodEqualiseSpeed;
    }
    
    void UserInputs() 
    {
        if (moodPad.moodPadActive == true) 
        {
            _pleasure = moodPad.xValue;
            _arousal = moodPad.yValue;
        }
        
        if (headPad.headPadActive == true)
        {
            _headIsTurning = false;
            _headIsNodding = false;
            _headTurn = headPad.xValue;
            _headNod = headPad.yValue;
        }
        else if (headPad.headPadActive == false)
        {
            _headIsTurning = true;
            _headIsNodding = true;
        }
    }
    
    void AutoMovements() //***TODO - Add a way to reset time to zero for each sine wave (resets to zero in start method.)
    {
        if (_headTurnSpeed != 0f && _headIsTurning == true )
        {
            //if (_headTurn <= _headTurnMin + 0.01f) { _headTurnTarget = _headTurnMax; }        //This uses Lerp. Change to control lerp with duration, not speed.
            //if (_headTurn >= _headTurnMax - 0.01f) { _headTurnTarget = _headTurnMin; }
            //_headTurnTime = _headTurnSpeed * Time.deltaTime;
            //float headTurnEvaluation = _headTurnCurve.Evaluate(_headTurnTime);                      
            //_headTurn = Mathf.Lerp(_headTurn, _headTurnTarget, headTurnEvaluation);

            _headTurnTime += Time.deltaTime * _headTurnSpeed;                                     // *** This is the sine wave code
            float headTurnValue = Mathf.Sin(_headTurnTime);// * _headTurnAmplitude;
            float headTurnRange = _headTurnMax - _headTurnMin;
            float headTurnFinalValue = (headTurnValue + 1f) / 2f * headTurnRange + _headTurnMin;
            _headTurn = headTurnFinalValue;
        }

        if (_headNodSpeed != 0f && _headIsNodding == true)
        {
            if (_headNodPlusTurn == true) // *** this bit of code is meant to make head swoop - needs work *TODO
            {
                _headNodTime = _headTurnTime * 2;
                _headNodSpeed = _headTurnSpeed;

                _headNodTime += Time.deltaTime * _headNodSpeed;
                float headNodRange = _headNodMax - _headNodMin;
                float headNodValue = Mathf.Cos(_headNodTime);// * _headNodAmplitude;
                float headNodFinalValue = (headNodValue + 1f) / 2f * headNodRange + _headNodMin;
                _headNod = headNodFinalValue;
            }
            else
            {
                _headNodTime += Time.deltaTime * _headNodSpeed;
                float headNodValue = Mathf.Sin(_headNodTime);// * _headNodAmplitude;
                float headNodRange = _headNodMax - _headNodMin;
                float headNodFinalValue = (headNodValue + 1f) / 2f * headNodRange + _headNodMin;
                _headNod = headNodFinalValue;
            }
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
            _eyeLidTop_L = Mathf.Lerp(_topLid_L_Min, _eyeLidBot_L, _testAnimCurve.Evaluate(_topLid_L_Time / _blinkDuration));
            _eyeLidTop_R = Mathf.Lerp(_topLid_R_Min, _eyeLidBot_R, _testAnimCurve.Evaluate(_topLid_L_Time / _blinkDuration));
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


    void ControlMood() 
    {
        //_pleasure = Mathf.MoveTowards(_pleasure, _pleasureDefault, _moodEqualiseSpeed * Time.deltaTime);

        //_arousal = Mathf.MoveTowards(_arousal, _arousalDefault, _moodEqualiseSpeed * Time.deltaTime);

        //_subDom = Mathf.MoveTowards(_subDom, _dominanceDefault, _moodEqualiseSpeed * Time.deltaTime);

        if (_pleasure < -0.5f && _arousal > 0.5f)                                               // x -1, y1
        {
            if (_pleasure == -1 && _arousal == 1) 
            {                
                _headTurnMax = 0.44f; //private float _modheadTurnMax = 0.44; _headTurnMax = Mathf.Lerp(_headTurnMax, _modvalue, t);
                _headTurnMin = 0.21f;
                _headTurnSpeed = 14f;
                _headNodMax = 0.48f;
                _headNodMin = 0.13f;
                _headNodSpeed = 1f;
                _headTiltMax = 0.46f;
                _headTiltMin = 0.13f;
                _headTiltSpeed = 0.3f;
                _jawOpenMax = 0.039f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 6.4f;
                _lookUDMax = 0.25f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1f;
                _lookLRMax = -0.43f;
                _lookLRMin = 0f;
                _lookLRSpeed = 1.1f;
                _shoulderSpeed = 5.7f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.115f;
                _topLid_R_Min = 0.134f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 0.4f;
                _lidsMeet_L = 0.76f;
                _lidsMeet_R = 0.76f;
                _botLid_L_Max = 0.85f;
                _botLid_R_Max = 0.86f;
            }
            else 
            {
                _headTurnMax = -0.04f;
                _headTurnMin = -0.27f;
                _headTurnSpeed = 9.6f;
                _headNodMax = 0.51f;
                _headNodMin = 0.25f;
                _headNodSpeed = 0.3f;
                _headTiltMax = 0.18f;
                _headTiltMin = -0.12f;
                _headTiltSpeed = 0.6f;
                _jawOpenMax = 0.03f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 12.2f;
                _lookUDMax = 0f;
                _lookUDMin = 0f;
                _lookUDSpeed = 0f;
                _lookLRMax = 0.9f;
                _lookLRMin = 0f;
                _lookLRSpeed = 1f;
                _shoulderSpeed = 2.7f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0.7f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0.7f;
                _topLid_L_Min = 0.115f;
                _topLid_R_Min = 0.134f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 0.7f;
                _lidsMeet_L = 0.72f;
                _lidsMeet_R = 0.72f;
                _botLid_L_Max = 0.8f;
                _botLid_R_Max = 0.8f;
            }
        }
        if (_pleasure >= -0.5f && _pleasure <= 0.5f && _arousal > 0.5f)                         //x 0, y 1
        {
            if (_arousal == 1) 
            {
                _headTurnMax = 0.2f;
                _headTurnMin = -0.15f;
                _headTurnSpeed = 0.2f;
                _headNodMax = 0.2f;
                _headNodMin = -0.03f;
                _headNodSpeed = 0.6f;
                _headTiltMax = 0.15f;
                _headTiltMin = -0.1f;
                _headTiltSpeed = 0.8f;
                _jawOpenMax = 0.557f;
                _jawOpenMin = 0.27f;
                _jawOpenSpeed = 0.7f;
                _lookUDMax = 0f;
                _lookUDMin = 0f;
                _lookUDSpeed = 0.1f;
                _lookLRMax = 0f;
                _lookLRMin = 0f;
                _lookLRSpeed = 0.1f;
                _shoulderSpeed = 4.1f;
                _shoulder_L_Max = 0.365f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.365f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0f;
                _topLid_R_Min = 0f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 0.6f;
                _lidsMeet_L = 0.72f;
                _lidsMeet_R = 0.72f;
                _botLid_L_Max = 0.8f;
                _botLid_R_Max = 0.8f;
            }
            else 
            {
                _headTurnMax = 0.423f;
                _headTurnMin = -0.234f;
                _headTurnSpeed = 0.4f;
                _headNodMax = 0.36f;
                _headNodMin = -0.03f;
                _headNodSpeed = 0.6f;
                _headTiltMax = 0.25f;
                _headTiltMin = -0.15f;
                _headTiltSpeed = 0.8f;
                _jawOpenMax = 0.354f;
                _jawOpenMin = 0.174f;
                _jawOpenSpeed = 1.1f;
                _lookUDMax = 0f;
                _lookUDMin = -0.36f;
                _lookUDSpeed = 2.3f;
                _lookLRMax = 0f;
                _lookLRMin = -0.33f;
                _lookLRSpeed = 0.7f;
                _shoulderSpeed = 4.1f;
                _shoulder_L_Max = 0.365f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.365f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.165f;
                _topLid_R_Min = 0.165f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 0.6f;
                _lidsMeet_L = 0.72f;
                _lidsMeet_R = 0.72f;
                _botLid_L_Max = 0.8f;
                _botLid_R_Max = 0.8f;
            }
        }
        if (_pleasure > 0.5f && _arousal > 0.5f)                                                //x 1, y 1
        {
            if (_pleasure == 1 && _arousal == 1) 
            {
                _headTurnMax = 0.33f;
                _headTurnMin = -0.32f;
                _headTurnSpeed = 0.3f;
                _headNodMax = 0.76f;
                _headNodMin = -0.37f;
                _headNodSpeed = 6f;
                _headTiltMax = 0.47f;
                _headTiltMin = -0.52f;
                _headTiltSpeed = 0.5f;
                _jawOpenMax = 0.407f;
                _jawOpenMin = 0.125f;
                _jawOpenSpeed = 0.5f;
                _lookUDMax = 0.28f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1.8f;
                _lookLRMax = 0f;
                _lookLRMin = 0f;
                _lookLRSpeed = 0.1f;
                _shoulderSpeed = 5.9f;
                _shoulder_L_Max = 0.799f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.799f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0f;
                _topLid_R_Min = 0f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 0.69f;
                _lidsMeet_L = 0.92f;
                _lidsMeet_R = 0.92f;
                _botLid_L_Max = 1f;
                _botLid_R_Max = 1f;
            }
            else 
            {
                _headTurnMax = 0.62f;
                _headTurnMin = -0.54f;
                _headTurnSpeed = 0.3f;
                _headNodMax = 0.29f;
                _headNodMin = -0.18f;
                _headNodSpeed = 4.2f;
                _headTiltMax = 0.18f;
                _headTiltMin = -0.26f;
                _headTiltSpeed = 1.5f;
                _jawOpenMax = 0.235f;
                _jawOpenMin = 0.125f;
                _jawOpenSpeed = 2.2f;
                _lookUDMax = 0.84f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1.8f;
                _lookLRMax = 0f;
                _lookLRMin = 0f;
                _lookLRSpeed = 0.1f;
                _shoulderSpeed = 4.1f;
                _shoulder_L_Max = 0.365f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.365f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.158f;
                _topLid_R_Min = 0.158f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 1.52f;
                _lidsMeet_L = 0.679f;
                _lidsMeet_R = 0.679f;
                _botLid_L_Max = 0.727f;
                _botLid_R_Max = 0.727f;
            }
        }
        if (_pleasure < -0.5f && _arousal <= 0.5f && _arousal >= -0.5f)                          //x -1, y 0
        {
            if (_pleasure == -1) 
            {
                _headTurnMax = 0.52f;
                _headTurnMin = -0.42f;
                _headTurnSpeed = 6.8f;
                _headNodMax = 0.87f;
                _headNodMin = 0.69f;
                _headNodSpeed = 0.5f;
                _headTiltMax = 0.44f;
                _headTiltMin = -0.26f;
                _headTiltSpeed = 0.7f;
                _jawOpenMax = 0.096f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 3.5f;
                _lookUDMax = 0f;
                _lookUDMin = 0f;
                _lookUDSpeed = 0.1f;
                _lookLRMax = 0f;
                _lookLRMin = 0f;
                _lookLRSpeed = 0.1f;
                _shoulderSpeed = 6.8f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0.809f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0.809f;
                _topLid_L_Min = 0.558f;
                _topLid_R_Min = 0.558f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 1f;
                _lidsMeet_L = 0.558f;
                _lidsMeet_R = 0.558f;
                _botLid_L_Max = 0.565f;
                _botLid_R_Max = 0.565f;
                _botLidSpeed = 4.4f;
            }
            else 
            {
                _headTurnMax = 0.52f;
                _headTurnMin = -0.42f;
                _headTurnSpeed = 1.1f;
                _headNodMax = 0.44f;
                _headNodMin = 0.3f;
                _headNodSpeed = 0.5f;
                _headTiltMax = 0.1f;
                _headTiltMin = -0.26f;
                _headTiltSpeed = 0.1f;
                _jawOpenMax = 0.267f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 0.9f;
                _lookUDMax = 0.61f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1.7f;
                _lookLRMax = 0f;
                _lookLRMin = 0f;
                _lookLRSpeed = 0.1f;
                _shoulderSpeed = 2.1f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0.63f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0.63f;
                _topLid_L_Min = 0.523f;
                _topLid_R_Min = 0.523f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 1f;
                _lidsMeet_L = 0.631f;
                _lidsMeet_R = 0.631f;
                _botLid_L_Max = 0.672f;
                _botLid_R_Max = 0.672f;
                _botLidSpeed = 4.4f;
            }
        }
        if (_pleasure >= -0.5f && _pleasure <= 0.5f && _arousal <= 0.5f && _arousal >= -0.5f)   //x 0, y 0
        {
            _headTurnMax = 0.33f;
            _headTurnMin = -0.33f;
            _headTurnSpeed = 0.2f;
            _headNodMax = 0.17f;
            _headNodMin = -0.09f;
            _headNodSpeed = 0.4f;
            _headTiltMax = 0.1f;
            _headTiltMin = -0.1f;
            _headTiltSpeed = 0.6f;
            _jawOpenMax = 0.076f;
            _jawOpenMin = 0f;
            _jawOpenSpeed = 0.1f;
            _lookUDMax = 0f;
            _lookUDMin = 0f;
            _lookUDSpeed = 0.1f;
            _lookLRMax = 0.36f;
            _lookLRMin = -0.36f;
            _lookLRSpeed = 0.5f;
            _shoulderSpeed = 1.8f;
            _shoulder_L_Max = 0.365f;
            _shoulder_L_Min = 0f;
            _shoulder_R_Max = 0.365f;
            _shoulder_R_Min = 0f;
            _topLid_L_Min = 0.275f;
            _topLid_R_Min = 0.275f;
            _blinkDuration = 0.5f;
            _blinkPauseDuration = 3f;
            _lidsMeet_L = 0.78f;
            _lidsMeet_R = 0.78f;
            _botLid_L_Max = 0.81f;
            _botLid_R_Max = 0.81f;
            _botLidSpeed = 0.3f;
        }
        if (_pleasure > 0.5f && _arousal <= 0.5f && _arousal >= -0.5f)                          //x 1, y 0
        {
            if (_pleasure == 1) 
            {
                _headTurnMax = 0.33f;
                _headTurnMin = -0.33f;
                _headTurnSpeed = 0.2f;
                _headNodMax = 0.17f;
                _headNodMin = -0.09f;
                _headNodSpeed = 6.3f;
                _headTiltMax = 0.33f;
                _headTiltMin = -0.33f;
                _headTiltSpeed = 2.7f;
                _jawOpenMax = 0.318f;
                _jawOpenMin = 0.179f;
                _jawOpenSpeed = 3.8f;
                _lookUDMax = 0.51f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1.5f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.5f;
                _shoulderSpeed = 3.2f;
                _shoulder_L_Max = 0.66f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.66f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.303f;
                _topLid_R_Min = 0.303f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 2f;
                _lidsMeet_L = 0.566f;
                _lidsMeet_R = 0.566f;
                _botLid_L_Max = 0.6f;
                _botLid_R_Max = 0.6f;
                _botLidSpeed = 5.5f;
            }
            else 
            {
                _headTurnMax = 0.33f;
                _headTurnMin = -0.33f;
                _headTurnSpeed = 0.2f;
                _headNodMax = 0.17f;
                _headNodMin = -0.09f;
                _headNodSpeed = 4f;
                _headTiltMax = 0.2f;
                _headTiltMin = -0.2f;
                _headTiltSpeed = 1.6f;
                _jawOpenMax = 0.2f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 0.5f;
                _lookUDMax = 0.51f;
                _lookUDMin = 0f;
                _lookUDSpeed = 1.5f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.5f;
                _shoulderSpeed = 3.2f;
                _shoulder_L_Max = 0.559f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.559f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.303f;
                _topLid_R_Min = 0.303f;
                _blinkDuration = 0.5f;
                _blinkPauseDuration = 2.5f;
                _lidsMeet_L = 0.65f;
                _lidsMeet_R = 0.65f;
                _botLid_L_Max = 0.7f;
                _botLid_R_Max = 0.7f;
                _botLidSpeed = 2.9f;
            }
        }
        if (_pleasure < -0.5f && _arousal < -0.5f)                                              //x -1, y -1
        {
            if (_pleasure == -1 && _arousal == -1)
            {
                _headTurnMax = 0.2f;
                _headTurnMin = -0.47f;
                _headTurnSpeed = 0.3f;
                _headNodMax = -0.74f;
                _headNodMin = -1f;
                _headNodSpeed = 0.4f;
                _headTiltMax = 0f;
                _headTiltMin = -0.23f;
                _headTiltSpeed = 0.4f;
                _jawOpenMax = 0.433f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 0.4f;
                _lookUDMax = -1;
                _lookUDMin = -1f;
                _lookUDSpeed = 0.1f;
                _lookLRMax = 0.24f;
                _lookLRMin = -0.44f;
                _lookLRSpeed = 0.5f;
                _shoulderSpeed = 1f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0.334f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0.334f;
                _topLid_L_Min = 0.44f;
                _topLid_R_Min = 0.44f;
                _blinkDuration = 3.5f;
                _blinkPauseDuration = 2f;
                _lidsMeet_L = 0.57f;
                _lidsMeet_R = 0.57f;
                _botLid_L_Max = 0.6f;
                _botLid_R_Max = 0.6f;
                _botLidSpeed = 0.12f;
            }
            else
            {
                _headTurnMax = -0.3f;
                _headTurnMin = -0.47f;
                _headTurnSpeed = 0.3f;
                _headNodMax = -0.39f;
                _headNodMin = -0.56f;
                _headNodSpeed = 0.4f;
                _headTiltMax = 0f;
                _headTiltMin = -0.23f;
                _headTiltSpeed = 0.4f;
                _jawOpenMax = 0.213f;
                _jawOpenMin = 0.103f;
                _jawOpenSpeed = 0.4f;
                _lookUDMax = -0.3f;
                _lookUDMin = -0.7f;
                _lookUDSpeed = 0.33f;
                _lookLRMax = 0.24f;
                _lookLRMin = -0.44f;
                _lookLRSpeed = 0.5f;
                _shoulderSpeed = 2.8f;
                _shoulder_L_Max = 1f;
                _shoulder_L_Min = 0.63f;
                _shoulder_R_Max = 1f;
                _shoulder_R_Min = 0.63f;
                _topLid_L_Min = 0.44f;
                _topLid_R_Min = 0.44f;
                _blinkDuration = 3f;
                _blinkPauseDuration = 2f;
                _lidsMeet_L = 0.57f;
                _lidsMeet_R = 0.57f;
                _botLid_L_Max = 0.6f;
                _botLid_R_Max = 0.6f;
                _botLidSpeed = 0.12f;
            }
        }
        if (_pleasure >= -0.5f && _pleasure <= 0.5f && _arousal < -0.5f)                        //x 0, y -1
        {
            if (_arousal == -1)
            {
                _headTurnMax = 0.14f;
                _headTurnMin = -0.23f;
                _headTurnSpeed = 0.2f;
                _headNodMax = -1f;
                _headNodMin = -0.66f;
                _headNodSpeed = 0.5f;
                _headTiltMax = 0.1f;
                _headTiltMin = -0.1f;
                _headTiltSpeed = 0.3f;
                _jawOpenMax = 0.17f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 1.1f;
                _lookUDMax = -0.3f;
                _lookUDMin = -0.3f;
                _lookUDSpeed = 0.1f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.2f;
                _shoulderSpeed = 1.8f;
                _shoulder_L_Max = 0.54f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.54f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.53f;
                _topLid_R_Min = 0.45f;
                _blinkDuration = 3f;
                _blinkPauseDuration = 3f;
                _lidsMeet_L = 0.6f;
                _lidsMeet_R = 0.53f;
                _botLid_L_Max = 0.61f;
                _botLid_R_Max = 0.57f;
                _botLidSpeed = 1.1f;
            }
            else
            {
                _headTurnMax = 0.14f;
                _headTurnMin = -0.23f;
                _headTurnSpeed = 0.2f;
                _headNodMax = -0.64f;
                _headNodMin = -0.39f;
                _headNodSpeed = 1.1f;
                _headTiltMax = 0.1f;
                _headTiltMin = -0.1f;
                _headTiltSpeed = 0.7f;
                _jawOpenMax = 0.162f;
                _jawOpenMin = 0.058f;
                _jawOpenSpeed = 0.4f;
                _lookUDMax = -0.5f;
                _lookUDMin = -0.97f;
                _lookUDSpeed = 0.2f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.2f;
                _shoulderSpeed = 1.8f;
                _shoulder_L_Max = 0.365f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.365f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.45f;
                _topLid_R_Min = 0.45f;
                _blinkDuration = 2.5f;
                _blinkPauseDuration = 4f;
                _lidsMeet_L = 0.58f;
                _lidsMeet_R = 0.58f;
                _botLid_L_Max = 0.61f;
                _botLid_R_Max = 0.61f;
                _botLidSpeed = 1.1f;
            }
        }
        if (_pleasure > 0.5f && _arousal < -0.5f)                                               //x 1, y -1
        {
            if (_pleasure == 1 && _arousal == -1)
            {
                _headTurnMax = 0.45f;
                _headTurnMin = -0.23f;
                _headTurnSpeed = 0.2f;
                _headNodMax = -1f;
                _headNodMin = -0.39f;
                _headNodSpeed = 1f;
                _headTiltMax = 0.35f;
                _headTiltMin = -0.41f;
                _headTiltSpeed = 0.3f;
                _jawOpenMax = 0.162f;
                _jawOpenMin = 0f;
                _jawOpenSpeed = 0.7f;
                _lookUDMax = -0.03f;
                _lookUDMin = -0.03f;
                _lookUDSpeed = 0.2f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.2f;
                _shoulderSpeed = 1.8f;
                _shoulder_L_Max = 0.77f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.76f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.42f;
                _topLid_R_Min = 0.45f;
                _blinkDuration = 3f;
                _blinkPauseDuration = 2f;
                _lidsMeet_L = 0.5f;
                _lidsMeet_R = 0.5f;
                _botLid_L_Max = 0.5f;
                _botLid_R_Max = 0.5f;
                _botLidSpeed = 0.1f;
            }
            else
            {
                _headTurnMax = 0.14f;
                _headTurnMin = -0.23f;
                _headTurnSpeed = 0.2f;
                _headNodMax = -0.21f;
                _headNodMin = -0.39f;
                _headNodSpeed = 1.1f;
                _headTiltMax = 0.19f;
                _headTiltMin = -0.21f;
                _headTiltSpeed = 0.7f;
                _jawOpenMax = 0.162f;
                _jawOpenMin = 0.058f;
                _jawOpenSpeed = 0.4f;
                _lookUDMax = -0.03f;
                _lookUDMin = -0.03f;
                _lookUDSpeed = 0.2f;
                _lookLRMax = 0.36f;
                _lookLRMin = -0.36f;
                _lookLRSpeed = 0.2f;
                _shoulderSpeed = 1.8f;
                _shoulder_L_Max = 0.365f;
                _shoulder_L_Min = 0f;
                _shoulder_R_Max = 0.365f;
                _shoulder_R_Min = 0f;
                _topLid_L_Min = 0.42f;
                _topLid_R_Min = 0.45f;
                _blinkDuration = 2f;
                _blinkPauseDuration = 2f;
                _lidsMeet_L = 0.58f;
                _lidsMeet_R = 0.58f;
                _botLid_L_Max = 0.61f;
                _botLid_R_Max = 0.61f;
                _botLidSpeed = 1.1f;
            }
        }

    }

    public void SetNewEmote(string _emoteName)
    {
       Emote newEmote = null;
       for(int i = 0; i < Emotes.Length; i++)
        {
            if (Emotes[i].emoteName == _emoteName)
            {
                newEmote = Emotes[i];
            }
        }
        if (newEmote == null) return;
        PlayEmote(newEmote);    
    }
    public void PlayEmote(Emote emote)
    {
        currentEmote = emote;
        Debug.Log(emote.emoteName);

        _shoulder_L_Amount = currentEmote.shoulder_L_Amount;
        _shoulder_R_Amount = currentEmote.shoulder_R_Amount;
        _jawOpenAmount = currentEmote.jawOpenAmount;
        _squintAmount = currentEmote.squintAmount;
        _whaleEyeAmount = currentEmote.whaleEyeAmount;
        _browLiftAmount = currentEmote.browLiftAmount;
        _frownAmount = currentEmote.frownAmount;
        _eyeLidTop_L_Amount = currentEmote.eyeLidTop_L_Amount;
        _eyeLidTop_R_Amount = currentEmote.eyeLidTop_L_Amount;
        _eyeLidBot_L_Amount = currentEmote.eyeLidBot_L_Amount;
        _eyeLidBot_R_Amount = currentEmote.eyeLidBot_R_Amount;
        _smileAmount = currentEmote.smileAmount;
        _lipStretchAmount = currentEmote.lipStretchAmount;
        _lipTightAmount = currentEmote.lipTightAmount;
        _poutAmount = currentEmote.poutAmount;
        _speakAmount = currentEmote.speakAmount;
        _lipCnrsAmount = currentEmote.lipCnrsAmount;
        _sneerAmount = currentEmote.sneerAmount;

        _emoteBlinkRate = currentEmote.emoteBlinkRate; //***TODO figure out how to change blink rate as part of emote.
        _tongueStretchAmount = currentEmote.tongueStretchAmount;
        _tongueUpDownAmount = currentEmote.tongueUpDownAmount;

        _pleasureImpact = currentEmote.pleasureImpact;
        _arousalImpact = currentEmote.arousalImpact;
        _dominanceImpact = currentEmote.dominanceImpact;

        _attack = currentEmote.attack;
        _easeType = currentEmote.easeType;
        _sustain = currentEmote.sustain;
        _decay = currentEmote.decay;

        //ATTACK
        // *** Add Blink rate change
        DOVirtual.Float(0, 1, _attack, OnMoodOrEmoteUpdate).SetEase(_easeType);

        DOVirtual.Float(_shoulder_L, _shoulder_L_Amount, _attack, OnShoulder_L_Update).SetEase(_easeType);
        DOVirtual.Float(_shoulder_R, _shoulder_R_Amount, _attack, OnShoulder_R_Update).SetEase(_easeType);
        DOVirtual.Float(_jawOpen, _jawOpenAmount, _attack, OnJawOpenUpdate).SetEase(_easeType);
        DOVirtual.Float(_squint, _squintAmount, _attack, OnSquintUpdate).SetEase(_easeType);
        DOVirtual.Float(_whaleEye, _whaleEyeAmount, _attack, OnWhaleEyeUpdate).SetEase(_easeType);
        DOVirtual.Float(_browLift, _browLiftAmount, _attack, OnBrowLiftUpdate).SetEase(_easeType);
        DOVirtual.Float(_frown, _frownAmount, _attack, OnFrownUpdate).SetEase(_easeType);
        DOVirtual.Float(_eyeLidTop_L, _eyeLidTop_L_Amount, _attack, OnEyeLidTop_L_Update).SetEase(_easeType);
        DOVirtual.Float(_eyeLidTop_R, _eyeLidTop_R_Amount, _attack, OnEyeLidTop_R_Update).SetEase(_easeType);
        DOVirtual.Float(_eyeLidBot_L, _eyeLidBot_L_Amount, _attack, OnEyeLidBot_L_Update).SetEase(_easeType);
        DOVirtual.Float(_eyeLidBot_R, _eyeLidBot_R_Amount, _attack, OnEyeLidBot_R_Update).SetEase(_easeType);
        DOVirtual.Float(_smile, _smileAmount, _attack, OnSmileUpdate).SetEase(_easeType);
        DOVirtual.Float(_lipStretch, _lipStretchAmount, _attack, OnLipStretchUpdate).SetEase(_easeType);
        DOVirtual.Float(_lipTight, _lipTightAmount, _attack, OnLipTightUpdate).SetEase(_easeType);
        DOVirtual.Float(_pout, _poutAmount, _attack, OnPoutUpdate).SetEase(_easeType);
        DOVirtual.Float(_speak, _speakAmount, _attack, OnSpeakUpdate).SetEase(_easeType);
        DOVirtual.Float(_lipCnrs, _lipCnrsAmount, _attack, OnLipCnrsUpdate).SetEase(_easeType);
        DOVirtual.Float(_sneer, _sneerAmount, _attack, OnSneerUpdate).SetEase(_easeType).OnComplete(SustainEmote);
    }
    void SustainEmote() //SUSTAIN  "Hold Pose" - Push expression... //***TODO find a better way to pass time for duration of '_sustain'
    {

        DOVirtual.Float(_shoulder_L, _shoulder_L + (_shoulder_L_Amount / 5), _sustain, OnShoulder_L_Update);
        DOVirtual.Float(_shoulder_R, _shoulder_R + (_shoulder_R_Amount / 5), _sustain, OnShoulder_R_Update);
        DOVirtual.Float(_jawOpen, _jawOpen + (_jawOpenAmount / 5), _sustain, OnJawOpenUpdate);
        DOVirtual.Float(_squint, _squint + (_squintAmount / 5), _sustain, OnSquintUpdate);
        DOVirtual.Float(_whaleEye, _whaleEye + (_whaleEyeAmount / 5), _sustain, OnWhaleEyeUpdate);
        DOVirtual.Float(_browLift, _browLift + (_browLiftAmount / 5), _sustain, OnBrowLiftUpdate);
        DOVirtual.Float(_frown, _frown + (_frownAmount / 5), _sustain, OnFrownUpdate);
        DOVirtual.Float(_eyeLidTop_L, _eyeLidTop_L + (_eyeLidTop_L_Amount / 5), _sustain, OnEyeLidTop_L_Update);
        DOVirtual.Float(_eyeLidTop_R, _eyeLidTop_R + (_eyeLidTop_R_Amount / 5), _sustain, OnEyeLidTop_R_Update);
        DOVirtual.Float(_eyeLidBot_L, _eyeLidBot_L + (_eyeLidBot_L_Amount / 5), _sustain, OnEyeLidBot_L_Update);
        DOVirtual.Float(_eyeLidBot_R, _eyeLidBot_R + (_eyeLidBot_R_Amount / 5), _sustain, OnEyeLidBot_R_Update);
        DOVirtual.Float(_smile, _smile + (_smileAmount / 5), _sustain, OnSmileUpdate);
        DOVirtual.Float(_lipStretch, _lipStretch + (_lipStretchAmount / 5), _sustain, OnLipStretchUpdate);
        DOVirtual.Float(_lipTight, _lipTight + (_lipTightAmount / 5), _sustain, OnLipTightUpdate);
        DOVirtual.Float(_pout, _pout + (_poutAmount / 5), _sustain, OnPoutUpdate);
        DOVirtual.Float(_speak, _speak + (_speakAmount / 5), _sustain, OnSpeakUpdate);
        DOVirtual.Float(_lipCnrs, _lipCnrs + (_lipCnrsAmount / 5), _sustain, OnLipCnrsUpdate);
        DOVirtual.Float(_sneer, _sneer + (_sneerAmount / 5), _sustain, OnSneerUpdate).SetEase(_easeType);

        //Mood Impacts:
        DOVirtual.Float(_pleasure, _pleasure + _pleasureImpact, _sustain, OnPleasureUpdate).SetEase(_easeType);
        DOVirtual.Float(_arousal, _arousal + _arousalImpact, _sustain, OnArousalUpdate).SetEase(_easeType);
        DOVirtual.Float(_subDom, _subDom + _dominanceImpact, _sustain, OnSubDomUpdate).SetEase(_easeType).OnComplete(OnEmoteTransitionCompleted); //OnComplete(DecayEmote);
    }
    //void DecayEmote()
    //{
    //DECAY expressions
    //_moodOrEmote = Mathf.MoveTowards(_moodOrEmote, 0, _decay* Time.deltaTime); //Transitions to other Blend Tree ***

    //_headTurn = Mathf.MoveTowards(_headTurn, 0, _decay * Time.deltaTime);     //***TODO Could Probably remove all these
    //_headNod = Mathf.MoveTowards(_headNod, 0, _decay * Time.deltaTime);
    //_headTilt = Mathf.MoveTowards(_headTilt, 0, _decay * Time.deltaTime);
    //_headLateralX = Mathf.MoveTowards(_headLateralX, 0, _decay * Time.deltaTime);
    //_headLateralY = Mathf.MoveTowards(_headLateralY, 0, _decay * Time.deltaTime);

    //_shoulder_L = Mathf.MoveTowards(_shoulder_L, 0, _decay * Time.deltaTime);
    //_shoulder_R = Mathf.MoveTowards(_shoulder_R, 0, _decay * Time.deltaTime);
    //_jawOpen = Mathf.MoveTowards(_jawOpen, 0, _decay * Time.deltaTime);
    //_squint = Mathf.MoveTowards(_squint, 0, _decay * Time.deltaTime);
    //_whaleEye = Mathf.MoveTowards(_whaleEye, 0, _decay * Time.deltaTime);
    //_browLift = Mathf.MoveTowards(_browLift, 0, _decay * Time.deltaTime);
    //_frown = Mathf.MoveTowards(_frown, 0, _decay * Time.deltaTime);
    //_eyeLidTop_L = Mathf.MoveTowards(_eyeLidTop_L, 0, _decay * Time.deltaTime);
    //_eyeLidTop_R = Mathf.MoveTowards(_eyeLidTop_R, 0, _decay * Time.deltaTime);
    //_eyeLidBot_L = Mathf.MoveTowards(_eyeLidBot_L, 0, _decay * Time.deltaTime);
    //_eyeLidBot_R = Mathf.MoveTowards(_eyeLidBot_R, 0, _decay * Time.deltaTime);
    //_smile = Mathf.MoveTowards(_smile, 0, _decay * Time.deltaTime);
    //_lipStretch = Mathf.MoveTowards(_lipStretch, 0, _decay * Time.deltaTime);
    //_lipTight = Mathf.MoveTowards(_lipTight, 0, _decay * Time.deltaTime);
    //_pout = Mathf.MoveTowards(_pout, 0, _decay * Time.deltaTime);
    //_speak = Mathf.MoveTowards(_speak, 0, _decay * Time.deltaTime);
    //_lipCnrs = Mathf.MoveTowards(_lipCnrs, 0, _decay * Time.deltaTime);
    //_sneer = Mathf.MoveTowards(_sneer, 0, _decay * Time.deltaTime);
    //}

    void OnEmoteTransitionCompleted() 
    {
        Debug.Log("Hey, I've now completed my emote transition...");

        //*** restore Blink Rate

        // ***TODO Can set things in here for game to continue after Emote finished playing.
    }

    // Updates called back by Tweens:
    public void OnPleasureUpdate(float newValue) //*** TODO: set max/min limits for these updated values ***
    {
        _pleasure = newValue;
        _pleasure = Mathf.Clamp(_pleasure + _pleasureImpact, -1f, 1f);
        express.pleasure = _pleasure;
    }
    public void OnArousalUpdate(float newValue) 
    {
        _arousal = newValue;
        _arousal = Mathf.Clamp(_arousal + _arousalImpact, -1f, 1f);
        express.arousal = _arousal;
    }
    public void OnSubDomUpdate(float newValue) 
    {
        _subDom = newValue;
        _subDom = Mathf.Clamp(_subDom + _dominanceImpact, -1f, 1f);
        express.subDom = _subDom;
    }
    public void OnMoodOrEmoteUpdate(float newValue)
    {
        _moodOrEmote = newValue;
        _moodOrEmote = Mathf.Clamp(_moodOrEmote, 0f, 1f);
        express.moodOrEmote = _moodOrEmote;
    }
    public void OnShoulder_L_Update(float newValue)
    {
        _shoulder_L = newValue;
        _shoulder_L = Mathf.Clamp(_shoulder_L, 0f, 1f);
        express.shoulder_L = _shoulder_L;
    }
    public void OnShoulder_R_Update(float newValue)
    {
        _shoulder_R = newValue;
        _shoulder_R = Mathf.Clamp(_shoulder_R, 0f, 1f);
        express.shoulder_R = _shoulder_R;
    }
    public void OnJawOpenUpdate(float newValue)
    {
        _jawOpen = newValue;
        _jawOpen = Mathf.Clamp(_jawOpen, 0f, 1f);
        express.jawOpen = _jawOpen;
    }
    public void OnSquintUpdate(float newValue) 
    {
        _squint = newValue;
        _squint = Mathf.Clamp(_squint, 0f, 1f);
        express.squint = _squint;
    }
    public void OnWhaleEyeUpdate(float newValue)
    {
        _whaleEye = newValue;
        _whaleEye = Mathf.Clamp(_whaleEye, 0f, 1f);
        express.whaleEye = _whaleEye;
    }
    public void OnBrowLiftUpdate(float newValue)
    {
        _browLift = newValue;
        _browLift = Mathf.Clamp(_browLift, 0f, 1f);
        express.browLift = _browLift;
    }
    public void OnFrownUpdate(float newValue)
    {
        _frown = newValue;
        _frown = Mathf.Clamp(_frown, 0f, 1f);
        express.frown = _frown;
    }
    public void OnEyeLidTop_L_Update(float newValue)
    {
        _eyeLidTop_L = newValue;
        _eyeLidTop_L = Mathf.Clamp(_eyeLidTop_L, 0f, 1f);
        express.eyeLidTop_L = _eyeLidTop_L;
    }
    public void OnEyeLidTop_R_Update(float newValue)
    {
        _eyeLidTop_R = newValue;
        _eyeLidTop_R = Mathf.Clamp(_eyeLidTop_R, 0f, 1f);
        express.eyeLidTop_R = _eyeLidTop_R;
    }
    public void OnEyeLidBot_L_Update(float newValue)
    {
        _eyeLidBot_L = newValue;
        _eyeLidBot_L = Mathf.Clamp(_eyeLidBot_L, 0f, 1f);
        express.eyeLidBot_L = _eyeLidBot_L;
    }
    public void OnEyeLidBot_R_Update(float newValue)
    {
        _eyeLidBot_R = newValue;
        _eyeLidBot_R = Mathf.Clamp(_eyeLidBot_R, 0f, 1f);
        express.eyeLidBot_R = _eyeLidBot_R;
    }
    public void OnSmileUpdate(float newValue)
    {
        _smile = newValue;
        _smile = Mathf.Clamp(_smile, 0f, 1f);
        express.smile = _smile;
    }
    public void OnLipStretchUpdate(float newValue)
    {
        _lipStretch = newValue;
        _lipStretch = Mathf.Clamp(_lipStretch, 0f, 1f);
        express.lipStretch = _lipStretch;
    }
    public void OnLipTightUpdate(float newValue)
    {
        _lipTight = newValue;
        _lipTight = Mathf.Clamp(_lipTight, 0f, 1f);
        express.lipTight = _lipTight;
    }
    public void OnPoutUpdate(float newValue)
    {
        _pout = newValue;
        _pout = Mathf.Clamp(_pout, 0f, 1f);
        express.pout = _pout;

    }
    public void OnSpeakUpdate(float newValue)
    {
        _speak = newValue;
        _speak = Mathf.Clamp(_speak, 0f, 1f);
        animator.SetFloat("Speak", _speak);
        
    }
    public void OnLipCnrsUpdate(float newValue)
    {
        _lipCnrs = newValue;
        _lipCnrs = Mathf.Clamp(_lipCnrs, 0f, 1f);
        express.lipCnrs = _lipCnrs;
    }
    public void OnSneerUpdate(float newValue)
    {
        _sneer = newValue;
        _sneer = Mathf.Clamp(_sneer, 0f, 1f);
        express.sneer = _sneer;
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

        // *** DONT copy the lids for other things
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
        // *** DONT copy the lids for other things

        express.shoulder_L = _shoulder_L;
        express.shoulder_R = _shoulder_R;
        express.lookUD = _lookUD;
        express.lookLR = _lookLR;
        express.jawOpen = _jawOpen;
        express.squint = _squint;
        express.whaleEye = _whaleEye;
        express.browLift = _browLift;
        express.frown = _frown;
        express.eyeLidTop_L = _eyeLidTop_L;
        express.eyeLidTop_R = _eyeLidTop_R;
        express.eyeLidBot_L = _eyeLidBot_L;
        express.eyeLidBot_R = _eyeLidBot_R;
        express.smile = _smile;
        express.lipStretch = _lipStretch;
        express.lipTight = _lipTight;
        express.pout = _pout;
        express.speak = _speak;
        express.lipCnrs = _lipCnrs;
        express.sneer = _sneer;
        express.pleasure = _pleasure;
        express.arousal = _arousal;
        express.subDom = _subDom;
        express.tongueStretch = _tongueStretch;
        express.tongueUpDown = _tongueUpDown;
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
        animator.SetFloat("EarFlap_L", _earFlap_L);
        animator.SetFloat("EarFlap_R", _earFlap_R);
        animator.SetFloat("JawOpen", _jawOpen);
        animator.SetFloat("Squint", _squint);
        animator.SetFloat("WhaleEye", _whaleEye);
        animator.SetFloat("BrowLift", _browLift);
        animator.SetFloat("Frown", _frown);
        animator.SetFloat("EyeLidTop_L", _eyeLidTop_L);
        animator.SetFloat("EyeLidTop_R", _eyeLidTop_R);
        animator.SetFloat("EyeLidBot_L", _eyeLidBot_L);
        animator.SetFloat("EyeLidBot_R", _eyeLidBot_R);
        animator.SetFloat("Smile", _smile);
        animator.SetFloat("LipStretch", _lipStretch);
        animator.SetFloat("LipTight", _lipTight);
        animator.SetFloat("Pout", _pout);
        animator.SetFloat("Speak", _speak);
        animator.SetFloat("LipCnrs", _lipCnrs);
        animator.SetFloat("Sneer", _sneer);
        animator.SetFloat("Pleasure", _pleasure);
        animator.SetFloat("Arousal", _arousal);
        animator.SetFloat("SubDom", _subDom);
        animator.SetFloat("TongueStretch", _tongueStretch);
        animator.SetFloat("TongueUpDown", _tongueUpDown);
    }
}
