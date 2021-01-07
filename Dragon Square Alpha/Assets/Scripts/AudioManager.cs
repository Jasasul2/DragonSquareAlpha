using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static bool muted = false;

    private static AudioClip movement, 
        grow, 
        shrink, 
        shrinkFood, 
        die, 
        click, 
        lose, 
        win, 
        place, 
        remove, 
        cantPlace, 
        reach,
        infoIn,
        infoOut,
        buy; 

    public static AudioManager instance;

    [HideInInspector]
    public enum Sound
    {
        Movement,
        Grow,
        Shrink,
        ShrinkFood,
        Die,
        Click,
        Lose,
        Win,
        Place,
        Remove,
        CantPlace,
        Reach,
        InfoIn,
        InfoOut,
        Buy
    }

    private static AudioSource sfx;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;//Avoid doing anything else
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        sfx = GetComponent<AudioSource>();
        LoadSounds();
        if (muted)
            sfx.volume = 0;
    }

    private void LoadSounds()
    {
        movement = Resources.Load<AudioClip>("mvmnt");
        grow = Resources.Load<AudioClip>("pearl");
        shrink = Resources.Load<AudioClip>("shrink");
        shrinkFood = Resources.Load<AudioClip>("shrink");
        die = Resources.Load<AudioClip>("death");
        click = Resources.Load<AudioClip>("click");
        lose = Resources.Load<AudioClip>("lose");
        win = Resources.Load<AudioClip>("win");
        place = Resources.Load<AudioClip>("place");
        remove = Resources.Load<AudioClip>("rw");
        cantPlace = Resources.Load<AudioClip>("cpw");
        reach = Resources.Load<AudioClip>("reach");
        infoIn = Resources.Load<AudioClip>("infoTableIn");
        infoOut = Resources.Load<AudioClip>("infoTableOut");
        buy = Resources.Load<AudioClip>("buy");
    }

    public static void PlaySound(Sound clip)
    {
        if (sfx == null)
            return;

        switch (clip)
        {
            case (Sound.Movement): 
                sfx.PlayOneShot(movement);
                break;
            case (Sound.Grow):
                sfx.PlayOneShot(grow);
                break;
            case (Sound.Shrink):  
                sfx.PlayOneShot(shrink);
                break;
            case (Sound.ShrinkFood): 
                sfx.PlayOneShot(shrinkFood);
                break;
            case (Sound.Die): 
                sfx.PlayOneShot(die);
                break;
            case (Sound.Click):
                sfx.PlayOneShot(click);
                break;
            case (Sound.Lose):
                sfx.PlayOneShot(lose);
                break;
            case (Sound.Win):
                sfx.PlayOneShot(win);
                break;
            case (Sound.Place):
                sfx.PlayOneShot(place);
                break;
            case (Sound.Remove):
                sfx.PlayOneShot(remove); 
                break;
            case (Sound.CantPlace):
                sfx.PlayOneShot(cantPlace);
                break;
            case (Sound.Reach):
                sfx.PlayOneShot(reach);
                break;
            case (Sound.InfoIn):
                sfx.PlayOneShot(infoIn);
                break;
            case (Sound.InfoOut):
                sfx.PlayOneShot(infoOut);
                break;
            case (Sound.Buy):
                sfx.PlayOneShot(buy);
                break;
        }
    }

    public static void Mute(bool mute)
    {
        muted = mute;
        if (sfx != null)
            sfx.mute = mute;
    }
}