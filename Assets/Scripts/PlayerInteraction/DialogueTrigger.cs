using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Serialization;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private Transform CamTargetTransform;
    [SerializeField] private bool isAINpc = false;
    [SerializeField] private string AIPrompt;
    [SerializeField] private bool isRuledByGlobalPrompt = true;
    private void OnTriggerEnter(Collider other){
        int hasSpoken = PlayerPrefs.GetInt("HasSpoken" + transform.parent.name, 0);
        if(other.CompareTag("Player") && (isAINpc || hasSpoken == 0)){
            other.gameObject.GetComponent<DialogueManager>().DialogueStart(dialogueStrings, NPCTransform,CamTargetTransform,AIPrompt,isRuledByGlobalPrompt);
            PlayerPrefs.SetInt("HasSpoken" + transform.parent.name, 1);
            hasSpoken = PlayerPrefs.GetInt("HasSpoken" + transform.parent.name, 0);
        }
    }
}
[System.Serializable]
public class DialogueString
{
    public string text;
    public bool isEnd;
    public bool isAIDialog;

    [Header("Branch")] 
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Triggered Events")] 
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialougueEvent;
}