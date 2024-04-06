using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void ChangeScene()
    {
        if(sceneName == "DemoScene"){
            StartCoroutine(SceneChangeManager.instance.LoadSceneAsync(sceneName));
            Debug.Log("回到主场景");
        }
        else StartCoroutine(SceneChangeManager.instance.LoadSceneAsync(sceneName));
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            Debug.Log("进入触发器");
            ChangeScene();
            SceneChangeManager.instance.SavePlayerPosition(other.gameObject);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
