using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainUIController : MonoBehaviour
{
    public static MainUIController mainUIControllerInstance;
    public Slider _musicSlider,_sfxSlider,_dialogueSlider;
    public Image fDoorButton;
    public Image blackPanelStart;
    public Image blackPanelEnd;
    private void Awake() {
        if(mainUIControllerInstance == null){
            mainUIControllerInstance = this;
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
    public void GenerateDoorButton(){
        fDoorButton.gameObject.SetActive(true);
    }
    public void DesTroyDoorButton(){
        fDoorButton.gameObject.SetActive(false);
    }
/// <summary>
/// 黑色背景淡出与显示,以及开始与通关背景介绍
/// </summary>
    public void HideBlackPanel(){
        blackPanelStart.DOFade(0,2).SetEase(Ease.Linear).OnComplete(()=>blackPanelStart.gameObject.SetActive(false));
        MainEventManager.Instance.HideCursor();
    }
    public void ShowBlackStartPanel(){
        blackPanelStart.gameObject.SetActive(true);
        blackPanelStart.DOFade(1,2).SetEase(Ease.Linear);
        StartCoroutine(ShowDialogueStart());
    }
    public void ShowBlackEndPanel(){
        blackPanelStart.gameObject.SetActive(true);
        blackPanelStart.DOFade(1,2).SetEase(Ease.Linear);
        StartCoroutine(ShowDialogueEnd());
    }
    public IEnumerator ShowDialogueStart(){
        for (int i = 0; i < blackPanelStart.transform.childCount; i++){
            Transform temp = blackPanelStart.transform.GetChild(i);
            temp.gameObject.SetActive(true);
            temp.GetComponent<TextMeshProUGUI>().DOFade(1,2).SetEase(Ease.Linear);
            yield return new WaitForSeconds(2);
            temp.GetComponent<TextMeshProUGUI>().DOFade(0,2).SetEase(Ease.Linear).OnComplete(()=>temp.gameObject.SetActive(false));
            yield return new WaitForSeconds(2);
        }
        HideBlackPanel();
        yield return new WaitForSeconds(2);
        InitialUIController.Instance.HideCanvas();
    }
    public IEnumerator ShowDialogueEnd(){
        for (int i = 0; i < blackPanelEnd.transform.childCount; i++){
            Transform temp = blackPanelEnd.transform.GetChild(i);
            temp.gameObject.SetActive(true);
            temp.GetComponent<TextMeshProUGUI>().DOFade(1,2).SetEase(Ease.Linear);
            yield return new WaitForSeconds(2);
            temp.GetComponent<TextMeshProUGUI>().DOFade(0,2).SetEase(Ease.Linear).OnComplete(()=>temp.gameObject.SetActive(false));
            yield return new WaitForSeconds(2);
        }
    }
}
