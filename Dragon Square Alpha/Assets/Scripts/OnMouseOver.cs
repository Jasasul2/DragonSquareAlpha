using UnityEngine;

public class OnMouseOver : MonoBehaviour
{
    public SpriteRenderer tile;

    private void Awake() => tile = GetComponent<SpriteRenderer>();

    private void OnMouseEnter()
    {
        if (GameManager.planningPhase)
        {
            if (PlanSystem.CanPlace())
                tile.color = Color.green;
            else
                tile.color = Color.red; 
        }
    }

    private void OnMouseExit()
    {
        tile.color = Color.white;
        if (!GameManager.planningPhase)
            enabled = false;
    }
}
