using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public LevelAttributes thisLevel;
    public LevelData data = new LevelData(false, 80, 0);

    public string levelName;
    public int maxWayPoints = 12;
    public static Pathfinding pathfinding;
    private Dragon dragon;

    public LevelLayout layout;

    public void Start()
    {
        Load();
        pathfinding = new Pathfinding(17, 9);
        dragon = GameObject.Find("Dragon").GetComponent<Dragon>();
        dragon.grid = pathfinding.GetGrid();
        maxWayPoints = thisLevel.wayPointLimits[2];
    }

    public void Save()
    {
        string saveString = JsonUtility.ToJson(data);
        SaveSystem.SaveLevelData(saveString, SceneManager.GetActiveScene().buildIndex);
    }

    public void Load()
    {
        string json = SaveSystem.LoadLevelData(SceneManager.GetActiveScene().buildIndex);
        if (json != null)
        { 
            data = JsonUtility.FromJson<LevelData>(json);
        }
    }

    public void CalculateStarCount(int usedWaypoints)
    {
        GameManager.waypointsUsed = usedWaypoints;
        int reverse = 0;
        for (int i = 1; i < thisLevel.wayPointLimits.Length; i++)
        {
            if (usedWaypoints >= thisLevel.wayPointLimits[i])
            {
                reverse++;
            }
        }
        GameManager.numberOfStars = 3 - reverse;  
    }
}

public class LevelData
{
    public bool completed;
    public int waypointHighscore;
    public int obtainedStars; //between 0 and 3 including

    public LevelData(bool completed, int waypointHighscore, int obtainedStars)
    {
        this.completed = completed;
        this.waypointHighscore = waypointHighscore;
        this.obtainedStars = obtainedStars;

    }
}