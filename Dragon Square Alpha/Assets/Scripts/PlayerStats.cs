using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static int skinIndex = 0;
    public static Skin currentSkin;
    public static Color mainColor = Color.red, secondaryColor = Color.white;
    public static int currentStars, currentDragoins;

    public static string mainColorHex, secondaryColorHex;

    public enum SkinEffects
    { 
        Trail,      //gives dragon a trail
        Paint,      //gives draogn a lasting trail 
        Pulse,      //primary and secondary colors are lerping 
        Fade,       //primary and secondary colors' alpha channels are slowly fading in and out
        Kyoki,      //each peace is continously randomly rotating 
        Energy,     //dragon emits particles 
        Reversed,   //dragon is going backwards
        Pessimist    //everything is greyscale 
    }

    public static Dictionary<SkinEffects, bool> unlockedSkinEffects = new Dictionary<SkinEffects, bool>
    {
        { SkinEffects.Trail, false },
        { SkinEffects.Paint, false },
        { SkinEffects.Pulse, false },
        { SkinEffects.Fade, false },
        { SkinEffects.Kyoki, false },
        { SkinEffects.Energy, false },
        { SkinEffects.Reversed, false },
        { SkinEffects.Pessimist, false },
    };

    public static Dictionary<SkinEffects, bool> selectedSkinEffects = new Dictionary<SkinEffects, bool>
    {
        { SkinEffects.Trail, false },
        { SkinEffects.Paint, false },
        { SkinEffects.Pulse, false },
        { SkinEffects.Fade, false },
        { SkinEffects.Kyoki, false },
        { SkinEffects.Energy, false },
        { SkinEffects.Reversed, false },
        { SkinEffects.Pessimist, false },
    };

    public enum Masteries // 3 stages each - 3 stars, 5 stars, 10 stars prices 
    {  
        Logic,       //Stage 1 - waypoints have different colors based        *** Stage 2 - shows number of first and last placed waypoint *** Stage 3 - shows numbers of all waypoints 
        Vision,      //Stage 1 - shows a path for the first half of the tiles *** Stage 2 - shows a path for all waypints                  *** Stage 3 - shows a lingering path for all waypoints 
        Focus,       //Stage 1 - adds a quick restart button to the UI        *** Stage 2 - adds a timer to the UI                         *** Stage 3 - adds a pause button to the UI           
        Memory,      //Stage 1 - shows spirits of the first 3 waypoints       *** Stage 2 - shows spirits of all the waypoints             *** Stage 3 - Shows spirits of all waypints and places the first 3 automatically 
        Wisdom,      //Stage 1 - Dragon can tell you a small hint             *** Stage 2 - Dragon can give you a bigger hint              *** Stage 3 - Dragon can tell you where to put first waypoint
    }

    public static Dictionary<Masteries, int> unlockedMasteryStages = new Dictionary<Masteries, int>{
        { Masteries.Logic, 0},
        { Masteries.Vision, 0 },
        { Masteries.Focus, 0 },
        { Masteries.Memory, 0 },
        { Masteries.Wisdom, 0 },
    };

    public static Dictionary<Masteries, int> selectedMasteryStages = new Dictionary<Masteries, int>{
        { Masteries.Logic, 0},
        { Masteries.Vision, 0 },
        { Masteries.Focus, 0 },
        { Masteries.Memory, 0 },
        { Masteries.Wisdom, 0 },
    };

    private void Awake()
    {
        SaveSystem.Init();
        Load();
    }

    public void SetBaseStats()
    {
        skinIndex = 0;
        mainColorHex = "#1B4161";
        secondaryColorHex = "#FBF3A7";
        GetColorsFromHex();
        currentStars = 30;
        currentDragoins = 100;
        unlockedMasteryStages = new Dictionary<Masteries, int>{
            { Masteries.Logic, 0  },
            { Masteries.Vision, 0 },
            { Masteries.Focus, 0  },
            { Masteries.Memory, 0 },
            { Masteries.Wisdom, 0 },
        };

        selectedMasteryStages = new Dictionary<Masteries, int>{
            { Masteries.Logic, 0  },
            { Masteries.Vision, 0 },
            { Masteries.Focus, 0  },
            { Masteries.Memory, 0 },
            { Masteries.Wisdom, 0 },
        };

        unlockedSkinEffects = new Dictionary<SkinEffects, bool>
        {
            { SkinEffects.Trail, true },
            { SkinEffects.Paint, true },
            { SkinEffects.Pulse, true },
            { SkinEffects.Fade, true },
            { SkinEffects.Kyoki, true },
            { SkinEffects.Energy, true },
            { SkinEffects.Reversed, true },
            { SkinEffects.Pessimist, true },
        };

        selectedSkinEffects = new Dictionary<SkinEffects, bool>
        {
            { SkinEffects.Trail, false },
            { SkinEffects.Paint, false },
            { SkinEffects.Pulse, false },
            { SkinEffects.Fade, false },
            { SkinEffects.Kyoki, false },
            { SkinEffects.Energy, false },
            { SkinEffects.Reversed, false },
            { SkinEffects.Pessimist, false },
        };
    }

    public static void Save()
    {
        mainColorHex = ColorUtility.ToHtmlStringRGB(mainColor);
        secondaryColorHex = ColorUtility.ToHtmlStringRGB(secondaryColor);

        int[] unlockedM = new int[unlockedMasteryStages.Count];
        unlockedMasteryStages.Values.CopyTo(unlockedM, 0);

        int[] selectedM = new int[selectedMasteryStages.Count];
        selectedMasteryStages.Values.CopyTo(selectedM, 0);

        bool[] unlockedSE = new bool[unlockedSkinEffects.Count];
        unlockedSkinEffects.Values.CopyTo(unlockedSE, 0);

        bool[] selectedSE = new bool[selectedSkinEffects.Count];
        selectedSkinEffects.Values.CopyTo(selectedSE, 0);


        PlayerData dataToSave = new PlayerData(
            skinIndex,
            currentStars,
            currentDragoins,
            mainColorHex,
            secondaryColorHex,
            unlockedM,
            selectedM,
            unlockedSE,
            selectedSE
            ); 

        string json = JsonUtility.ToJson(dataToSave);
        SaveSystem.SavePlayer(json);
    }

    public void Load()
    {
        string json = SaveSystem.LoadPlayer();
        PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
        if (loadedData != null)
        {
            skinIndex = loadedData.skinIndex;
            currentStars = loadedData.stars;
            currentDragoins = loadedData.dragoins;
            mainColorHex = loadedData.mainHex;
            secondaryColorHex = loadedData.secHex;
            unlockedMasteryStages = new Dictionary<Masteries, int>
            {
                { Masteries.Logic,  loadedData.ums[0] },
                { Masteries.Vision, loadedData.ums[1] },
                { Masteries.Focus,  loadedData.ums[2] },
                { Masteries.Memory, loadedData.ums[3] },
                { Masteries.Wisdom, loadedData.ums[4] },
            };
            selectedMasteryStages = new Dictionary<Masteries, int>
            {
                { Masteries.Logic,  loadedData.sms[0] },
                { Masteries.Vision, loadedData.sms[1] },
                { Masteries.Focus,  loadedData.sms[2] },
                { Masteries.Memory, loadedData.sms[3] },
                { Masteries.Wisdom, loadedData.sms[4] },
            };
            GetColorsFromHex();

            unlockedSkinEffects = new Dictionary<SkinEffects, bool>
            {
                { SkinEffects.Trail, loadedData.use[0]    },
                { SkinEffects.Paint, loadedData.use[1]    },
                { SkinEffects.Pulse, loadedData.use[2]    },
                { SkinEffects.Fade, loadedData.use[3]     },
                { SkinEffects.Kyoki, loadedData.use[4]    },
                { SkinEffects.Energy, loadedData.use[5]   },
                { SkinEffects.Reversed, loadedData.use[6] },
                { SkinEffects.Pessimist, loadedData.use[7] }
            };

            selectedSkinEffects = new Dictionary<SkinEffects, bool>
            {
                { SkinEffects.Trail, loadedData.sse[0]    },
                { SkinEffects.Paint, loadedData.sse[1]    },
                { SkinEffects.Pulse, loadedData.sse[2]    },
                { SkinEffects.Fade, loadedData.sse[3]     },
                { SkinEffects.Kyoki, loadedData.sse[4]    },
                { SkinEffects.Energy, loadedData.sse[5]   },
                { SkinEffects.Reversed, loadedData.sse[6] },
                { SkinEffects.Pessimist, loadedData.sse[7] }
            };
        }
        else
        { 
            SetBaseStats();
        }
    }

    public void GetColorsFromHex()
    {
        ColorUtility.TryParseHtmlString(FlexibleColorPicker.GetSanitizedHex(mainColorHex, true), out mainColor);
        ColorUtility.TryParseHtmlString(FlexibleColorPicker.GetSanitizedHex(secondaryColorHex, true), out secondaryColor);
    }
}

public class PlayerData
{
    public int skinIndex, stars, dragoins;
    public string mainHex, secHex;
    public int[] ums; //unlocked mastery stages
    public int[] sms; //selected mastery stages 
    public bool trail, paint, pulse, fade;
    public bool[] use; //unlocked skin effects
    public bool[] sse; //selected skin effects 

    public PlayerData(int skinIndex, int stars, int dragoins, string mainHex, string secHex,
        int[] ums, int[] sms, bool[] use, bool[] sse)
    {
        this.skinIndex = skinIndex;
        this.stars = stars;
        this.dragoins = dragoins;
        this.mainHex = mainHex;
        this.secHex = secHex;
        this.ums = ums;
        this.sms = sms;
        this.use = use;
        this.sse = sse; 
    }
}

