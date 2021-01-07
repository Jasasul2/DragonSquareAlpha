using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignInfoTable : MonoBehaviour
{ 
    public Vector3 offset = new Vector3(0, 1.7f, 1);

    public GameObject infoTable;

    public Color bgColor, nameColor, subColor, thirdColor;

    public List<string> information; 

    public float yLimit = 2.6f, xLimit = 7f;

    public bool showing = true;

    private GameObject spawnedTable;

    private void Start()
    {
        transform.parent = null;
        if (transform.position.y + offset.y >= yLimit)
            offset.y -= 2f;

        else if (transform.position.y + offset.y <= yLimit * -1)
            offset.y += 2f; 

        if (transform.position.x > xLimit)
            offset.x -= 1.25f;

        else if(transform.position.x < xLimit * -1)
            offset.x += 1.25f;

        spawnedTable = Instantiate(infoTable, transform.position + offset, Quaternion.identity);
        spawnedTable.GetComponent<SpriteRenderer>().color = bgColor; 
        
        for (int i = 0; i < information.Count; i++)
        {
            GameObject text = spawnedTable.transform.GetChild(i).gameObject;
            text.GetComponent<TMPro.TextMeshPro>().text = information[i];
            if (i == 0) //name is always first 
            {
                text.GetComponent<TMPro.TextMeshPro>().color = nameColor;
            }
            else if (i == spawnedTable.transform.childCount - 2 && thirdColor != null)
            {
                text.GetComponent<TMPro.TextMeshPro>().color = thirdColor;
            }
            else
            { 
                text.GetComponent<TMPro.TextMeshPro>().color = subColor;
            }
        }

        spawnedTable.SetActive(false);
        GameManager.instance.blackUnderTable.SetActive(false);
    }

    public void SwitchState()
    {
        showing = !showing;
        if (showing) //showing 
        {
            spawnedTable.SetActive(true);
            GameManager.instance.blackUnderTable.SetActive(true);
            AudioManager.PlaySound(AudioManager.Sound.InfoIn);
        }
        else
        {
            spawnedTable.SetActive(false);
            GameManager.instance.blackUnderTable.SetActive(false);
            AudioManager.PlaySound(AudioManager.Sound.InfoOut);
        }
    }
}
