using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonCollider : MonoBehaviour
{
    public LayerMask solidBlocks;
    private ContactFilter2D contacts = new ContactFilter2D();
    private Collider2D[] results = new Collider2D[5]; 
    private Collider2D coll;
    public int spawnLayer, collidingLayer;
    private bool colliding = false;

    private void Awake()
    {
        contacts.layerMask = solidBlocks;
        contacts.useLayerMask = true;
        coll = GetComponent<Collider2D>();

        gameObject.layer = spawnLayer;
        UnCollide();
    }

    private bool GetOverlap()
    {
        if (coll.OverlapCollider(contacts, results) > 0)
        {
            return true; 
        }
        return false; 
    }

    private void UnCollide() //callled after se zbraò pøestane vyskytovat ve zdi 
    {
        colliding = GetOverlap();
        if (!colliding)
        {
            gameObject.layer = collidingLayer;
            enabled = false; 
        }
    }

    private void Update()
    {
        if (colliding)
            UnCollide();
    }
}
