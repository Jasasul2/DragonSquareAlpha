using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;

//Moves, grows and shrinks the dragon
public class Dragon : MonoBehaviour
{
    private int currentOrder = 46; 
    private DragonSkins skins;
    [HideInInspector] public Grid grid;
    public float speed = 4.8f; //n tiles per second 

    [HideInInspector]
    public float maxBodies = 0;
    public Rigidbody2D head;
    public List<Rigidbody2D> bodies;
    private List<SpriteRenderer> mainSprites;
    private List<SpriteRenderer> detailSprites; 

    public Dictionary<float, Vector2> startRotationOffset = new Dictionary<float, Vector2> {
        { 0f, new Vector2(-1, 0)},
        { 180f, new Vector2(1, 0)},
        { 270f, new Vector2(0, 1)},
        { 90f, new Vector2(0, -1)}
    };

    private const int BASE_SIZE = 3;  
    public int startSize = 3; //2 = head + tail, 3 = head + body + tail, n = head + (n - 2) * body + tail 
    private int size = 3;
    [HideInInspector]
    public bool
        dead = false,
        hasKyoki = false;

    private List<int> kyokiRotations = new List<int>();

    public GameObject 
        body, 
        tail, 
        bloodStain;

    public GameObject 
        bloodParticles,
        growParticles, 
        shrinkParticles, 
        pathObject; 

    private Vector2 gridOffset;

    private List<Vector2> pathPoints = new List<Vector2>(); // the list of targets 
    private int currentPathIndex;                           // index of path point which the head follows right now
    private Vector2 previousPathPoint;

    public int vision;  // Mastery 

    private void AssignBodyPartToArray(GameObject body, int detailChildIndex)
    {
        mainSprites.Add(body.GetComponent<SpriteRenderer>());
        detailSprites.Add(body.transform.GetChild(detailChildIndex).GetComponent<SpriteRenderer>());
    }

    private void Start()
    {
        vision = PlayerStats.selectedMasteryStages[PlayerStats.Masteries.Vision];
        hasKyoki = PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Kyoki];

        skins = GetComponent<DragonSkins>();

        gridOffset = Vector2.one * grid.cellsize / 2;

        mainSprites = new List<SpriteRenderer>();
        detailSprites = new List<SpriteRenderer>();

        size = BASE_SIZE;
        startSize = Mathf.Clamp(startSize, 2, 20);
        maxBodies += startSize;
        bodies = new List<Rigidbody2D>();
        bodies.Add(head);
        AssignBodyPartToArray(head.gameObject, 2);

        bodies.Add(body.GetComponent<Rigidbody2D>());
        AssignBodyPartToArray(body, 0);

        bodies.Add(tail.GetComponent<Rigidbody2D>());
        AssignBodyPartToArray(tail, 0);

        foreach (Rigidbody2D b in bodies) //snaps the dragon to the grid 
        {
            b.transform.parent = null; 
            Vector3 tempVector = grid.MatchToTheGrid(b.transform.position);
            b.transform.position = new Vector3(tempVector.x, tempVector.y, -1);
        }
        PreGrow();

        if (startSize + 1 == BASE_SIZE)
            Shrink(false, false); //removes the body so the dragon starts normal; 

        else if (startSize > BASE_SIZE)
        {
            for (int i = 0; i < startSize - 3; i++)
            {
                Grow(false, false);
            }
        }
    }

    public void SelectAPathToMoveTo(List<Vector2> wayPoints)
    {
        Vector2 targetPoint = UtilsClass.GetMouseWorldPosition();

        grid.GetXY(head.position, out int headX, out int headY);   //the true start point 
        Vector2 headPositionOnGrid = grid.GetWorldPosition(headX, headY) + gridOffset;

        grid.GetXY(targetPoint, out int targetX, out int targetY); //the true end point
        Vector2 trueEndPoint = grid.GetWorldPosition(targetX, targetY) + gridOffset;

        Vector2 previousVector = Vector2.zero;

        if (headPositionOnGrid == trueEndPoint)
            return;
        
        //getting the path in position vectors 
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (i == 0)
                pathPoints = Level.pathfinding.FindPath(head.position, wayPoints[i]);
            else
                pathPoints = pathPoints.Concat(Level.pathfinding.FindPath(wayPoints[i - 1], wayPoints[i])).ToList();
        }

        pathPoints.RemoveAt(0); //to not count head 

        //path signalization 
        if (vision == 1)
            ShowPathSignalization(true);
        else if (vision == 2)
            ShowPathSignalization(false);
        else if (vision == 3)
            ShowPathSignalization(false);

        //turns path points into vectors to count how long before the head reaches each one of them 
        float totalTime = 0f;
        float distance;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (i == 0)
            {
                distance = Vector2.Distance(pathPoints[i], head.position);
            }
            else
            {
                distance = Vector2.Distance(pathPoints[i], pathPoints[i - 1]);
            }
            totalTime += distance / speed;
            for (int bodyIndex = 0; bodyIndex < bodies.Count; bodyIndex++)
            {
                float addedTime = bodyIndex * 1 / speed; 
                StartCoroutine(LockToPosition(totalTime + addedTime, i, bodyIndex));
            }
        }
        Invoke("FinishMovement", totalTime);
        previousPathPoint = head.position;
        EnablePhysics();
        AudioManager.PlaySound(AudioManager.Sound.Movement);
    }

    private void FixedUpdate()
    {
        if (dead || GameManager.planningPhase || GameManager.gameHasEnded)
            return;

        Vector2 difference = pathPoints[currentPathIndex] - previousPathPoint;
        head.velocity = difference * speed / difference.magnitude; //* (2 - difference.magnitude);
        if(!hasKyoki)
            head.rotation = Mathf.Atan2(head.velocity.y, head.velocity.x) * Mathf.Rad2Deg;
        else
            head.rotation += kyokiRotations[0] * Time.fixedDeltaTime;

        for (int i = 1; i < bodies.Count; i++)
        {
            difference = bodies[i - 1].position - bodies[i].position;
            bodies[i].velocity = difference * speed / difference.magnitude; //* (2 - difference.magnitude);
            if (!hasKyoki)
                bodies[i].rotation = Mathf.Atan2(bodies[i].velocity.y, bodies[i].velocity.x) * Mathf.Rad2Deg;
            else
                bodies[i].rotation += kyokiRotations[i] * Time.fixedDeltaTime;
        }
    }

    public void FinishMovement() 
    {
        GameManager.OverCheck(true);
        StopMoving();
    }
     
    private IEnumerator InstantiatePathParticles(float timeDelay, Vector2 position, bool end = false)
    {
        yield return new WaitForSeconds(timeDelay);

        GameObject shit;
        if (!end)
        {
            shit = Instantiate(pathObject, position, Quaternion.identity);
        }
        else
        { 
            shit = Instantiate(pathObject, position, Quaternion.identity);
            shit.transform.localScale *= 1.25f;
        }
        if (vision == 3)
        {
            shit.GetComponent<Animator>().enabled = false;
        }
        else
        {
            Destroy(shit, 2f);
        }
    }

    private IEnumerator LockToPosition(float timeDelay, int vectorIndex, int bodyIndex)
    {

        yield return new WaitForSeconds(timeDelay);

        if (bodyIndex + vectorIndex < pathPoints.Count && !GameManager.gameHasEnded && !dead)
        { 
            bodies[bodyIndex].position = pathPoints[vectorIndex];

            if (bodyIndex == 0)
            {
                previousPathPoint = pathPoints[vectorIndex];
                currentPathIndex++;
            }
        }
    }

    private void StopMoving() //locks the dragon on place when he finishes 
    {
        if (dead) return; 

        foreach (Rigidbody2D body in bodies)
        {
            body.velocity = Vector2.zero;
            body.isKinematic = true;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private Vector2 Rotate(Vector2 vectorToRotate, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = vectorToRotate.x;
        float ty = vectorToRotate.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public void PreGrow() //called on the start of the level, pre instantiates all the bodies and tail; 
    {
        Vector2 bodyVector;
        Vector3 newPos;
        for (int i = 0; i < maxBodies - BASE_SIZE; i++)
        {
            GameObject nBody = bodies[BASE_SIZE + i - 1].gameObject;
            bodyVector = Rotate(Vector2.right, nBody.transform.rotation.eulerAngles.z);
            newPos = new Vector3(nBody.transform.position.x - bodyVector.x, nBody.transform.position.y - bodyVector.y, 1);
            nBody = Instantiate(nBody, newPos, rotation: nBody.transform.rotation);
            nBody.transform.parent = transform;
            bodies.Add(nBody.GetComponent<Rigidbody2D>());
            currentOrder -= 2;

            SpriteRenderer renderer = nBody.GetComponent<SpriteRenderer>();
            renderer.sprite = skins.currentSkin.baseSprites[1];
            mainSprites.Add(renderer);

            SpriteMask mask = nBody.GetComponent<SpriteMask>();
            mask.sprite = skins.currentSkin.baseSprites[1];

            SpriteRenderer renderer2 = nBody.transform.GetChild(0).GetComponent<SpriteRenderer>();
            renderer2.sprite = skins.currentSkin.midSprites[1];
            detailSprites.Add(renderer2);

            SpriteMask mask2 = nBody.transform.GetChild(0).GetComponent<SpriteMask>();
            mask2.sprite = skins.currentSkin.midSprites[1];

            renderer.sortingOrder = currentOrder;
            renderer2.sortingOrder = currentOrder;
            mask.frontSortingOrder = currentOrder + 2;
            mask2.frontSortingOrder = currentOrder + 2;
            mask.backSortingOrder = currentOrder + 1;
            mask2.backSortingOrder = currentOrder + 1;
            renderer.enabled = false;
            renderer2.enabled = false;
        }
        if (!hasKyoki)
            return;

        for (int i = 0; i < maxBodies; i++)
        {
            kyokiRotations.Add(Random.Range(-360, 360));
        }
    }

    public void Grow(bool particles, bool sound = true)
    {
        if (dead || GameManager.gameHasEnded)
            return;

        if(sound)
            AudioManager.PlaySound(AudioManager.Sound.Grow);

        GameObject lBody = bodies[size - 1].gameObject;
        ChangeVisuals(size - 1, 1);

        Vector2 bodyVector = Rotate(Vector2.right, lBody.transform.rotation.eulerAngles.z);
        Vector3 newPos = new Vector3(lBody.transform.position.x - bodyVector.x, lBody.transform.position.y - bodyVector.y, 1);
        GameObject nBody = bodies[size].gameObject;

        nBody.GetComponent<SpriteRenderer>().enabled = true;
        nBody.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        nBody.transform.position = newPos;
        nBody.transform.rotation = bodies[size - 1].transform.rotation; 

        if (particles)
            Instantiate(growParticles, nBody.transform.position, nBody.transform.rotation);

        size++;
        ChangeVisuals(size - 1, 2); 
    }

    public void Shrink(bool particles = true, bool sound = true, bool blood = false)
    {
        if (dead || GameManager.gameHasEnded)
            return;

        if (size >= 3) //has more than just head
        {
            GameObject nBody = bodies[size - 1].gameObject;

            if (sound)
                AudioManager.PlaySound(AudioManager.Sound.Shrink);

            if (blood)
                Instantiate(bloodParticles, nBody.transform.position, Quaternion.identity);

            if (particles)
                Instantiate(shrinkParticles, nBody.transform.position, nBody.transform.rotation);

            nBody.GetComponent<SpriteRenderer>().enabled = false;
            nBody.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            size--;
            nBody = bodies[size - 1].gameObject;
            ChangeVisuals(size - 1, 2);
        }
        else //only head and tail remains -- kills the dragon 
        {
            GameObject nBody = bodies[size - 1].gameObject;
            nBody.GetComponent<SpriteRenderer>().enabled = false;
            nBody.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            Die();
        }
    }

    private void ChangeVisuals(int bodyIndex, int part)
    {
        mainSprites[bodyIndex].sprite = skins.currentSkin.baseSprites[part];

        SpriteMask mask = mainSprites[bodyIndex].GetComponent<SpriteMask>();
        mask.sprite = skins.currentSkin.baseSprites[part];

        detailSprites[bodyIndex].sprite = skins.currentSkin.midSprites[part];

        SpriteMask mask2 = detailSprites[bodyIndex].GetComponent<SpriteMask>();
        mask2.sprite = skins.currentSkin.midSprites[part];
    }

    public void UpdateColors(Color primary, Color secondary)
    {
        foreach (SpriteRenderer sprite in mainSprites)
        {
            sprite.color = primary;
        }
        foreach (SpriteRenderer sprite in detailSprites)
        {
            sprite.color = secondary;
        }
    }

    private void EnablePhysics()
    {
        foreach (Rigidbody2D body in bodies)
        {
            if (body.gameObject.activeSelf)
            {
                body.isKinematic = false;
                body.constraints = RigidbodyConstraints2D.None; 
            }
        }
    }

    public void Die(bool lava = false)
    {
        if (dead)
            return;

        AudioManager.PlaySound(AudioManager.Sound.Die);
        head.GetComponent<DragonMouth>().enabled = false; //so it can't eat anymore 

        Instantiate(bloodParticles, head.position, Quaternion.identity);

        GameObject stain = Instantiate(bloodStain, head.position, Quaternion.Euler(GetSpreadFloat(), GetSpreadFloat(), head.rotation));
        stain.transform.parent = head.transform;

        EnablePhysics();
        foreach (Rigidbody2D body in bodies)
        {
            if (body.gameObject.activeSelf && body.GetComponent<SpriteRenderer>().enabled) 
            {
                body.gameObject.layer = 8;
                body.AddForce(Vector2.up, ForceMode2D.Impulse);
                Instantiate(bloodParticles, body.transform.position, Quaternion.identity);
                stain = Instantiate(bloodStain, body.position, Quaternion.Euler(GetSpreadFloat(), GetSpreadFloat(), body.rotation));
                stain.transform.parent = body.transform;
                stain.GetComponent<SpriteRenderer>().sortingOrder = body.GetComponent<SpriteMask>().frontSortingOrder - 1;
            }
        }
        head.gameObject.layer = 8;
        dead = true;
        GameManager.EraseEnemies();
        Invoke("Restart", 1.44f);
    }

    public void ShowPathSignalization(bool half)
    {
        bool end = false;
        int x = pathPoints.Count;
        if (half)
        {
            x = pathPoints.Count / 2;
        }
        for (int i = 0; i < x; i++)
        {
            if (i == pathPoints.Count - 1)
                end = true;
            StartCoroutine(InstantiatePathParticles(i * 0.05f, pathPoints[i], end));
        }
    }

    private float GetSpreadFloat() => Random.Range(-0.5f, 0.5f);

    private void Restart() => GameManager.Restart();
}