using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerSceneInteraction : MonoBehaviour
{
    public delegate void InteractionHandler(Interaction interaction);
    public static event InteractionHandler OnInteraction;
    public GameObject fDoorButton;
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
                if(Door.isOpenDoorInfo == true){
                    MainUIController.mainUIControllerInstance.ShowInteractionInfo(MainUIController.InteractionInfo.DoorInfo);
                    Debug.Log("传递可以开门信息");
                }
                break;
            case Interaction.NPC:

                break;
        }
    }
    public static void TriggerInteraction(Interaction interaction){
        OnInteraction?.Invoke(interaction);
    }
    public void PressFDoorButton(){
        if(fDoorButton.activeSelf == true){
            if(Input.GetKeyDown(KeyCode.F)){//检测键盘F键输入
                Door.isOpenDoor = true;
                Debug.Log(Door.isOpenDoor);
            }
        }
    }
}
