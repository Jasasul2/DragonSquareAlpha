using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadow : MonoBehaviour
{
    public Color shadowColor = Color.black;
    public Vector3 offset;

    [HideInInspector]
    public GameObject shadow; 
    private void Start()
    {
        shadow = new GameObject("Shadow");
        offset *= transform.localScale.x; 

        shadow.transform.position = transform.position + offset;
        shadow.transform.rotation = Quaternion.identity;
        shadow.transform.localScale = transform.localScale; 

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();

        shadowRenderer.sprite = renderer.sprite;
        shadowRenderer.color = shadowColor;

        shadowRenderer.sortingOrder = -1;
    }

    private void OnDestroy()
    {
        Destroy(shadow);
    }

    private void LateUpdate()
    {
        shadow.transform.position = transform.position + offset; 
        shadow.transform.rotation = transform.rotation; 
    }
}
