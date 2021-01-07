using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class LevelBoardReferences : MonoBehaviour
{
    public Image levelImageBoard, decoration;
    public Image[] stars = new Image[3];
    public GameObject[] checkMarks = new GameObject[3];
    public GameObject unlocked; 
    public TextMeshProUGUI levelNameBoard, bronzeCoins, silverCoins, goldCoins, stateBoard, flavourBoard, highScoreHead, gold, silver, bronze;
    public int levelID;

    public void ClickSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

    public void Play()
    {
        PlayerStats.Save();
        MainMenuScript.StartStartingGame(levelID);
    }
}
