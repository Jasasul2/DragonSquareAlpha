using UnityEngine;
using TMPro;
using UnityEngine.UI; 


public class BlackFaderNextLevel : MonoBehaviour
{
    public TextMeshProUGUI lName;

    public void NextLevel()
    {
        GameManager.NextLevel();
    }

    public void SetName()
    {
        lName.text = GameObject.Find("Level").GetComponent<Level>().thisLevel.lName;
    }
}
