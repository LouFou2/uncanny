using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Script references Expression Control variables
// and controls changes to Mood.
// MoodControl should listen for impacts from EmotionalReactions ScriptableObjects

[CreateAssetMenu(fileName = "MoodControl", menuName = "Scriptable Object/MoodControl")]
public class MoodControl : ScriptableObject
{
    [HideInInspector] public UnityEvent updated;

    [Range(-1.0f, 1.0f)] public float pleasureDefault;   // mood parameters slowly return to default. Can be changed
    [Range(-1.0f, 1.0f)] public float arousalDefault;
    [Range(-1.0f, 1.0f)] public float dominanceDefault;
    [Range(0.0f, 1.0f)] public float moodBlinkRate;     // used for calculating blink rates for different moods
    [Range(0.0f, 1.0f)] public float moodMouthMoves;    // to change mouth movement variations in idle
    [Range(0.0f, 1.0f)] public float moodHeadMoves;     // to change head movement variations in idle
    [Range(0.0f, 1.0f)] public float moodEqualiseSpeed; // Rate at which mood returns to defaults

    // Combinations of Mood variables can be used to animate mood expressions in idle states

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
