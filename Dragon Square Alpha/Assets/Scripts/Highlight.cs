using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour
{
    public SpriteRenderer highlight;
    public GameObject letter, shadow; 
    public bool showing = false;
    public Vector3 defaultScale;

    private void Start()
    {
        highlight.color = Color.white;
        Show(false);
        transform.parent = null;
        transform.localScale = defaultScale;
    }

    public void Show(bool show)
    {
        if (show)
        {
            showing = true;
            highlight.enabled = true;
            shadow.SetActive(true);
            letter.SetActive(true);
        }
        else
        {
            showing = false;
            highlight.enabled = false;
            shadow.SetActive(false);
            letter.SetActive(false);
        }

    }

}
