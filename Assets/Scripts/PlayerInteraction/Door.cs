using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;
    Transform doorTransform;
    public static bool isOpenDoorInfo = false;
    public static bool isOpenDoor = false;
    private void Start() {
        animator = GetComponent<Animator>();
        doorTransform = GetComponent<Transform>();
    }
    public void Open(){
        if(isOpenDoor == true){
            animator.SetBool("isDoorOpen",true);
            animator.SetBool("isDoorClose",false);
            MainAudioManager.AudioManagerInstance.PlaySFXScene("OpenDoor"); 
        }
    }
    public void Close(){
        if(isOpenDoor == false){
            animator.SetBool("isDoorClose",true);
            animator.SetBool("isDoorOpen",false);
            MainAudioManager.AudioManagerInstance.PlaySFXScene("CloseDoor");
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            isOpenDoorInfo = true;
            PlayerSceneInteraction.TriggerInteraction(PlayerSceneInteraction.Interaction.Door);
            Open();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            isOpenDoorInfo = false;
            isOpenDoor = false;
            PlayerSceneInteraction.TriggerInteraction(PlayerSceneInteraction.Interaction.Door);
            Close();
        }
    }
}
