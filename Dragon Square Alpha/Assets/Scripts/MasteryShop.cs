using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class MasteryShop : MonoBehaviour
{
    public static readonly int[] starPrices = {0, 3, 6, 12 };
    public int cmIndex = 0; //current mastery index

    public Color[] stageColors = new Color[4];

    public Image symbol1, masteryBoxDetail; 

    public TextMeshProUGUI masteryNameInBox, stage1Text, stage2Text, stage3Text, starCount, stars;

    public GameObject blackBG, masteryBox, starIcon;

    public MasteryPrefab[] masteryPrefabs = new MasteryPrefab[5];
    public Sprite unlockedImage, lockedImage;
    public GameObject[] stageSelectors;
    public Button[] buyButtons;
    public GameObject[] outlines;
    public GameObject[] buyUI;

    private void Start()
    {
        UpdateStarCount();
        HideMasteryBox();
    }

    public void PlayMasteryClickSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

    public void ShowMasteryBox(int which)
    {
        cmIndex = which;
        symbol1.sprite = masteryPrefabs[cmIndex].iconIcon;
        masteryNameInBox.color = stageColors[PlayerStats.selectedMasteryStages[masteryPrefabs[cmIndex].mastery]];
        InitializeSelectors();
        SelectStage(PlayerStats.selectedMasteryStages[masteryPrefabs[cmIndex].mastery]);
        CheckButtons();
        UpdateStarCount();
        masteryBox.SetActive(true);
        masteryBox.GetComponent<Image>().color = masteryPrefabs[cmIndex].masteryColor;
        blackBG.SetActive(true);

        masteryNameInBox.text = masteryPrefabs[which].mastery.ToString();
        stage1Text.text = "Stage 1 : " + masteryPrefabs[which].stageDescriptions[0];
        stage2Text.text = "Stage 2 : " + masteryPrefabs[which].stageDescriptions[1];
        stage3Text.text = "Stage 3 : " + masteryPrefabs[which].stageDescriptions[2];
    }

    public void HideMasteryBox()
    {
        masteryBox.SetActive(false);
        blackBG.SetActive(false);
    }

    public void SelectStage(int which)
    {
        PlayerStats.selectedMasteryStages[masteryPrefabs[cmIndex].mastery] = which;
        InitializeSelectors();
        stageSelectors[which].GetComponent<Button>().interactable = false;
        outlines[which].SetActive(true);
        outlines[which].GetComponent<Image>().color = stageColors[which];
        masteryPrefabs[cmIndex].ColorName(which);
        masteryBoxDetail.color = stageColors[PlayerStats.selectedMasteryStages[masteryPrefabs[cmIndex].mastery]];
        masteryNameInBox.color = stageColors[PlayerStats.selectedMasteryStages[masteryPrefabs[cmIndex].mastery]];
    }

    public void BuyStage(int which)
    {
        if (PlayerStats.currentStars >= starPrices[which])
        {
            PlayerStats.unlockedMasteryStages[masteryPrefabs[cmIndex].mastery] = which;
            PlayerStats.currentStars -= starPrices[which];
            UpdateStarCount();

            VisualUnlock(which);
            SelectStage(which);
            CheckButtons();
        }
    }
    private void VisualUnlock(int which)
    {
        stageSelectors[which].GetComponent<Button>().interactable = true;
        stageSelectors[which].GetComponent<Image>().sprite = unlockedImage;
        stageSelectors[which].transform.GetChild(1).gameObject.SetActive(true);
    }

    private void DisableAllOutlines()
    {
        foreach (GameObject outline in outlines)
        {
            outline.SetActive(false);
        }
    }

    private void UpdateStarCount()
    {
        stars.text = PlayerStats.currentStars.ToString();
        starCount.text = PlayerStats.currentStars.ToString();
    }

    public void InitializeSelectors()
    {
        DisableAllOutlines();
        for (int i = 1; i < stageSelectors.Length; i++)
        {
            stageSelectors[i].GetComponent<Button>().interactable = false;
            stageSelectors[i].GetComponent<Image>().sprite = lockedImage;
            stageSelectors[i].transform.GetChild(1).gameObject.SetActive(false);
        }
        for (int i = 0; i < PlayerStats.unlockedMasteryStages[masteryPrefabs[cmIndex].mastery] + 1; i++)
        {
            VisualUnlock(i);
        }

    }

    public void CheckButtons()
    {
        if (PlayerStats.unlockedMasteryStages[masteryPrefabs[cmIndex].mastery] == 3)
        {
            starCount.gameObject.SetActive(false);
            starIcon.SetActive(false);
        }
        else
        {
            starCount.gameObject.SetActive(true);
            starIcon.SetActive(true);
        }

        for (int i = 1; i < starPrices.Length; i++)
        {
            //no money 
            if (starPrices[i] > PlayerStats.currentStars)
            {
                buyButtons[i - 1].interactable = false; 
            }
            else
                buyButtons[i - 1].interactable = true;

            //not unlocked yet
            if (PlayerStats.unlockedMasteryStages[masteryPrefabs[cmIndex].mastery] != i - 1)
            {
                buyButtons[i - 1].interactable = false;
            }

            //already unlocked 
            if (PlayerStats.unlockedMasteryStages[masteryPrefabs[cmIndex].mastery] >= i)
            {
                buyUI[i - 1].SetActive(false);
            }
            else
            {
                buyUI[i - 1].SetActive(true);
            }
        }
    }
}
