using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private Transform CamTargetTransform;
    private void OnTriggerEnter(Collider other){
        int hasSpoken = PlayerPrefs.GetInt("HasSpoken" + transform.parent.name, 0);
        if(other.CompareTag("Player") && hasSpoken == 0){
            other.gameObject.GetComponent<DialogueManager>().DialogueStart(dialogueStrings, NPCTransform,CamTargetTransform);
            other.transform.rotation = Quaternion.Euler(other.transform.rotation.x, 0, other.transform.rotation.z);
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