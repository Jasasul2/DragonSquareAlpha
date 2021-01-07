using UnityEngine;

public class FoodAnimation : MonoBehaviour
{
    public float spread;
    private Vector3[] positions = new Vector3[2];
    public float speed;

    private void Start()
    {  
        positions[0] = new Vector3(transform.position.x, transform.position.y + spread, -1);
        positions[1] = new Vector3(transform.position.x, transform.position.y - spread, -1);
    }

    private void Update() => transform.position = Vector3.Lerp(new Vector3(transform.position.x, positions[0].y, -1), 
        new Vector3(transform.position.x, positions[1].y, -1), (Mathf.Sin(speed * Time.time) + 1f) / 2f);
}
