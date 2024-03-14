using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainAudioManager : MonoBehaviour
{
    public static MainAudioManager AudioManagerInstance;
    public Sound[] musicSounds,sfxSounds,dialogue;
    public AudioSource musicSource,sfxSource,dialogueSource;
    private void Awake() {
        if(AudioManagerInstance == null){
            AudioManagerInstance = this;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(gameObject);
        }
    }
    private void Start() {
        PlayMusic("MainBackGroundMusic");
    }
    public void PlayMusic(string name){
        Sound s = Array.Find(musicSounds,x => x.name == name);
        if(s == null) Debug.Log("Sound Not Found");
        else{
            musicSource.clip = s.audioClip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name){
        Sound s = Array.Find(sfxSounds,x => x.name == name);
        if(s == null) Debug.Log("Sound Not Found");
        else{
            sfxSource.clip = s.audioClip;
            sfxSource.Play();
        }
    }
    public void PlayDialogue(string name){
        Sound s = Array.Find(dialogue,x => x.name == name);
        if(s == null) Debug.Log("Sound Not Found");
        else{
            dialogueSource.clip = s.audioClip;
            dialogueSource.Play();
        }
    }
/// <summary>
/// UI调整音量
/// </summary>
    public void ToggleMusic(){
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX(){
        sfxSource.mute = !sfxSource.mute;
    }
    public void ToggleDialogue(){
        dialogueSource.mute = !dialogueSource.mute;
    }
    public void MusicVolume(float volume){
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume){
        sfxSource.volume = volume;
    }
    public void DialogueVolume(float volume){
        dialogueSource.volume = volume;
    }

    
}
