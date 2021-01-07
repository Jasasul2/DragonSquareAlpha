using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "skin", menuName = "Skin", order = 1)]
public class Skin : ScriptableObject
{
    public string skinName = "skin";
    public int dragoinPrice = 10;
    public Sprite[] baseSprites = new Sprite[3];
    public Sprite[] midSprites = new Sprite[3];
    public Sprite headDetailSprite;
}

public class SkinData
{
    public bool locked = true;

    public SkinData(bool locked)
    {
        this.locked = locked;
    }
}