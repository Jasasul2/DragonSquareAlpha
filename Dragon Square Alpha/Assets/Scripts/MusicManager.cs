using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private static AudioClip lastMusic; 
    private static AudioSource source;
    public static bool muted = false; 
    public static float volume = 0.13f;
    public AudioClip[] musicChoices;

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return; //Avoid doing anything else
        }
        lastMusic = musicChoices[0];
        SetUpMusic();
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public static void Initialize() => instance.SetUpMusic();

    private void SetUpMusic()
    {
        source = GetComponent<AudioSource>();
        if (muted)
            source.volume = 0;
        else
            source.volume = volume;

        AudioClip newMusic;
        if (musicChoices.Length > 1)
        {
            do
            {
                newMusic = musicChoices[Random.Range(0, musicChoices.Length)];
            }
            while (newMusic == lastMusic);
            lastMusic = newMusic;

        }
        else
            newMusic = musicChoices[0];
        source.clip = newMusic;
        source.Play();
    }

    public static void Mute(bool mute)
    {
        muted = mute; 
        if(source != null)
            source.mute = mute;
    }

    public static void FadeOut(float duration, float target)
    {
        instance.StartCoroutine(StartFade(duration, target)); 
    }

    public static IEnumerator StartFade(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
