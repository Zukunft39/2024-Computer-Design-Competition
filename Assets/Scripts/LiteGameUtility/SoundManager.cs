using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSource2;
    
    [SerializeField] private SerializeableDictionary<string,AudioClip> globalAudioDict;
    
    public void PlayBGM(AudioClip clip)
    {
        if (clip is null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    
    public void PlaySFX(AudioClip clip,float pitchOffset=0,float volume=1)
    {
        if(clip is null) return;
        AudioSource curSource = sfxSource.isPlaying?sfxSource2:sfxSource;
        curSource.clip = clip;
        curSource.pitch = Random.Range(1-pitchOffset,1+pitchOffset);
        curSource.volume = volume;
        curSource.PlayOneShot(clip);
    }
    public void PlaySFX(string clipName,float pitchOffset=0,float volume=1)
    {
        AudioClip clip = globalAudioDict[clipName];
        if(clip is null) return;
        AudioSource curSource = sfxSource.isPlaying?sfxSource2:sfxSource;
        curSource.clip = clip;
        curSource.pitch = Random.Range(1-pitchOffset,1+pitchOffset);
        curSource.volume = volume;
        curSource.PlayOneShot(clip);
    }
}
