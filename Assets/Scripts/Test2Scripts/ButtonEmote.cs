using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEmote : MonoBehaviour
{
    public string emoteName;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnEmoteButtonClicked);
    }

    // Because this button will swap to different buttons with different Emotes (and other impacts),
    // I believe I need it to reference Scriptable Objects 
    public void OnEmoteButtonClicked()
    {
        CharacterBehaviour characterBehaviour = GameObject.FindObjectOfType<CharacterBehaviour>();

        if(characterBehaviour != null )
        {
            //characterBehaviour.SetNewEmote( emoteName );
            Debug.Log("set emote not active in script");
        }
    }
}
