using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Highlight highlight;
    private AssignInfoTable table; 

    public int child = 0;

    private void Start()
    {
        highlight = transform.GetChild(0).GetComponent<Highlight>();
        table = GetComponent<AssignInfoTable>();
    }

    private void OnMouseEnter()
    {
        if (GameManager.planningPhase && !UIManager.showingHintTable)
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
                table.SwitchState();
            }
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.planningPhase)
        {
            highlight.Show(false);
            if (table.showing)
                table.SwitchState();
        }
    }
}
