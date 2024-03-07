using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }
    public SoundType[] Sounds;

    public AudioSource soundEffect;
    public AudioSource soundMusic;

    public bool IsMute = false;
    public float Volume = 1f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetVolume(0.5f);
        PlayMusic(global::Sounds.Music);
    }

    public void Mute(bool status)
    {
        IsMute = status;
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        soundEffect.volume = Volume;
        soundMusic.volume = Volume;
    }   

    public void setVolumeOfAudioSource(AudioSource sourceName, float volume)
    {
        sourceName.volume = volume;
    }

    public float PlaySoundOfAudioSource(AudioSource sourceName, Sounds sound)
    {
        AudioClip clip = getSoundClip(sound, sourceName);

        if (clip != null)
        {
            sourceName.PlayOneShot(clip);
            
            //Debug.Log(clip.name);
            return clip.length;
        }
        else
        {
            Debug.LogError("Clip not found for sound type: " + sound);
            return 0;
        }
    }


    public void PlayMusic(Sounds sound)
    {
        if (IsMute)
            return;

        AudioClip clip = getSoundClip(sound, soundMusic);
        if (clip != null)
        {
            soundMusic.clip = clip; 
            soundMusic.Play();

            if(sound == global::Sounds.Music)
            { 
                soundMusic.loop = true;
            }
            else
            {
                soundMusic.loop = false;
            }
        }
        else
        {
            Debug.LogError("Clip not found for sound type: " + sound);
        }
    }

    public void Play(Sounds sound)
    {
        AudioClip clip = getSoundClip(sound, soundEffect);
        
        if (clip != null)
        {
            soundEffect.PlayOneShot(clip);
            //soundEffect.clip = clip;
            //soundEffect.Play();
            Debug.Log(clip.name);
        }
        else
        {
            Debug.LogError("Clip not found for sound type: " + sound);
        }
    }

    private AudioClip getSoundClip(Sounds sound, AudioSource audioSourceName)
    {
        //int index = Array.FindIndex(Sounds, 0, Sounds.Length, i => i.soundType == sound);

        SoundType item = Array.Find(Sounds, i => i.soundType == sound);
        if (item != null)
        {
           return item.soundClip;
        }

        return null;
    }
}

[Serializable]
public class SoundType
{
    public Sounds soundType;
    public AudioClip soundClip;
}

public enum Sounds
{
    ButtonClick,
    Music,
    PlayerMove,
    PlayerJump,
    PlayerLand,
    PlayerDeath,
    EnemyAttack,
    EnemyDeath,
    Collectible,
    LevelFinish,
    LevelFail,
    NewLevelStart
}

