using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using CodeMonkey.Utils;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlanSystem : MonoBehaviour
{
    private static PlanSystem instance;
    private List<Food> foodToActivate;

    public GameObject 
        wayPointPref, 
        particlesPref;
    [HideInInspector] public Level level;
    private int 
        maxWayPoints, 
        currentWayPoint = 1;
    public List<GameObject> placedPoints;
    public List<Vector2> placedPointsCoordinates;
    private Grid grid;

    private Vector2 gridOffset;
    public Color[] colors;

    //postProcessing
    public Volume volume;
    private ColorAdjustments filter;
    public Animator UIswitch;

    public TMPro.TextMeshProUGUI wText;
    public Button startButton, removeAllButton;

    private int logic; //mastery
    private int memory;
    private int focus;

    public GameObject waypointSpiritPref;
    public List<GameObject> currentWaypointSpirits; 
    public WaypointSpirits lastRoundSpirits; 

    private void Start()
    {
        memory = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Memory];
        logic = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Logic];
        focus = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Focus];

        instance = this;
        level = GameObject.Find("Level").GetComponent<Level>();
        maxWayPoints = level.maxWayPoints;
        grid = Level.pathfinding.GetGrid();

        gridOffset = Vector2.one * grid.cellsize / 2;

        volume.profile.TryGet(out filter);
        UIswitch.enabled = false;

        UpdateText();

        foodToActivate = new List<Food>();
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            foodToActivate.Add(food.GetComponent<Food>());
        }
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("ShrinkFood"))
        {
            foodToActivate.Add(food.GetComponent<Food>());
        }
        if (memory < 3)
        {
            removeAllButton.interactable = false; 
        }
        InstantiateWaypointSpirits();
    }
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 targetPoint = UtilsClass.GetMouseWorldPosition();
            PlaceWayPoint(targetPoint);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RemoveLastWaypoint();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (focus >= 2)
            { 
                RemoveAllWaypoints();
            }
        }
    }

    public void PlaceWayPoint(Vector2 targetPoint, bool sound = true)
    {
        if (currentWayPoint == maxWayPoints + 1)
        {
            AudioManager.PlaySound(AudioManager.Sound.CantPlace);
            return;
        }

        if (targetPoint.y > 3.85f)
            return;

        grid.GetXY(targetPoint, out int targetX, out int targetY); //the true end point
        Vector2 currentPos = new Vector2(targetX, targetY);

        if (grid.CheckPlacement(targetX, targetY))
        {
            currentPos = grid.GetWorldPosition(targetX, targetY) + gridOffset;

            if (currentPos.y > Grid.topLimit)
                return;

            foreach (Vector2 vector in placedPointsCoordinates)
            {
                if (currentPos == vector)
                {
                    if(sound)
                        AudioManager.PlaySound(AudioManager.Sound.CantPlace);
                    return;
                }

            }

            GameObject waypoint = Instantiate(wayPointPref, currentPos, Quaternion.identity);
            if (placedPoints.Count == 0)
            {
                waypoint.GetComponent<WayPoint>().stats = new WayPoint.WayPointStats(true, null);
            }
            else
            {
                waypoint.GetComponent<WayPoint>().stats = new WayPoint.WayPointStats(false, placedPoints[placedPoints.Count - 1]);
            }
            //LOGIC MASTERY 
            TMPro.TextMeshPro text = waypoint.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            Color color = GetColor();
            color.a = 0.4f;
            waypoint.transform.GetChild(1).GetComponent<SpriteRenderer>().color = color;

            Color color2 = GetColor();
            text.text = currentWayPoint.ToString();
            color2.a = 0.75f;
            text.color = color2;

            if (logic < 2)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                if (logic == 2) //needs to turn of the last waypoint number if it isn't waypoint one 
                {
                    if (currentWayPoint > 2)
                    {
                        placedPoints[placedPoints.Count - 1].transform.GetChild(0).gameObject.SetActive(false);
                    }

                }
            }

            SpawnParticles(currentPos);

            placedPoints.Add(waypoint);
            placedPointsCoordinates.Add(currentPos);
            currentWayPoint += 1;

            if (currentWayPoint == 2)
            { 
                startButton.interactable = true;
                removeAllButton.interactable = true;
            }

            UpdateText();
            if(sound)
                AudioManager.PlaySound(AudioManager.Sound.Place);
        }
        else
        {
            if(sound)
                AudioManager.PlaySound(AudioManager.Sound.CantPlace);
        }

    }

    public Color GetColor(int add = 0, bool useWaypointIndex = true, int whatIndexThen = 0)
    {
        Color col;
        if (logic < 1)
        {
            col = PlayerStats.secondaryColor;
        }
        else
        {
            int index;
            if (useWaypointIndex)
            {
                index = currentWayPoint - 1 + add;
            }
            else
            { 
                index = whatIndexThen;
            }
            while (index >= colors.Length)
            { 
                index = index - (colors.Length);
                Debug.Log(index);
            }
            col = colors[index];
        }
        return col;
    }

    private void RemoveLastWaypoint(bool sound = true)
    {
        if (currentWayPoint == 1)
        {
            if(sound)
                AudioManager.PlaySound(AudioManager.Sound.CantPlace);
            return;
        }

        GameObject latestPoint = placedPoints[placedPoints.Count - 1];
        placedPoints.RemoveAt(placedPoints.Count - 1);
        SpawnParticles(latestPoint.transform.position, -1);

        Destroy(latestPoint);
        placedPointsCoordinates.RemoveAt(placedPointsCoordinates.Count - 1);
        currentWayPoint -= 1;

        if (currentWayPoint == 1)
        { 
            startButton.interactable = false;
            removeAllButton.interactable = false; 
        }

        if (logic == 2 && currentWayPoint > 1) //needs to turn on the last waypoint number if it isn't waypoint one 
        {
            placedPoints[placedPoints.Count - 1].transform.GetChild(0).gameObject.SetActive(true);
        }

        UpdateText();
        if(sound)
            AudioManager.PlaySound(AudioManager.Sound.Remove);
    }

    public void RemoveAllWaypoints()
    {
        if (currentWayPoint == 1)
        {
            AudioManager.PlaySound(AudioManager.Sound.CantPlace);
            return;
        }
        AudioManager.PlaySound(AudioManager.Sound.Remove);
        for (int i = 0; i < maxWayPoints; i++)
        {
            RemoveLastWaypoint(false);
        }
    }

    private void SpawnParticles(Vector3 position, int add = 0)
    {
        GameObject particles = Instantiate(particlesPref, position, Quaternion.identity);
        ParticleSystem.MainModule mod = particles.GetComponent<ParticleSystem>().main;
        mod.startColor = GetColor(add);
    }

    public void EndPlanning()
    {
        Time.timeScale = 1;
        GameObject.Find("Dragon").GetComponent<Dragon>().SelectAPathToMoveTo(placedPointsCoordinates);

        filter.contrast.value = 6;
        if (!PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Pessimist])
        { 
            filter.saturation.value = 20;
        }
        UIswitch.enabled = true;

        foreach (Food food in foodToActivate)
        {
            food.EnableAnimation();
        }

        SaveWaypointSprits();


        level.CalculateStarCount(currentWayPoint - 1);
        GameManager.planningPhase = false;
        enabled = false;
    }

    private void UpdateText()
    {
        wText.text = "Waypoints: " + (currentWayPoint - 1).ToString() + "/" + maxWayPoints.ToString();
    }

    public static bool CanPlace()
    {
        if (instance.currentWayPoint - 1 != instance.maxWayPoints)
            return true;
        return false;
    }

    public void InstantiateWaypointSpirits()
    {
        string loadString = SaveSystem.LoadSpiritData();
        if (loadString != null)
        {
            lastRoundSpirits = JsonUtility.FromJson<WaypointSpirits>(loadString);
            if (memory == 3)
            {
                Debug.Log("placing waypoints");
                for (int i = 0; i < lastRoundSpirits.waypoints.Count; i++)
                {
                    PlaceWayPoint(lastRoundSpirits.waypoints[i], false);
                }
            }
            else if (memory == 2 || memory == 1)
            {
                for (int i = 0; i < lastRoundSpirits.waypoints.Count; i++)
                {
                    GameObject spirit = Instantiate(waypointSpiritPref, lastRoundSpirits.waypoints[i], Quaternion.identity);
                    Color col = GetColor(0, false, i);
                    spirit.transform.GetChild(0).GetComponent<SpriteRenderer>().color = col;
                    Destroy(spirit, 12f);
                }
            }
        }
    }

    public void SaveWaypointSprits()
    {
        List<Vector2> spirits = new List<Vector2>();
        int count = 0;
        if (memory == 1)
        {
            count = 3;
        }
        else if (memory == 2 || memory == 3)
        {
            count = placedPointsCoordinates.Count;
        }
        count = Mathf.Clamp(count, 0, placedPointsCoordinates.Count);
        for (int i = 0; i < count; i++)
        {
            spirits.Add(placedPointsCoordinates[i]);
        }
        lastRoundSpirits = new WaypointSpirits(spirits);
        string save = JsonUtility.ToJson(lastRoundSpirits);
        SaveSystem.SaveSpiritData(save);

    }

    public class WaypointSpirits
    {
        public List<Vector2> waypoints;

        public WaypointSpirits(List<Vector2> waypoints)
        {
            this.waypoints = waypoints;
        }
    }

}
