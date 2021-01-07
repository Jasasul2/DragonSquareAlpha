using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public Vector3 playButtonFocus2Position;
    public Button playButton;

    public static bool 
        shouldLoadMenu = false, 
        hasTimer = false;

    private string levelName;
    public GameObject 
        restartButton,
        removeAllButton, 
        pauseButton;

    public static int 
        essencesToCollect, 
        foodToCollect;

    public Animator blackFader;

    public Toggle 
        Sound, 
        Music;

    public int focus; //Mastery
    public bool
        paused = false,
        pessimist = false;

    public Volume volume;
    private ColorAdjustments filter;

    public GameObject 
        hintTable, 
        blackBG;
    public static bool showingHintTable = false;

    public int wisdom;
    public PlanSystem system;
    public TextMeshProUGUI 
        dragonName, 
        description;

    public Button[] hintTableButtons = new Button[4];
    [HideInInspector] public LevelAttributes texts;
    public int lastSelectedText = 0;

    public Color[] starColors = new Color[4]; 
    public SpriteRenderer border;
    public TextMeshProUGUI[] colorableTexts;
    public ParticleSystem[] glowParticles;

    private Level level;

    public void Awake()
    {
        showingHintTable = false; 
        lastSelectedText = 0;
        focus = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Focus];
        wisdom = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Wisdom];
        pessimist = PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Pessimist];
        blackFader.gameObject.SetActive(true);

        if (AudioManager.muted)
            Sound.isOn = true;
        if (MusicManager.muted)
            Music.isOn = true;

        volume.profile.TryGet(out filter);
        if (pessimist)
        {
            filter.saturation.value = -100;
        }
    }

    public void Start()
    {
        level = GameObject.Find("Level").GetComponent<Level>();
        texts = level.thisLevel;
        levelName = texts.lName;
        GameObject.Find("LevelName").GetComponent<TMPro.TextMeshProUGUI>().text = levelName;

        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Food");
        essencesToCollect = collectibles.Length;

        int totalEssenceValues = 0;
        foreach (GameObject essence in collectibles)
        {
            totalEssenceValues += essence.GetComponent<Food>().foodValue;
        }
        GameObject.Find("Dragon").GetComponent<Dragon>().maxBodies = totalEssenceValues; 

        collectibles = GameObject.FindGameObjectsWithTag("ShrinkFood");
        foodToCollect = collectibles.Length;

        //FOCUS MASTERY 
        if (focus < 1)
        {
            restartButton.SetActive(false);
        }
        if (focus < 2)
        {
            removeAllButton.SetActive(false);
        }
        if (focus < 3)
        {
            pauseButton.SetActive(false);
        }
        if (focus >= 2)
        {
            playButton.GetComponent<RectTransform>().localPosition = playButtonFocus2Position;
        }

        //prepares buttons
        for (int i = 0; i < hintTableButtons.Length; i++)
        {
            if (wisdom < i)
                hintTableButtons[i].gameObject.SetActive(false);
            else if (wisdom == 0 && i == 0)
            {
                hintTableButtons[i].gameObject.SetActive(false);
            }
        }
        ShowHintTable(false);

        RecolorUI();
    }

    public void RecolorUI()
    {
        Color currentCol = starColors[level.data.obtainedStars];
        foreach (TextMeshProUGUI sext in colorableTexts)
        {
            sext.color = currentCol;
        }
        border.color = currentCol;

        foreach (ParticleSystem particles in glowParticles)
        {
            ParticleSystem.MainModule mod = particles.main;
            ParticleSystem.EmissionModule emma = particles.emission;
            emma.rateOverTime = 15 * level.data.obtainedStars;
            mod.startColor = currentCol;
        }
    }

    public void Pause()
    {
        paused = !paused;
        if (paused)
        {
            if (!pessimist)
            {
                filter.saturation.value = -40;
            }
            else
            {
                filter.saturation.value = 20;
            }
            Time.timeScale = 0;
        }
        else
        {
            if (!pessimist)
            {
                filter.saturation.value = 20;
            }
            else
            {
                filter.saturation.value = -100;
            }
            Time.timeScale = 1;
        }

    }

    public void ButtonRestart() => GameManager.Restart();

    public void ButtonNextLevel(bool backToMenu)
    {
        shouldLoadMenu = backToMenu;
        if (backToMenu)
        {
            MainMenuScript.goToLevelScreen = true;
        }
        blackFader.SetTrigger("FadeOut");
    }


    public static void EssenceCollected()
    {
        essencesToCollect--;
    }

    public static void ShrinkFoodCollected()
    {
        foodToCollect--;
    }

    public static bool CheckIfOver()
    {
        if (essencesToCollect == 0 && foodToCollect == 0)
            return true;
        return false; 
    }

    public void Mute(bool mute) => AudioManager.Mute(mute);

    public void MuteMusic(bool mute) => MusicManager.Mute(mute);

    public void ClickSound() => AudioManager.PlaySound(AudioManager.Sound.Click);

    public void SwitchHintTable()
    {
        showingHintTable = !showingHintTable;
        ShowHintTable(showingHintTable);
    }

    public void UpdateHintTableButtons(int toggle)
    {
        foreach (Button button in hintTableButtons)
        {
            button.interactable = true; 
        }

        if (toggle >= 0)
        { 
            hintTableButtons[toggle].interactable = false;
        }
    }

    public void PlaceFirstWaypoint()
    {
        SwitchHintTable();
        system.PlaceWayPoint(texts.firstWaypointToPlace);
    }

    public void ShowHintTable(bool show)
    {
        if (show)
        {
            if (PlayerStats.currentSkin != null)
            { 
                if (wisdom > 0)
                {
                    dragonName.text = PlayerStats.currentSkin.skinName + " can give you his...";
                }
                else
                {
                    dragonName.text = PlayerStats.currentSkin.skinName + "'s thoughts: ";
                }
            }
            UpdateText(lastSelectedText);
            AudioManager.PlaySound(AudioManager.Sound.InfoIn);
        }
        else
        { 
            AudioManager.PlaySound(AudioManager.Sound.InfoOut);
        }
        system.enabled = !show;
        blackBG.SetActive(show);
        hintTable.SetActive(show);
    }

    public void UpdateText(int which)
    {
        if (which == 0)
        {
            description.text = texts.thoughts;
        }
        else if (which == 1)
        {
            description.text = texts.smalHint;
        }
        else if (which == 2)
        {
            description.text = texts.hugeHint;
        }
        lastSelectedText = which;
        UpdateHintTableButtons(which);
    }
}
