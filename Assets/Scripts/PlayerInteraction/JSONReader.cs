using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;
    [System.Serializable]
    public class Dialogue{
        public int number;
        public string name;
        public string dialogueContent;
        public string others;
    }
    [System.Serializable]
    public class DialogueList{
        public Dialogue[] dialogue;
    }
    public DialogueList dialogueList;
    void Start()
    {
        dialogueList = JsonUtility.FromJson<DialogueList>(textJSON.text);
    }
}
