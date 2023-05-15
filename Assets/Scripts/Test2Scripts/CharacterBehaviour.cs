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

public class CharacterBehaviour : MonoBehaviour
{
    // Components involved in character Anmation
    public ExpressionControl express;   // ExpressionControl (Scriptable Object) - Controls all variables used to Animate character expressions and movement
    public MoodControl mood;            // Scriptable Object that stores variables to adjust Mood Animations
    public Emote[] Emotes;              // Scriptable Objects that adjusts expression variables to create different expressions - Animated using Tweening
    public Emote currentEmote;
    public Animator animator;

    // Sine Wave variables
    [HideInInspector] public float _headTurnMax;
    [HideInInspector] public float _headTurnMin;
    [HideInInspector] public float _headTurnSpeed;
    private float _headTurnTime;

    [HideInInspector] public float _headNodMax;
    [HideInInspector] public float _headNodMin;
    [HideInInspector] public float _headNodSpeed;
    private float _headNodTime;
    [HideInInspector] public bool _headNodPlusTurn;

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

    [HideInInspector] public float _topLidSpeed;

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

    [HideInInspector] public float _pleasure; // Mood Parameters
    [HideInInspector] public float _arousal;
    [HideInInspector] public float _subDom;
    [HideInInspector] public float _blinkRate;
    
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
        express.updated.AddListener(CheckParams);
        mood.updated.AddListener(CheckParams);
    }
    private void OnDisable()
    {
        express.updated.RemoveListener(CheckParams);
        mood.updated.RemoveListener(CheckParams);
    }
    
    void Start()
    {
        CheckParams();
    }

    void Update()
    {
        //ControlMood();
        SineWaveMovements();
        UpdateParams(); //updates ExpressionControl + MoodControl parameters (main data container for all expressions)
        Animate();
    }
    void CheckParams()
    {
        //Head Movements (Expression Control)
        _headTurn = express.headTurn;
        _headNod = express.headNod;
        _headTilt = express.headTilt;
        _headLateralX = express.headLateralX;
        _headLateralY = express.headLateralY;

        //Mood (Idle) or Emote (Emotional expression or Reaction)
        _moodOrEmote = express.moodOrEmote;

        //Sine Wave variables (Expression Control)
        _headTurnMax = express.headTurnMax;
        _headTurnMin = express.headTurnMin;
        _headTurnSpeed = express.headTurnSpeed;
        _headTurnTime = express.headTurnTime;

        _headNodMax = express.headNodMax;
        _headNodMin = express.headNodMin;
        _headNodSpeed = express.headNodSpeed;
        _headNodTime = express.headNodTime;
        _headNodPlusTurn = express.headNodPlusTurn;

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

        _topLidSpeed = express.topLidSpeed;

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
        _pleasure = express.pleasure;
        _arousal = express.arousal;
        _subDom = express.subDom;
        _blinkRate = express.blinkRate;

        //MoodControl
        _pleasureDefault = mood.pleasureDefault;
        _arousalDefault = mood.arousalDefault;
        _dominanceDefault = mood.dominanceDefault;
        _moodBlinkRate = mood.moodBlinkRate;
        _moodMouthMoves = mood.moodMouthMoves;
        _moodHeadMoves = mood.moodHeadMoves;
        _moodEqualiseSpeed = mood.moodEqualiseSpeed;
    }
    
    void SineWaveMovements() //***TODO - Add a way to reset time to zero for each sine wave (figure out best way to do this)
    {
        if (_headTurnSpeed != 0f)
        {
            _headTurnTime += Time.deltaTime * _headTurnSpeed;
            float headTurnValue = Mathf.Sin(_headTurnTime);// * _headTurnAmplitude;
            float headTurnRange = _headTurnMax - _headTurnMin;
            float headTurnFinalValue = (headTurnValue + 1f) / 2f * headTurnRange + _headTurnMin;
            _headTurn = headTurnFinalValue;
        }

        if (_headNodSpeed != 0f)
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
        if (_topLidSpeed != 0f)
        {
            _topLid_L_Time += Time.deltaTime * _topLidSpeed;
            float topLid_L_Value = Mathf.Sin(_topLid_L_Time);
            float topLid_L_Range = _eyeLidBot_L - _topLid_L_Min;
            float topLid_L_FinalValue = (topLid_L_Value + 1f) / 2f * topLid_L_Range + _topLid_L_Min;
            _eyeLidTop_L = topLid_L_FinalValue;

            _topLid_R_Time += Time.deltaTime * _topLidSpeed;
            float topLid_R_Value = Mathf.Sin(_topLid_R_Time);
            float topLid_R_Range = _eyeLidBot_R - _topLid_R_Min;
            float topLid_R_FinalValue = (topLid_R_Value + 1f) / 2f * topLid_R_Range + _topLid_R_Min;
            _eyeLidTop_R = topLid_R_FinalValue;

            if (_eyeLidTop_L <= _topLid_L_Min + 0.01f && !_pauseBlinking && _topLid_L_Time > 0.5f)
            {
                StartCoroutine(BlinkPause(_blinkPauseDuration));
            }
        }

        if(_botLidSpeed > 0) 
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
        float originalSpeed = _topLidSpeed;
        _topLidSpeed = 0.001f;

        yield return new WaitForSeconds(duration);

        _topLid_L_Time = 0;
        _topLid_R_Time = 0;
        _topLidSpeed = originalSpeed;
        _pauseBlinking = false;
    }


    void ControlMood() 
    {
        //_pleasure = Mathf.MoveTowards(_pleasure, _pleasureDefault, _moodEqualiseSpeed * Time.deltaTime);

        //_arousal = Mathf.MoveTowards(_arousal, _arousalDefault, _moodEqualiseSpeed * Time.deltaTime);

        //_subDom = Mathf.MoveTowards(_subDom, _dominanceDefault, _moodEqualiseSpeed * Time.deltaTime);

        //DECAY expressions
        _moodOrEmote = Mathf.MoveTowards(_moodOrEmote, 0, _decay * Time.deltaTime); //Transitions to other Blend Tree ***

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
        DOVirtual.Float(_subDom, _subDom + _dominanceImpact, _sustain, OnSubDomUpdate).SetEase(_easeType).OnComplete(OnEmoteTransitionCompleted);
    }

    //*** TODO: "Decay" of emotional reaction - in Mood Control
    // *** mood impacts = change "default" mood. Decay = Mood equalisation to new "default".
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
        express.headLateralX = _headLateralX;
        express.headLateralY = _headLateralY;

        express.moodOrEmote = _moodOrEmote;

        // *** Sine Waves ***
        express.headTurnMax = _headTurnMax;
        express.headTurnMin = _headTurnMin;
        express.headTurnSpeed = _headTurnSpeed;
        express.headTurnTime = _headTurnTime;

        express.headNodMax = _headNodMax;
        express.headNodMin = _headNodMin;
        express.headNodSpeed = _headNodSpeed;
        express.headNodTime = _headNodTime;

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
        express.topLidSpeed = _topLidSpeed;

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

        express.blinkPauseDuration = _blinkPauseDuration;
        // *** DONT copy the lids for other things

        //

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
        express.blinkRate = _blinkRate;
        express.tongueStretch = _tongueStretch;
        express.tongueUpDown = _tongueUpDown;

        //mood.moodBlinkRate = _moodBlinkRate;
        //mood.moodMouthMoves = _moodMouthMoves;
        //mood.moodHeadMoves = _moodHeadMoves;
        //mood.moodEqualiseSpeed = _moodEqualiseSpeed;
    }
    void Animate()
    {
        animator.SetFloat("HeadTurn", _headTurn);
        animator.SetFloat("HeadNod", _headNod);
        animator.SetFloat("HeadTilt", _headTilt);
        animator.SetFloat("HeadLateralX", _headLateralX);
        animator.SetFloat("HeadLateralY", _headLateralY);
        animator.SetFloat("MoodOrEmote", _moodOrEmote);
        animator.SetFloat("LookLR", _lookLR);
        animator.SetFloat("LookUD", _lookUD);
        animator.SetFloat("Shoulder_L", _shoulder_L);
        animator.SetFloat("Shoulder_R", _shoulder_R);
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
        animator.SetFloat("BlinkRate", _blinkRate);
        animator.SetFloat("TongueStretch", _tongueStretch);
        animator.SetFloat("TongueUpDown", _tongueUpDown);
    }
}
