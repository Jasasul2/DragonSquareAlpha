using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEffect : MonoBehaviour
{
    public PlayerStats.SkinEffects effect;
    public TextMeshProUGUI effectName;
    public Toggle toggle;
    private bool readyToMakeSound = false; 

    void Awake()
    {
        effectName.text = effect.ToString();
    }

    public void PrepareToggle()
    {
        if (!PlayerStats.unlockedSkinEffects[effect])
        {
            toggle.interactable = false;
        }
        else
        {
            toggle.interactable = true;
        }

        if (PlayerStats.selectedSkinEffects[effect])
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
        readyToMakeSound = true; 
    }

    public void SelectEffect(bool selected)
    {
        PlayerStats.selectedSkinEffects[effect] = selected; 
    }

    public void ClickSound()
    {
        if(readyToMakeSound)
        { 
            AudioManager.PlaySound(AudioManager.Sound.Click);
        }
    }
}
