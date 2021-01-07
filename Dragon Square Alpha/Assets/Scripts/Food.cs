using UnityEngine;
using System.Collections.Generic; 

public class Food : MonoBehaviour
{ 
    public int foodValue = 1; //negative values will shrink the dragon 
    public GameObject particles;
    private TMPro.TextMeshPro valueText;

    private FoodAnimation anim; 
    public bool wantToStick = true;
    private string type = "Shrink Food", description = "Makes the dragon shorter";

    public bool spawnWhenEeaten = false;
    public Color spawnColor;
    public GameObject spawnParticles; 
    public List<GameObject> toSpawnWhenEaten; 
    
    private void Start()
    {
        anim = GetComponent<FoodAnimation>();
        int value = Mathf.Abs(foodValue);

        if (wantToStick)
            Stick();

        if (value > 1)
        {
            float multiplier = 0.12f * (value - 1);
            transform.localScale += new Vector3(multiplier, multiplier, 1);
        }

        if (foodValue > 0)
        { 
            type = "Essence";
            description = "Makes the dragon longer";
        }
        AssignInfoTable foodTable = GetComponent<AssignInfoTable>();
        foodTable.information = new List<string> { type, "Weight : " + value.ToString(), description };

        if (spawnWhenEeaten)
        {
            GetComponent<SpriteRenderer>().color = spawnColor;
            SpawnStuff(false);
        }
    }

    public void SpawnStuff(bool spawn)
    {
        foreach (GameObject item in toSpawnWhenEaten)
        {
            item.SetActive(spawn);
            if (spawn)
                Instantiate(spawnParticles, item.transform.position, Quaternion.identity);
        }
    }

    public void Dissapear()
    {
        if(particles != null)
            Instantiate(particles, transform.position, Quaternion.identity);
        if (spawnWhenEeaten)
        {
            AudioManager.PlaySound(AudioManager.Sound.InfoIn);
            SpawnStuff(true);
        }
        Destroy(gameObject);
    }

    public void Stick()
    {
        Grid grid = Level.pathfinding.GetGrid();
        Vector3 tempVector = grid.MatchToTheGrid(transform.position);
        transform.position = new Vector3(tempVector.x, tempVector.y, -1);
    }

    public void EnableAnimation()
    {
        if (anim != null)
            anim.enabled = true; 
    }
}

