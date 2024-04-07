using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;
    [SerializeField] private ThirdPersonController thirdPersonController;
    public Vector3 PlayerPosition { get; private set; }
    public static float progress = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log("成功传入参数");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading: " + 100 * asyncLoad.progress +"%");
            // 更新UI显示进度
            //yourProgressBar.value = progress;
            yield return null;
        }
        if(sceneName != "DemoScene"){
            SavePlayerPosition();
        } 
        DOVirtual.DelayedCall(1, () =>
        {
            Debug.Log("1秒已过,现在执行");
            Debug.Log("保存玩家位置" + PlayerPosition);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }); 
        asyncLoad.allowSceneActivation = true;
        if(sceneName == "DemoScene"){
            RestorePlayerPosition();
            Debug.Log("还原玩家位置"+ PlayerPosition);
        }
    }
    public void SavePlayerPosition() {
        PlayerPosition = transform.position;
    }
    public void RestorePlayerPosition() {
        transform.position = PlayerPosition;
    }
}
