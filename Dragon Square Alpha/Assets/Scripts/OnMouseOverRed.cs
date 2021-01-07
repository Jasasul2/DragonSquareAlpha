using UnityEngine;

public class OnMouseOverRed : MonoBehaviour
{
    public SpriteRenderer tile;

    private void Awake() => tile = GetComponent<SpriteRenderer>();

    private void OnMouseEnter()
    {
        if (GameManager.planningPhase)
            tile.color = Color.red;
    }

    private void OnMouseExit()
    {
        tile.color = Color.white;
        if (!GameManager.planningPhase)
            enabled = false;
    }
}
