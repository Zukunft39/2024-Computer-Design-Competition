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
            musicSource.clip = s.audioClip;
            musicSource.Play();
        }
    }
    public void PlayDialogue(string name){
        Sound s = Array.Find(dialogue,x => x.name == name);
        if(s == null) Debug.Log("Sound Not Found");
        else{
            musicSource.clip = s.audioClip;
            musicSource.Play();
        }
    }

}
