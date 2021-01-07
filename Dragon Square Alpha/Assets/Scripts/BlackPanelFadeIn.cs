using UnityEngine;
using UnityEngine.UI; 

public class BlackPanelFadeIn : MonoBehaviour
{
    private Image image;
    private Color col; 
    public float treshold, fadeSpeed; 

    private void Awake()
    {
        image = GetComponent<Image>();
        col = image.color;
        col.a = 0;
        image.color = col; 
    }

    private void Update()
    {
        col.a += fadeSpeed * Time.deltaTime;
        image.color = col; 
        if (col.a >= treshold)
        {
            enabled = false; 
        }
    }

}
