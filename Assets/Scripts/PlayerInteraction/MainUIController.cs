using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    public static MainUIController mainUIControllerInstance;
    public Slider _musicSlider,_sfxSlider,_dialogueSlider;
    public Image fDoorButton;
    public enum InteractionInfo{
        DoorInfo,
        NPCInfo
    }
    private void Awake() {
        if(mainUIControllerInstance == null){
            mainUIControllerInstance = this;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(gameObject);
        }
    }
/// <summary>
/// 音效管理
/// </summary>
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
/// <summary>
/// 显示交互按键
/// </summary>
    public void ShowInteractionInfo(InteractionInfo info){
        switch (info)
        {
            case InteractionInfo.DoorInfo:
                fDoorButton.gameObject.SetActive(true);
                Debug.Log("显示按f开门");
                if(Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("玩家按下了F键");
                    fDoorButton.gameObject.SetActive(false);
                    Debug.Log("关闭按f开门");
                }
                break;
            case InteractionInfo.NPCInfo:
                
                break;
            default:
                break;
        }
    }
}
