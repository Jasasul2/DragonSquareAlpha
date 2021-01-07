using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : MonoBehaviour, IEnemyInterface
{
    public NameGenerator.MonsterTypes type; 

    public float speed, enragedSpeed, currentSpeed;
    public int damage; //how much will collision with this enemy shorten the dragon 

    private Transform dragon;
    private Rigidbody2D rb; 

    private SpriteRenderer spriteRenderer;
    public Sprite normalSprite, enragedSprite;
    public GameObject deathParticles, rageParticles;

    public string description =
        "Follows the dragon, gets angry when it's near him";

    private bool enraged = false;

    public bool wantToStick = true;

    private void Start()
    {
        currentSpeed = speed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        dragon = GameManager.DragonHead;

        if (wantToStick)
            Stick();

        if (damage > 1)
        {
            float multiplier = 0.12f * damage;
            transform.localScale += new Vector3(multiplier, multiplier, 1);
        }

        AssignInfoTable foodTable = GetComponent<AssignInfoTable>();
        foodTable.information = new List<string> {NameGenerator.GetEnemyName(type),
            "Damage : " + damage.ToString(),
            "Speed : " + speed.ToString() + "cm/s",
            "Enraged speed : " + enragedSpeed.ToString() + "cm/s",
            description
        };
    }

    private void FixedUpdate()
    {
        Vector2 distance = dragon.position - transform.position;
        distance.Normalize();

        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg + 180f;
        rb.MoveRotation(angle);

        rb.velocity = currentSpeed * distance * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Head") && !enraged)
        {
            enraged = true;
            currentSpeed = enragedSpeed;
            Instantiate(rageParticles, transform.position, Quaternion.Euler(0, 0, rb.rotation));
            spriteRenderer.sprite = enragedSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Head") && enraged)
        {
            enraged = false;
            currentSpeed = speed; 
            spriteRenderer.sprite = normalSprite;
        }
    }

    public void DamageDragon(Dragon dragon)
    {
        for (int i = 0; i < damage; i++)
        {
            dragon.Shrink(blood: true);
        }
        Dissapear();
    }

    public void Dissapear()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Stick()
    {
        Grid grid = Level.pathfinding.GetGrid();
        Vector3 tempVector = grid.MatchToTheGrid(transform.position);
        transform.position = new Vector3(tempVector.x, tempVector.y, -1);
    }
}
