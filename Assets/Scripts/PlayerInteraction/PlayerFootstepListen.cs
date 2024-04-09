using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerFootstepListen: MonoBehaviourSingleton<PlayerFootstepListen>
{
    public delegate void FootstepEventHander(SoundType type);
    public static event FootstepEventHander OnFootstep;
    private bool isWalkSound = false,isRunSound = false;
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
    /// <summary>
    /// 播放走路声音控制变化
    /// </summary>
    /// <param name="type"> 声音类型 </param>
    /// <param name="soundName"> 声音名称 </param>
    /// <param name="currentState"> 当前状态 </param>
    /// <param name="otherState"> 其他状态 </param>
    private void PlaySound(SoundType type, string soundName, ref bool currentState, ref bool otherState) {
        AudioSource sfxSource = MainAudioManager.AudioManagerInstance.sfxSource;
        if (sfxSource.isPlaying && sfxSource.clip.name == soundName) return; // 防止重复播放
        if (!currentState) { // 停止播放其他声音并播放当前声音
            otherState = false;
            currentState = true;
            MainAudioManager.AudioManagerInstance.PlaySFX(soundName);
        }
    }
    private void StopAllSounds() {
        AudioSource sfxSource = MainAudioManager.AudioManagerInstance.sfxSource;
        if (sfxSource.isPlaying) {
            isRunSound = false;
            isWalkSound = false;
            sfxSource.Stop();
        }
    }
    public void PlayFootstepSound(SoundType type) {
        switch (type) {
            case SoundType.WalkSound:
                PlaySound(type, "Walk", ref isWalkSound, ref isRunSound);
                break;
            case SoundType.RunSound:
                PlaySound(type, "Run", ref isRunSound, ref isWalkSound);
                break;
            case SoundType.NoSound:
                StopAllSounds();
                break;
        }
    }
    public static void TriggerFootStep(SoundType type){
        OnFootstep?.Invoke(type);
    }
}