using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeScene : MonoBehaviour
{

    public void ChangeScene()
    {
        StartCoroutine(SceneChangeManager.instance.LoadSceneAsync("CircleChop"));
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            Debug.Log("进入触发器");
            ChangeScene();
        }
    }
}
