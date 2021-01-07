using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MasteryPrefab : MonoBehaviour
{
    public int order = 0;
    public Color masteryColor; 

    public string[] stageDescriptions = new string[3];

    public PlayerStats.Masteries mastery;
    public TextMeshProUGUI masteryName;

    private MasteryShop shop;

    public Image iconBg, icon;
    public Sprite iconIcon;

    public void Start()
    {
        masteryName.text = mastery.ToString();
        shop = GameObject.Find("Masteries").GetComponent<MasteryShop>();
        ColorName(PlayerStats.selectedMasteryStages[mastery]);
        iconBg.color = masteryColor;
        icon.sprite = iconIcon;
    }

    public void ColorName(int stage)
    {
        masteryName.color = shop.stageColors[stage];
    }

    public void ShowMasteryBox()
    {
        shop.ShowMasteryBox(order);
    }

    public void PlayMasteryClickSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

}
