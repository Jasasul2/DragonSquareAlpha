using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public static bool goToLevelScreen = false;

    public Animator cameraAnimator, mainMenuAnimator;
    public static int levelToStart = 1;
    public static MainMenuScript instance;

    public GameObject resetMenu, blackResetBG;

    public GameObject shopInfoBox, shopInfoBG;

    public GameObject creditBox, settingsBox, creditBG;

    private void Awake()
    {
        Time.timeScale = 1;
        instance = this;
        if (goToLevelScreen)
        {
            ToLevelScreen();
        }
        goToLevelScreen = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) EndGame();
    }

    public void ToLevelScreen() => cameraAnimator.SetBool("ToLevelScreen", true);

    public void FromLevelScreen() => cameraAnimator.SetBool("ToLevelScreen", false);

    public void ToShop() => cameraAnimator.SetBool("ToShop", true);

    public void FromShop()
    {
        cameraAnimator.speed = 1;
        cameraAnimator.SetBool("ToShop", false);
    } 

    public void ToShopFromMenu()
    {
        cameraAnimator.speed = 2;
        ToLevelScreen();
        Invoke("ToShop", 0.625f);
    }

    public static void StartStartingGame(int id)
    {
        levelToStart = id;
        instance.mainMenuAnimator.gameObject.SetActive(true);
        instance.mainMenuAnimator.gameObject.transform.SetAsLastSibling();
        instance.mainMenuAnimator.SetTrigger("FadeToBlack");
    }

    public void ClickSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.Click);
    }

    public void EndGame() => Application.Quit();

    public void ShowResetMenu(bool show)
    {
        resetMenu.SetActive(show);
        blackResetBG.SetActive(show);
    }

    public void ResetProgress()
    {
        SaveSystem.ResetProgress();
        StartStartingGame(0);   
    }

    public void ShowShopInfo(bool show)
    {
        shopInfoBG.SetActive(show);
        shopInfoBox.SetActive(show);
    }

    public void ShowCredits(bool show)
    {
        creditBG.SetActive(show);
        creditBox.SetActive(show);
    }

    public void ShowSettings(bool show)
    {
        creditBG.SetActive(show);
        settingsBox.SetActive(show);
    }
}
