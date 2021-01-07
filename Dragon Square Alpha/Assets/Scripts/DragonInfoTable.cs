using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonInfoTable : MonoBehaviour
{
    private Highlight highlight;

    public UIManager ui;

    private void Start()
    {
        highlight = transform.GetChild(0).GetComponent<Highlight>();
    }

    private void OnMouseEnter()
    {
        if (GameManager.planningPhase)
        {
            highlight.Show(true);
        }
    }

    private void OnMouseOver()
    {
        if (highlight.showing)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ui.SwitchHintTable();
            }
        }
    }

    private void OnMouseExit()
    {
        if (highlight.showing)
            highlight.Show(false);
    }
}