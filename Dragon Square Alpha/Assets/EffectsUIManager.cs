using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 

public class EffectsUIManager : MonoBehaviour
{
    public GameObject 
        trailParticlesObject, 
        paintParticlesObject; 

    [SerializeField]
    public ParticleSystem 
        trailParticlesP, 
        paintParticlesP;

    private ParticleSystem.MainModule 
        trailParticles, 
        paintParticles; 

    public List<UIEffect> allEffects;
    public List<GameObject> effectPages;
    public int currentPage = 0;

    public Image[] dragonBases;
    public Image[] dragonDetails;
    public Image dragonFace;

    public Rigidbody2D[] dragonBodies;
    private int[] dragonBodyRotationSpeeds = new int[3]; 

    private bool
        hasTrail = false,
        hasPaint = false,
        hasPulse = false,
        hasFade = false,
        hasKyoki = false,
        hasEnergy = false,
        hasReversed = false,
        hasPessimist = false;

    public Volume volume;
    private ColorAdjustments filter;

    private Color 
        primary, 
        secondary, 
        color1, 
        color2;

    private void Start()
    {
        volume.profile.TryGet(out filter);
        PrepareVisuals();
        gameObject.SetActive(false);
    }

    public void PrepareVisuals(bool shit = true)
    {
        primary = PlayerStats.mainColor;
        secondary = PlayerStats.secondaryColor;
        for (int i = 0; i < dragonBases.Length; i++)
        {
            Image sprite = dragonBases[i];
            sprite.color = primary;
            sprite.sprite = PlayerStats.currentSkin.baseSprites[i];
        }
        for (int i = 0; i < dragonDetails.Length; i++)
        {
            Image sprite = dragonDetails[i];
            sprite.color = secondary;
            sprite.sprite = PlayerStats.currentSkin.midSprites[i];
        }
        color1 = primary;
        color2 = secondary; 

        if (shit)
        { 
            dragonFace.sprite = PlayerStats.currentSkin.headDetailSprite;

            foreach (UIEffect effect in allEffects)
            {
                effect.PrepareToggle();
            }
        }
    }
    private void UpdateColors()
    {
        if (hasPulse)
        {
            color1 = Color.Lerp(primary, secondary, Mathf.PingPong(Time.time, 1));
            color2 = Color.Lerp(secondary, primary, Mathf.PingPong(Time.time, 1));
        }
        if (hasFade)
        {
            color1.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time / 2, 1));
            color2.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time / 2, 1));
        }
    }

    public void UpdateVisuals()
    {
        if (!hasPulse && !hasFade && !hasKyoki)
            return;

        UpdateColors();

        for (int i = 0; i < dragonBases.Length; i++)
        {
            dragonBases[i].color = color1; 
        }
        for (int i = 0; i < dragonDetails.Length; i++)
        {
            dragonDetails[i].color = color2;
        }
        if (hasTrail)
        {
            trailParticles.startColor = color2;
        }
        if (hasPaint)
        {
            paintParticles.startColor = color2; 
        }

        if (hasKyoki)
        {
            for (int i = 0; i < dragonBodies.Length; i++)
            {
                dragonBodies[i].rotation += dragonBodyRotationSpeeds[i] * Time.fixedDeltaTime;
            }
        }
    }

    private void Update()
    {
        UpdateVisuals();
    }

    public void EnableTrail(bool enable)
    {
        hasTrail = enable;
        trailParticlesObject.SetActive(enable);
        trailParticles = trailParticlesP.main;
        if (enable)
            trailParticles.startColor = color2;
    }
    public void EnablePaint(bool enable)
    {
        hasPaint = enable;
        paintParticlesObject.SetActive(enable);
        paintParticles = paintParticlesP.main;
        if (enable)
            paintParticles.startColor = color2;
    }

    public void EnablePulse(bool enable)
    {
        hasPulse = enable;
        if (!enable)
        {
            PrepareVisuals(false);
        }
    }

    public void EnableFade(bool enable)
    {
        hasFade = enable;
        if (!enable)
        {
            PrepareVisuals(false);
        }
    }

    public void EnableKyoki(bool enable)
    {
        hasKyoki = enable;
        for (int i = 0; i < dragonBodies.Length; i++)
        {
            Rigidbody2D rb = dragonBodies[i];
            rb.isKinematic = !enable;
            if (!enable)
            {
                rb.rotation = 0f;
            }
            dragonBodyRotationSpeeds[i] = Random.Range(-360, 360 );
        }
    }

    public void EnableEnergy(bool enable)
    {
        hasEnergy = enable;
    }

    public void EnableReversed(bool enable)
    {
        hasReversed = enable;
    }

    public void EnablePesimist(bool enable)
    {
        hasPessimist = enable;
        if (enable)
        {
            filter.saturation.value = -100;
        }
        else
        {
            filter.saturation.value = 20;
        }
    }

    public void ChangePage(bool toRight)
    {
        effectPages[currentPage].SetActive(false);
        if (toRight)
        {
            if (currentPage + 1 < effectPages.Count)
            {
                currentPage++;
            }
            else
            {
                currentPage = 0;
            }
        }
        else
        {
            if (currentPage - 1 >= 0)
            {
                currentPage--;
            }
            else
            {
                currentPage = effectPages.Count - 1;
            }
        }
        effectPages[currentPage].SetActive(true);
    }
}
