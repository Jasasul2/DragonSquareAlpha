using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelX", menuName = "Level", order = 2)]
public class LevelAttributes : ScriptableObject
{
    public string lName, description;
    public Sprite lImage;
    public int[] wayPointLimits = new int[3]; // one for each star rating - always gets at least 1 star
    public int[] dragoinRewards = new int[3]; // one for each star rating - 10 30% 60% of the total reward 
    public string thoughts, smalHint, hugeHint;
    public Vector2 firstWaypointToPlace;
}
