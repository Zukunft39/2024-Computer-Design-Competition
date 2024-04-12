using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSource2;
    [SerializeField] private float bgmWeakenPercentage = 0.1f;
    [SerializeField] private float bgmWeakenTime = 0.25f;
    [SerializeField] private float bgmRecoverTime = 1f;
    
    [SerializeField] private SerializeableDictionary<string,AudioClip> globalAudioDict;
    
    public void PlayBGM(AudioClip clip)
    {
        if (clip is null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    
    public void PlaySFX(AudioClip clip,float pitchOffset=0,float volume=1,bool isWeakenBGM=false)
    {
        if(clip is null) return;
        if(isWeakenBGM)
        {
            MainAudioManager.AudioManagerInstance.WeakenMusic(bgmWeakenPercentage, bgmWeakenTime);
            StartCoroutine(RecoverBGM((clip.length - 2)>0?clip.length - 2:0));
        }
        AudioSource curSource = sfxSource.isPlaying||pitchOffset!=0?sfxSource2:sfxSource;
        curSource.clip = clip;
        sfxSource2.pitch = Random.Range(1-pitchOffset,1+pitchOffset);
        curSource.volume = volume;
        curSource.PlayOneShot(clip);
        
    }
    
    IEnumerator RecoverBGM(float sfxDuration)
    {
        yield return new WaitForSeconds(sfxDuration);
        MainAudioManager.AudioManagerInstance.RecoverMusic(bgmRecoverTime);
    }
    
    public void PlaySFX(string clipName,float pitchOffset=0,float volume=1,bool isWeakenBGM=false)
    {
        AudioClip clip = globalAudioDict[clipName];
        PlaySFX(clip,pitchOffset,volume,isWeakenBGM);
    }
}
