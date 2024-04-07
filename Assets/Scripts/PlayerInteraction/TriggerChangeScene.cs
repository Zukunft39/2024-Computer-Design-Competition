using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void ChangeScene()
    {
        StartCoroutine(SceneChangeManager.instance.LoadSceneAsync(sceneName));
    }
}
