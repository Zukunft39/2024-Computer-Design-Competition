using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System;

public class MainEventManager: MonoBehaviour
{
/// <summary>
/// 脚步声音的监听
/// </summary>
    public delegate void FootstepEventHander(SoundType type);
    public static event FootstepEventHander OnFootstep;
    public enum SoundType{
        NoSound,
        WalkSound,
        RunSound
    }
    void Start(){
        OnFootstep += PlayFootstepSound;
    }
    void OnDestroy() {
        OnFootstep -= PlayFootstepSound;
    }
    public void PlayFootstepSound(SoundType type){
        if(type == SoundType.WalkSound){
            MainAudioManager.AudioManagerInstance.PlaySFX("Walk");
        }
        else if(type == SoundType.RunSound){
            MainAudioManager.AudioManagerInstance.PlaySFX("Run");
        }
    }
    public static void TriggerFootStep(SoundType type){
        OnFootstep?.Invoke(type);
    }
}