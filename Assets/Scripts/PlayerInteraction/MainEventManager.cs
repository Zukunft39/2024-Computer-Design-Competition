using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MainEventManager: MonoBehaviour
{
    public float timeScale;
/// <summary>
/// 脚步声音的监听
/// </summary>
    public delegate void FootstepEventHander(SoundType type);
    public static event FootstepEventHander OnFootstep;
    public bool isWalkSound = false,isRunSound = false;
    public enum SoundType{
        NoSound,
        WalkSound,
        RunSound
    }
    void Start(){
        OnFootstep += PlayFootstepSound;
        //HideCursor();
    }
    void Update() {
        TimeScale();
    }
    void OnDestroy() {
        OnFootstep -= PlayFootstepSound;
    }
    public void PlayFootstepSound(SoundType type){
        if(type == SoundType.WalkSound){
            if(MainAudioManager.AudioManagerInstance.sfxSource.isPlaying == true && MainAudioManager.AudioManagerInstance.sfxSource.clip.name == "WalkSound") return;
            else{
                if(!isWalkSound){
                    isRunSound = false;
                    MainAudioManager.AudioManagerInstance.PlaySFX("Walk");
                    //Debug.Log("正在播放"+MainAudioManager.AudioManagerInstance.sfxSource.clip.name+"音效");
                    isWalkSound = true;
                }
                else return;
            }
        }
        else if(type == SoundType.RunSound){
            if(MainAudioManager.AudioManagerInstance.sfxSource.isPlaying == true && MainAudioManager.AudioManagerInstance.sfxSource.clip.name == "RunSound") return;
            else{
                if(!isRunSound){
                    isWalkSound = false;
                    MainAudioManager.AudioManagerInstance.PlaySFX("Run");
                    //Debug.Log("正在播放"+MainAudioManager.AudioManagerInstance.sfxSource.clip.name+"音效");
                    isRunSound = true;
                }
                else return;
            }
        }
        else if(type == SoundType.NoSound){
            if(MainAudioManager.AudioManagerInstance.sfxSource.isPlaying == true){
                MainAudioManager.AudioManagerInstance.sfxSource.Stop();
            }
            else return;
        }
    }
    public static void TriggerFootStep(SoundType type){
        OnFootstep?.Invoke(type);
    }

/// <summary>
/// 锁定鼠标位置并隐藏鼠标.
/// </summary>
    private void HideCursor(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void TimeScale(){
        Time.timeScale = timeScale;
    }
}