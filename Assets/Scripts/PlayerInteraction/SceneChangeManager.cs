using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;
    public Vector3 PlayerPosition { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading: " + 100 * progress +"%");
            // 更新UI显示进度
            //yourProgressBar.value = progress;
            yield return null;
        }
    }
    public void SavePlayerPosition(GameObject player) {
        PlayerPosition = player.transform.position;
    }
    public void RestorePlayerPosition(GameObject player) {
        player.transform.position = PlayerPosition;
    }
    /// <summary>
    /// 包装器,用于在Unity的事件系统中使用包装
    /// </summary>
    /// <param name="sceneName"></param>
    public void WrapperLoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
}
