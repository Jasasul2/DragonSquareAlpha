using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSkins : MonoBehaviour
{
    public Skin currentSkin;
    public GameObject 
        paintParticles, 
        trailParticles;

    private ParticleSystem.MainModule 
        paintSpawned, 
        trailSpawned; 

    [HideInInspector] public Color 
        mainColor, 
        secondaryColor;

    public SpriteRenderer[] bases = new SpriteRenderer[3];
    public SpriteRenderer[] mids = new SpriteRenderer[3];
    public SpriteRenderer head;

    private bool fading = false, pulsing = false; 
    // Start is called before the first frame update
    private void Start()
    {
        if(PlayerStats.currentSkin != null)
            currentSkin = PlayerStats.currentSkin;

        if (PlayerStats.mainColor != null)
            mainColor = PlayerStats.mainColor;

        if (PlayerStats.secondaryColor != null)
            secondaryColor = PlayerStats.secondaryColor;

        for (int i = 0; i < currentSkin.baseSprites.Length; i++)
        {
            bases[i].sprite = currentSkin.baseSprites[i];
            bases[i].GetComponent<SpriteMask>().sprite = currentSkin.baseSprites[i];
            bases[i].color = mainColor;

            mids[i].sprite = currentSkin.midSprites[i];
            mids[i].GetComponent<SpriteMask>().sprite = currentSkin.midSprites[i];
            mids[i].color = secondaryColor; 
        }
        head.sprite = currentSkin.headDetailSprite;

        if (PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Pulse])
            pulsing = true;
        if (PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Fade])
            fading = true; 
        SetUpAbilities();
    }

    public void UpdateParticlesColor(bool trail, Color newColor)
    {
        if (trail)
        {
            trailSpawned.startColor = newColor;
        }
        else 
        {
            paintSpawned.startColor = newColor;
        }
    }

    private void SetUpAbilities()
    {
        if (PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Paint])
        {
            AddParticles(false);
        }
        if (PlayerStats.selectedSkinEffects[PlayerStats.SkinEffects.Trail])
        {
            AddParticles(true);
        }
        if (pulsing || fading)
        {
            SkinFadeOrPulse pulse = gameObject.AddComponent<SkinFadeOrPulse>();
            pulse.bases = bases;
            pulse.details = mids;
            pulse.head = head;
            pulse.primary = mainColor;
            pulse.secondary = secondaryColor;
            if(pulsing)
                pulse.pulsing = true;
            if (fading)
                pulse.fading = true; 
        }
    }

    private void AddParticles(bool trail)
    {
        Transform headPosition = bases[0].transform;
        if (trail)
        {
            GameObject spawnedTrailParticles = Instantiate(trailParticles, headPosition.position, Quaternion.identity, headPosition);
            trailSpawned = spawnedTrailParticles.GetComponent<ParticleSystem>().main;
            trailSpawned.startColor = secondaryColor;
        }
        else
        {
            GameObject spawnedPaintParticles = Instantiate(paintParticles, headPosition.position, Quaternion.identity, headPosition);
            paintSpawned = spawnedPaintParticles.GetComponent<ParticleSystem>().main;
            paintSpawned.startColor = secondaryColor;
        }
    }
}
