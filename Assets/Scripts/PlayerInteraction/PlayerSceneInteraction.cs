using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerSceneInteraction : MonoBehaviour
{
    public delegate void InteractionHandler(Interaction interaction);
    public static event InteractionHandler OnInteraction;
    public enum Interaction{
        Door,
        NPC
    }
    void Start(){
        OnInteraction += SceneInteract;
    }
    void OnDestroy(){
        OnInteraction -= SceneInteract;
    }
    public void SceneInteract(Interaction interaction){
        switch (interaction){
            case Interaction.Door:
                if(Door.isOpenDoor == true){
                    Debug.Log("按F键打开门");
                }
                else{
                    Debug.Log("按F键关闭门");
                }
                break;
            case Interaction.NPC:
                Debug.Log("按F键与NPC对话");
                break;
        }
    }
    public static void TriggerInteraction(Interaction interaction){
        OnInteraction?.Invoke(interaction);
    }
}
