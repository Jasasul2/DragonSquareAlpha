using UnityEngine;

public class DragonMouth : MonoBehaviour
{
    private Dragon dragon;
    public GameObject lavaParticles;

    private void Start() => dragon = transform.parent.GetComponent<Dragon>();

    public void Feed(int newFoodEaten)
    {
        for (int i = 0; i < newFoodEaten; i++)
        {
            dragon.Grow(true);
        }
        GameManager.EssenceEaten();
    }

    public void ShrinkFeed(int newShrinkFoodEeaten)
    {
        for (int i = 0; i < Mathf.Abs(newShrinkFoodEeaten); i++)
        {
            dragon.Shrink();
        }
        GameManager.ShrinkFoodEaten();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
            return;
        if (collision.CompareTag("Food"))
        {
            Food food = collision.GetComponent<Food>();
            Feed(food.foodValue);
            food.Dissapear();
        }
        else if (collision.CompareTag("ShrinkFood"))
        {
            Food food = collision.GetComponent<Food>();
            ShrinkFeed(food.foodValue);
            food.Dissapear();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            IEnemyInterface enemy = collision.collider.GetComponent<IEnemyInterface>();
            enemy.DamageDragon(dragon);
        }
        else if (collision.collider.CompareTag("Lava"))
        {
            Instantiate(lavaParticles, collision.transform.position, Quaternion.identity);
            if (GameManager.gameHasEnded)
                return; 
            dragon.Die();
        }
    }
}

