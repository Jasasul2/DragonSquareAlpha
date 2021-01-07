using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionScreen : MonoBehaviour
{
    public Material starObtainedMaterial;
    public GameObject coverPanel; 

    public Color[] starColors = new Color[3];
    public Color noStarsColor; 
    public LevelClass[] levels;
    public static LevelSelectionScreen instance;

    public static int[] xToLeftByID = { 7, 8, 15, 16, 23, 24, 31, 32 };

    public static int[] yMoveUpBorders = { 9, 17, 25 };
    public static float yOffset = 118.2f;

    public Button shop1, shop2;

    public void Awake()
    {
        instance = this; 

        for (int i = 0; i < levels.Length; i++)
        {
            if (i > 0)
            {
                levels[i].previousLevel = levels[i - 1];
            }
            levels[i].Initialize();
        }

        if (!levels[0].completed)
        {
            shop1.interactable = false;
            shop2.interactable = false; 
        }
    }

    public void HideLevelBoards()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
        HideLevelBoards(-1); //hides all boards 
    }


    public static void HideLevelBoards(int exceptionID)
    {
        foreach (LevelClass level in instance.levels)
        {
            if (level.showingBoard && level.levelID != exceptionID)
            {
                level.ShowBoard(false);
            }
        }
    }
}
