using UnityEngine;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   //Handles Wining an losing, enemies and more 
    public Dragon dragon;
    public GameObject blackUnderTable; 

    public static Transform DragonHead; //for enemies 

    public static GameManager instance; 

    public GameObject winUI, timerObject;

    public static bool gameHasEnded, planningPhase = true, newRecord = false, firstWin = false;
    public static int numberOfStars, waypointsUsed = 80;  

    public Sprite[] bgTiles = new Sprite[4];
    public Sprite[] fgTiles = new Sprite[4];
    public Sprite[] danger = new Sprite[4]; 

    private GameObject[] enemies;

    public Level level;

    public int logic; //Mastery 

    public static int previousStars = 0;
    public static int coinsToReward = 0;

    public UIManager gameUI;

    public void Awake()
    {
        instance = this;
        newRecord = false;
        firstWin = false;
        gameHasEnded = false;
        planningPhase = true; 
        Cursor.visible = true;
        Time.timeScale = 0;


        instance.logic = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Focus];

        blackUnderTable = GameObject.Find("BlackUnderTable");
        blackUnderTable.SetActive(false);
        level = GameObject.Find("Level").GetComponent<Level>();
        timerObject = GameObject.Find("Timer");

        DragonHead = GameObject.Find("DragonHead").transform;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        gameUI = GameObject.Find("UI").GetComponent<UIManager>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameUI.ButtonNextLevel(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(instance.logic >= 1)
                Restart();
        }
    }

    public static void Restart()
    {
        AudioManager.PlaySound(AudioManager.Sound.Lose);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void NextLevel()
    {
        if (UIManager.shouldLoadMenu == true)
        {
            SceneManager.LoadScene(0);
        }
        else
        { 
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            if (currentScene + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SaveSystem.DeleteSpiritData();
                SceneManager.LoadScene(currentScene + 1);
            }
        }
    }

    public static Sprite ReturnBGTile() => instance.bgTiles[Random.Range(0, instance.bgTiles.Length)];

    public static Sprite ReturnDangerTile() => instance.danger[Random.Range(0, instance.danger.Length)];

    public static Sprite ReturnFGTile() => instance.fgTiles[Random.Range(0, instance.fgTiles.Length)];

    public static void EssenceEaten()
    {
        if (instance.dragon.dead)
            return;
        UIManager.EssenceCollected();
        OverCheck();
    }

    public static void ShrinkFoodEaten()
    {
        if (instance.dragon.dead)
            return;
        UIManager.ShrinkFoodCollected();
        OverCheck();
    }

    public static void OverCheck(bool die = false)
    {
        if (UIManager.CheckIfOver() && !instance.dragon.dead)
            WinLevel();
        else
        {
            if (die)
            {
                instance.dragon.Die();
            }
        }
    }

    private static void WinLevel()
    {
        AudioManager.PlaySound(AudioManager.Sound.Win);
        gameHasEnded = true;
        EraseEnemies();

        if (!instance.level.data.completed) //first time win 
        {
            firstWin = true;
            instance.level.data.completed = true;
        }

        previousStars = instance.level.data.obtainedStars;
        if (numberOfStars > previousStars)
        {
            newRecord = true;

            int starDifference = numberOfStars - previousStars;
            instance.level.data.obtainedStars = previousStars;
 
            PlayerStats.currentStars += starDifference;
            instance.level.data.obtainedStars = numberOfStars;

            int diff = numberOfStars - previousStars;
            int newDragoins = 0;

            for (int i = 0; i < diff; i++)
            {
                int newCoinsIndex = previousStars + i;
                newDragoins += instance.level.thisLevel.dragoinRewards[newCoinsIndex];
            }

            coinsToReward = newDragoins;
        }
        else
        {
            coinsToReward = 0;
        }

        instance.gameUI.RecolorUI();
        MusicManager.FadeOut(0.6f, 0f);
        instance.winUI.SetActive(true);
        instance.level.Save();
    }

    public static void EraseEnemies()
    {
        if (instance.enemies.Length > 0)
        {
            foreach (GameObject enemy in instance.enemies)
            {
                if (enemy != null)
                    enemy.GetComponent<IEnemyInterface>().Dissapear();
            }
        }
    }
}
