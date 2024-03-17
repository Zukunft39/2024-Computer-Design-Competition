using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
/// <summary>
/// 狮山,F键开门,退出范围后自动关门
/// </summary>
    Animator animator;
    Transform doorTransform;
    public Transform playerTransform;
    private bool isOpenDoorInfo = false;
    private bool isOpen = false;
    private bool isForward = false;
    private void Start() {
        animator = GetComponent<Animator>();
        doorTransform = transform;
    }
    private void Update() {
        GetFKeyBoardInput();
    }
    private void Open(){
        if(isForward){
            Debug.Log("向里开门");
            animator.SetBool("isOpenDoorInward",true);
            animator.SetBool("isOpenDoorOutward",false);
            animator.SetBool("isCloseDoorInward",false);
            animator.SetBool("isCloseDoorOutward",false);
        }
        else{
            Debug.Log("向外开门");
            animator.SetBool("isOpenDoorOutward",true);
            animator.SetBool("isCloseDoorOutward",false);
            animator.SetBool("isOpenDoorInward",false);
            animator.SetBool("isCloseDoorInward",false);
        }
        MainAudioManager.AudioManagerInstance.PlaySFXScene("OpenDoor");
    }
    private void Close(){
        if(isForward){
            if(!isOpenDoorInfo && isOpen){
                Debug.Log("向里关门");
                isOpen = false;
                animator.SetBool("isCloseDoorInward",true);
                animator.SetBool("isOpenDoorInward",false);
                animator.SetBool("isOpenDoorOutward",false);
                animator.SetBool("isCloseDoorOutward",false);
            }
        }
        else{
            if(!isOpenDoorInfo && isOpen){
                Debug.Log("向外关门");
                isOpen = false;
                animator.SetBool("isCloseDoorOutward",true);
                animator.SetBool("isOpenDoorOutward",false);
                animator.SetBool("isOpenDoorInward",false);
                animator.SetBool("isCloseDoorInward",false);    
            }
        }
        MainAudioManager.AudioManagerInstance.PlaySFXScene("CloseDoor");
    }
    private void OnTriggerEnter(Collider other) {
        isForward = CalculateForward();
        MainUIController.mainUIControllerInstance.GenerateDoorButton();
        isOpenDoorInfo = true;
        Debug.Log("进入触发器,进行前后位置判断");
    }
    private void OnTriggerExit(Collider other) {
        MainUIController.mainUIControllerInstance.DesTroyDoorButton();
        isOpenDoorInfo = false;
        Close();
        Debug.Log("退出触发器判断");
    }
    private void GetFKeyBoardInput(){
        if(Input.GetKeyDown(KeyCode.F)){
            if(isOpenDoorInfo){
                Open();
                isOpen = true;
                isOpenDoorInfo = false;
                MainUIController.mainUIControllerInstance.DesTroyDoorButton();
            }
        }
        else return;
    }
    private bool CalculateForward(){
        Vector3 toPlayer = playerTransform.position - doorTransform.position;
        float dotProduct = Vector3.Dot(toPlayer.normalized, doorTransform.forward);
        if(dotProduct >= 0){
            return true;
        }
        else{
            return false;
        }
    }
}
