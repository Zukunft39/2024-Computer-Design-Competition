using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFindPlayer : MonoBehaviourSingleton<CameraFindPlayer>
{
    [SerializeField] private Transform playerTrans;
    public void FindPlayer(){
        playerTrans = GameObject.FindWithTag("PlayerCam").transform;
        transform.GetComponent<CinemachineVirtualCamera>().Follow = playerTrans;
    }
}
