using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro; 

public class VisualShop : MonoBehaviour
{
    public Image 
        dragonHead, 
        dragonHead2, 
        dragonHeadExtra,
        dragonBody, 
        dragonBody2, 
        dragonTail, 
        dragonTail2;

    public Color 
        mainColor, 
        secondaryColor = Color.white;
    public bool 
        main = true, 
        lockSkinForRandom = false;

    public FlexibleColorPicker colorPicker; 

    public Skin[] skins;
    public List<SkinData> boughtsSkins = new List<SkinData>();
    private Skin currentSkin;
    public int currentIndex = 0;

    public TextMeshProUGUI 
        whichColor, 
        skinName, 
        dragoins;
    public Animator switchColorAnimator;

    public GameObject
        skinLock, 
        lockedSkinBorder;
    public Button buyButton;
    public Image lockSkinImage;
    public Sprite 
        skinToBeLocked, 
        skinToBeUnlocked; 
    public TextMeshProUGUI price;

    public GameObject 
        EffectsBox, 
        effectsBoxBG;

    public void Start()
    {
        LoadSkinData();

        mainColor = PlayerStats.mainColor;
        secondaryColor = PlayerStats.secondaryColor;
        currentIndex = PlayerStats.skinIndex;
        UpdateDragoins(PlayerStats.currentDragoins);

        UpdateSkin();
        UpdateArt();
        colorPicker.TypeHex(ColorUtility.ToHtmlStringRGB(mainColor));
        UpdateColor(false, secondaryColor);
    }

    public void UpdateDragoins(int amount) => dragoins.text = amount.ToString();

    public void UpdateColorPicker()
    {
        if(main)
            colorPicker.TypeHex(ColorUtility.ToHtmlStringRGB(mainColor));
        else
            colorPicker.TypeHex(ColorUtility.ToHtmlStringRGB(secondaryColor));
    }

    public void SwitchColor()
    {
        main = !main;
        bool down = !main;
        switchColorAnimator.SetBool("Down", down);
        if (main)
        {
            whichColor.text = "Main Color";
        }
        else
        {
            whichColor.text = "Detail Color";
        }
        UpdateColorPicker();
    }

    public void UpdateColorCalled()
    {
        UpdateColor(main, colorPicker.color);
    }

    public void Move(int add)
    {
        currentIndex += add;
        if (currentIndex > skins.Length - 1)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = skins.Length - 1;

        UpdateSkin();
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

    public void UpdateArt()
    {

        skinName.text = currentSkin.skinName;
        dragonHead.sprite = currentSkin.baseSprites[0];
        dragonHead2.sprite = currentSkin.midSprites[0];
        dragonHeadExtra.sprite = currentSkin.headDetailSprite;
        dragonBody.sprite = currentSkin.baseSprites[1];
        dragonBody2.sprite = currentSkin.midSprites[1];
        dragonTail.sprite = currentSkin.baseSprites[2];
        dragonTail2.sprite = currentSkin.midSprites[2];
    }

    public void UpdateColor(bool main, Color color)
    {
        if (main)
        {
            mainColor = color;
            dragonHead.color = mainColor;
            dragonBody.color = mainColor;
            dragonTail.color = mainColor;
            PlayerStats.mainColor = mainColor;
        }
        else
        {
            secondaryColor = color;
            dragonHead2.color = secondaryColor;
            dragonBody2.color = secondaryColor;
            dragonTail2.color = secondaryColor;
            PlayerStats.secondaryColor = secondaryColor; 
        }

    }
    public void SwapColors()
    {
        Color color1 = mainColor;
        Color color2 = secondaryColor; 

        UpdateColor(true, color2);
        UpdateColor(false, color1);
        UpdateColorPicker();
    }

    private void UpdateSkin()
    {
        currentSkin = skins[currentIndex];
        if (CheckLock())
        {
            skinLock.SetActive(true);
            price.text = currentSkin.dragoinPrice.ToString();
            if (CheckPrice())
                buyButton.interactable = true;
            else
                buyButton.interactable = false; 
        }
        else
        {
            skinLock.SetActive(false);
            PlayerStats.skinIndex = currentIndex;
            PlayerStats.currentSkin = currentSkin;
        }

        UpdateArt();
    }

    public void ChangeSkinLockState()
    {
        lockSkinForRandom = !lockSkinForRandom;
        if (lockSkinForRandom)
        {
            lockedSkinBorder.SetActive(true);
            lockSkinImage.sprite = skinToBeUnlocked;
        }
        else
        {
            lockedSkinBorder.SetActive(false);
            lockSkinImage.sprite = skinToBeLocked;
        }
    }

    public void GenerateRandomSkinAndColors()
    {
        if (!lockSkinForRandom)
        { 
            do
            {
                currentIndex = Random.Range(0, skins.Length);
            } while (boughtsSkins[currentIndex].locked);    
        }

        UpdateSkin();
        UpdateColor(true, GenerateRandomColor());
        UpdateColor(false, GenerateRandomColor());
        UpdateColorPicker();
    }

    public Color GenerateRandomColor()
    {
        Color newColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            );
        return newColor;
    }

    public void LoadSkinData()
    {
        for (int id = 0; id < skins.Length; id++)
        {
            string hadson = SaveSystem.LoadSkinData(id);
            if (hadson != null)
            {
                boughtsSkins.Add(JsonUtility.FromJson<SkinData>(hadson));
                continue;
            }
            else 
            {
                if (id != 0)
                {
                    boughtsSkins.Add(new SkinData(true));
                    continue;
                }
            }
            boughtsSkins.Add(new SkinData(false));
        }
    }

    public void SaveSkinData()
    {
        for (int id = 0; id < skins.Length; id++)
        {
            string skinData = JsonUtility.ToJson(boughtsSkins[id]);
            SaveSystem.SaveSkinData(skinData, id);
        }
    }

    public void OnDestroy()
    {
        if (!SaveSystem.justReset)
        { 
            SaveSkinData();
            PlayerStats.Save();
        }
        else
            SaveSystem.justReset = false;

        SaveSystem.DeleteSpiritData();
    }

    public bool CheckLock() => boughtsSkins[currentIndex].locked;

    public bool CheckPrice() //checks if the player can afford to buy this skin 
    {
        if (skins[currentIndex].dragoinPrice <= PlayerStats.currentDragoins)
            return true;
        return false; 
    }

    public void BuyCurrentSkin()
    {
        if (CheckLock())
        {
            if (CheckPrice())
            {
                AudioManager.PlaySound(AudioManager.Sound.Buy);
                PlayerStats.currentDragoins -= skins[currentIndex].dragoinPrice;
                UpdateDragoins(PlayerStats.currentDragoins);
                skinLock.SetActive(false);
                boughtsSkins[currentIndex].locked = false;
                PlayerStats.skinIndex = currentIndex;
                PlayerStats.currentSkin = currentSkin;
            }
        }
    }

    public void ShowEffectsBox(bool show)
    {
        EffectsBox.SetActive(show);
        effectsBoxBG.SetActive(show);
    }
}
