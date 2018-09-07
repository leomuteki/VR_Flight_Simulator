using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public float defaultVolume = 1;
    [SerializeField]
    private AudioSource soundSource1;
    [SerializeField]
    private AudioSource soundSource2;
    [SerializeField]
    private AudioSource BGMSource;

    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Blackboard.Sounds = this;
    }

    private void Start()
    {
        defaultVolume = soundSource1.volume;
        LoadSounds();
    }

    private void LoadSounds()
    {
        UnityEngine.Object[] loadedSounds = Resources.LoadAll("Sounds");
        foreach (UnityEngine.Object sound in loadedSounds)
        {
            AudioClip clip = sound as AudioClip;
            sounds.Add(clip.name, clip);
        }
    }

    public void PlaySound(string name)
    {
        if (sounds.ContainsKey(name))
        {
            soundSource1.clip = sounds[name];
            soundSource1.loop = false;
            soundSource1.Play();
        }
    }

    public void PlaySoundLoop(string name)
    {
        if (sounds.ContainsKey(name))
        {
            soundSource1.clip = sounds[name];
            soundSource1.loop = true;
            soundSource1.Play();
        }
    }

    public void PlaySoundFade(int source, string name, float seconds)
    {
        AudioSource mySource = (source == 1) ? soundSource1 : mySource = soundSource2;
        if (sounds.ContainsKey(name))
        {
            mySource.clip = sounds[name];
            mySource.loop = true;
            FadeAudio(mySource, true, seconds);
        }
    }

    public void StopSound(string name)
    {
        if (soundSource1.clip && soundSource1.clip.name == name)
        {
            soundSource1.Stop();
        }
    }

    public void StopLoop(string name)
    {
        if (soundSource1.clip && soundSource1.clip.name == name)
        {
            soundSource1.loop = false;
        }
    }

    public void StopSoundFade(int source, string name, float seconds)
    {
        AudioSource mySource = (source == 1) ? soundSource1 : mySource = soundSource2;
        if (mySource.clip && mySource.clip.name == name)
        {
            FadeAudio(mySource, false, seconds);
        }
    }

    private void FadeAudio(AudioSource source, bool fadeIn, float seconds)
    {
        StartCoroutine(FadeSoundCoroutine(source, fadeIn, seconds));
    }

    private IEnumerator FadeSoundCoroutine(AudioSource source, bool fadeIn, float seconds)
    {
        if (fadeIn)
        {
            source.volume = 0;
            source.Play();
        }
        float startTime = Time.time;
        float interpolator = fadeIn ? 0 : 1;
        while ((!fadeIn && interpolator > float.Epsilon) || (fadeIn && interpolator < (1 - float.Epsilon)))
        {
            source.volume = interpolator * defaultVolume;
            yield return null;
            if (fadeIn)
            {
                interpolator = (Time.time - startTime) / seconds;
            }
            else
            {
                interpolator = 1 - ((Time.time - startTime) / seconds);
            }
        }
        if (!fadeIn)
        {
            source.Stop();
            source.volume = defaultVolume;
        }
    }
}
