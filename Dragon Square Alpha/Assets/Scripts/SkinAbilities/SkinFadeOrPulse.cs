using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinFadeOrPulse : MonoBehaviour
{
    public bool 
        fading = false, 
        pulsing = false, 
        trail = false, 
        paint = false;

    public Color 
        primary, 
        secondary, 
        current1, 
        current2; 

    public SpriteRenderer[] bases;
    public SpriteRenderer[] details;
    public SpriteRenderer head;

    private Dragon dragon;

    private DragonSkins dragonSkins; 

    public void Start()
    {
        current1 = primary;
        current2 = secondary;

        dragon = GetComponent<Dragon>();
        dragonSkins = GetComponent<DragonSkins>();

        trail = PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Trail];
        paint = PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Paint];
    }

    public void Update()
    {
        if (GameManager.planningPhase)
            return;
        LerpColors();
    } 

    public void LerpColors()
    {
        if (pulsing)
        { 
            current1 = Color.Lerp(primary, secondary, Mathf.PingPong(Time.time, 1));
            current2 = Color.Lerp(secondary, primary, Mathf.PingPong(Time.time, 1));
        }
        if (fading)
        {
            current1.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time / 2, 1));
            current2.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time / 2, 1));
        }
        if (trail)
        {
            dragonSkins.UpdateParticlesColor(true, current2);
        }
        if (paint)
        {
            dragonSkins.UpdateParticlesColor(false, current2);
        }
        dragon.UpdateColors(current1, current2);
    }
}
