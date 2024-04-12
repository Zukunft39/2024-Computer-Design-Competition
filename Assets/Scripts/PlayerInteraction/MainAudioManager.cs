using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// todo 修改这个为接口进行抽象类处理,太难管理了
public class MainAudioManager : MonoBehaviour
{
    private static MainAudioManager _AudioManagerInstance;
    public static MainAudioManager AudioManagerInstance{
        get{
            if (_AudioManagerInstance == null)
            {
                _AudioManagerInstance = FindObjectOfType<MainAudioManager>();
                if (_AudioManagerInstance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MainAudioManager).Name);
                    _AudioManagerInstance = singletonObject.AddComponent<MainAudioManager>();
                }
            }
            return _AudioManagerInstance;
        }
    }
    public Sound[] musicSounds,sfxSounds,sfxSceneSounds,dialogue;
    public AudioSource musicSource,sfxSource,sfxSceneSource,dialogueSource;
    private void Awake() {
        if(_AudioManagerInstance == null){
            _AudioManagerInstance = this as MainAudioManager;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(gameObject);
        }
    }
    private void Start() {
        PlayMusic("MainBackGroundMusic");
    }
    /// <summary>
    /// 音效管理,播放对应的音效
    /// </summary>
    /// <param name="name"></param>
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
    public void PlaySFXScene(string name){
        Sound s = Array.Find(sfxSceneSounds,x => x.name == name);
        if(s == null) Debug.Log("Sound Not Found");
        else{
            sfxSceneSource.clip = s.audioClip;
            sfxSceneSource.Play();
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
    public void ToggleSFXScene(){
        sfxSceneSource.mute = !sfxSceneSource.mute;
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
    public void SFXSceneVolume(float volume){
        sfxSceneSource.volume = volume;
    }
    public void DialogueVolume(float volume){
        dialogueSource.volume = volume;
    }
    /// <summary>
    /// 停止播放音频
    /// </summary>
    //todo 这里可以使用接口优化
    public void StopMusic(){
        musicSource.Stop();
    }
    public void StopSFX(){
        sfxSource.Stop();
    }
    public void StopSFXScene(){
        sfxSceneSource.Stop();
    }
    public void StopDialogue(){
        dialogueSource.Stop();
    }
    
    private float prevBgmVolume,targetVolume;
    public void WeakenMusic(float volumePercentage,float time){
        prevBgmVolume = musicSource.volume;
        targetVolume = prevBgmVolume * volumePercentage;
        StartCoroutine(WeakenMusicCoroutine(time));
    }
    
    private IEnumerator WeakenMusicCoroutine(float time){
        float timer = 0;
        while(timer < time){
            musicSource.volume = Mathf.Lerp(prevBgmVolume,targetVolume,timer/time);
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    public void RecoverMusic(float time){
        StartCoroutine(RecoverMusicCoroutine(time));
    }
    
    private IEnumerator RecoverMusicCoroutine(float time){
        float timer = 0;
        while(timer < time){
            musicSource.volume = Mathf.Lerp(targetVolume,prevBgmVolume,timer/time);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
