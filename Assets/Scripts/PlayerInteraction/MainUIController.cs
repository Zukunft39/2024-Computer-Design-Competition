using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
/// <summary>
/// 音效管理
/// </summary>
    public Slider _musicSlider,_sfxSlider,_dialogueSlider;
    public void ToggleMusic(){
        MainAudioManager.AudioManagerInstance.ToggleMusic();
    }
    public void ToggleSFX(){
        MainAudioManager.AudioManagerInstance.ToggleSFX();
    }
    public void ToggleDialogue(){
        MainAudioManager.AudioManagerInstance.ToggleDialogue();
    }
    public void MusicVolume(){
        MainAudioManager.AudioManagerInstance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolume(){
        MainAudioManager.AudioManagerInstance.SFXVolume(_sfxSlider.value);
    }
    public void DialogueVolume(){
        MainAudioManager.AudioManagerInstance.DialogueVolume(_dialogueSlider.value);
    }
}
