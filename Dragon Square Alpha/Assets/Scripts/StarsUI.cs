using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StarsUI : MonoBehaviour
{
    public TextMeshProUGUI waypointsUsed, 
        goldStar, silverStar, bronzeStar, levelWon, 
        starText, coinText, 
        starRewardedText, coinRewardedText,
        unlockedShop;

    public Animator timeSwitch;
    public Image[] stars = new Image[3];
    public Color[] starColors = new Color[3];
    private Level level;

    public GameObject starIcon, dragoinIcon;

    public Image detail;

    public Button nextLevelButton;

    private void Awake()
    {
        level = GameObject.Find("Level").GetComponent<Level>();

        goldStar.text = GameManager.instance.level.thisLevel.wayPointLimits[0].ToString() + " waypoints";
        silverStar.text = GameManager.instance.level.thisLevel.wayPointLimits[1].ToString() + " waypoints";
        bronzeStar.text = GameManager.instance.level.thisLevel.wayPointLimits[2].ToString() + " waypoints";

        int starNum = GameManager.numberOfStars;
        for (int i = 0; i < 3 - starNum; i++)
        {
            if (i == -1)
                return;

            stars[i].gameObject.SetActive(false);
        }
        foreach (Image star in stars)
        {
            if (star.gameObject.activeSelf)
            {
                star.color = starColors[starNum];
            }
        }
        waypointsUsed.text = GameManager.waypointsUsed.ToString() + " waypoints";
        waypointsUsed.color = starColors[starNum];
        levelWon.color = starColors[starNum];
        starText.color = starColors[starNum];
        starText.text = PlayerStats.currentStars.ToString();
        detail.color = starColors[starNum];

        if (GameManager.newRecord)
        { 
            starRewardedText.text = "+" + (starNum - GameManager.previousStars).ToString();
        }
        else
        {
            starRewardedText.gameObject.SetActive(false);
            starIcon.SetActive(false);
        }

        if (GameManager.coinsToReward != 0)
        {
            PlayerStats.currentDragoins += GameManager.coinsToReward;
            coinRewardedText.text = "+" + GameManager.coinsToReward.ToString();
        }
        else
        {
            coinRewardedText.gameObject.SetActive(false);
            dragoinIcon.SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1 && GameManager.firstWin)
        {
            unlockedShop.gameObject.SetActive(true);
        }

        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1)
        {
            nextLevelButton.interactable = false; 
        }

        coinText.color = starColors[starNum];
        coinText.text = PlayerStats.currentDragoins.ToString();

        PlayerStats.Save();
    }

    public void ShowTimePanel()
    {
        timeSwitch.SetTrigger("MoveRight");
    }
}
