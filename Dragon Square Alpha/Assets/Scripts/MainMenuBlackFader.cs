using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBlackFader : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        if (SceneManager.sceneCountInBuildSettings > MainMenuScript.levelToStart)
            SceneManager.LoadScene(MainMenuScript.levelToStart);
    }
}
