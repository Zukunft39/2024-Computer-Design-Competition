using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;
    Transform doorTransform;
    public static bool isOpenDoor = false;
    private void Start() {
        animator = GetComponent<Animator>();
        doorTransform = GetComponent<Transform>();
    }
    public void Open(){
        animator.SetBool("isDoorOpen",true);
        animator.SetBool("isDoorClose",false);
        MainAudioManager.AudioManagerInstance.PlaySFXScene("OpenDoor");
        isOpenDoor = true;
    }
    public void Close(){
        animator.SetBool("isDoorClose",true);
        animator.SetBool("isDoorOpen",false);
        MainAudioManager.AudioManagerInstance.PlaySFXScene("CloseDoor");
        isOpenDoor = true;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            PlayerSceneInteraction.TriggerInteraction(PlayerSceneInteraction.Interaction.Door);
            Open();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            PlayerSceneInteraction.TriggerInteraction(PlayerSceneInteraction.Interaction.Door);
            Close();
        }
    }
}
