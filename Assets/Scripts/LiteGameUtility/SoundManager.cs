using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  小游戏声音管理器
/// </summary>
public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    //背景音乐
    [SerializeField] private AudioSource bgmSource;
    //音效源1
    [SerializeField] private AudioSource sfxSource;
    //音效源2
    [SerializeField] private AudioSource sfxSource2;
    //背景音乐减弱百分比
    [SerializeField] private float bgmWeakenPercentage = 0.1f;
    //背景音乐减弱时间
    [SerializeField] private float bgmWeakenTime = 0.25f;
    //背景音乐恢复时间
    [SerializeField] private float bgmRecoverTime = 1f;
    
    //全局音频字典
    [SerializeField] private SerializeableDictionary<string,AudioClip> globalAudioDict;
    
    /// <summary>
    ///  播放背景音乐
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBGM(AudioClip clip)
    {
        if (clip is null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip">音效片段</param>
    /// <param name="pitchOffset">音高偏移</param>
    /// <param name="volume">音量</param>
    /// <param name="isWeakenBGM">是否暂时减弱背景音乐</param>
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
    
    // 恢复背景音乐协程
    IEnumerator RecoverBGM(float sfxDuration)
    {
        yield return new WaitForSeconds(sfxDuration);
        MainAudioManager.AudioManagerInstance.RecoverMusic(bgmRecoverTime);
    }
    
    /// <summary>
    ///  播放音效
    /// </summary>
    /// <param name="clipName">音效片段在全局音频字典中的名字</param>
    /// <param name="pitchOffset">音高偏移</param>
    /// <param name="volume">音量</param>
    /// <param name="isWeakenBGM">是否暂时减弱背景音乐</param>
    public void PlaySFX(string clipName,float pitchOffset=0,float volume=1,bool isWeakenBGM=false)
    {
        AudioClip clip = globalAudioDict[clipName];
        PlaySFX(clip,pitchOffset,volume,isWeakenBGM);
    }
}
