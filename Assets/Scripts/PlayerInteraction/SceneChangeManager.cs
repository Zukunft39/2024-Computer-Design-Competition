using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.IO;

/// <summary>
/// 存储玩家位置信息
/// </summary>
[System.Serializable]
public class PlayerTransform{
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public Vector3 playerScale;
}
public class SceneChangeManager : MonoBehaviour
{
    private static SceneChangeManager instance;
    public static SceneChangeManager Instance{
        get{
            if (instance == null)
            {
                instance = FindObjectOfType<SceneChangeManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SceneChangeManager).Name);
                    instance = singletonObject.AddComponent<SceneChangeManager>();
                }
            }
            return instance;
        }
    }
    [SerializeField] private Transform player;
    public PlayerTransform playerData; // Json文件存储信息
    public Transform _PlayerTransform { get; private set; } // Editor玩家位置
    public static float progress = 0;
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as SceneChangeManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != instance)
            {
                Destroy(gameObject);
            }
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
            Debug.Log("保存玩家位置" + _PlayerTransform.position);
        }
        StartCoroutine(Init(sceneName));
    }
    public IEnumerator Init(string sceneName){

        yield return new WaitForSeconds(0.25f);
        Debug.Log("0.25秒已过,现在执行");

        // 通用设置
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        MainAudioManager.AudioManagerInstance.StopMusic();
        MainAudioManager.AudioManagerInstance.StopSFX();

        if(sceneName == "DemoScene"){
            // 特定场景设置
            yield return new WaitForSeconds(3f); // Separate this as it's a different delay.
            Debug.Log("延迟三秒进入");
            player = GameObject.FindWithTag("Player").transform;
            //todo PlayerTransform 未获取到位置,明天来解决
            // todo 解决中
            if(player && _PlayerTransform)
            {
                Debug.Log("已找到玩家的位置"+player.transform.position);
                Debug.Log("原位置"+ _PlayerTransform.position);

                //RestorePlayerPosition(player.transform);
                Debug.Log("还原玩家位置"+ player.transform.position);
                MainAudioManager.AudioManagerInstance.PlayMusic("MainBackGroundMusic");

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else Debug.LogError("Player or PlayerTransform not found.");
        }
    }

    //todo 将玩家位置数据存储到JSON文件中进行读取
    /// <summary>
    /// 存储玩家位置到Json文件中
    /// </summary>
    /// <param name="playerTransform"></param>
    public void SavePlayerTransformToJson(PlayerTransform playerTransform){
        string json = JsonUtility.ToJson(playerTransform);
        File.WriteAllText(Application.persistentDataPath + "/playerTransform.json", json);
    }
    public PlayerTransform LoadPlayerTransformFromJson()
    {
        string path = Application.persistentDataPath + "/playerTransform.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerTransform>(json);
        }
        return null;
    }
    /// <summary>
    /// 读取与还原玩家位置
    /// </summary>
    public void SavePlayerPosition() {
        _PlayerTransform = GameObject.FindWithTag("Player").transform;
        
        /*if (_PlayerTransform == null)
        {
            Debug.LogError("_PlayerTransform is not set!");
        }
        if (playerData == null)
        {
            Debug.LogError("playerData has not been instantiated!");
        }*/

        playerData.playerPosition = _PlayerTransform.position;
        playerData.playerRotation = _PlayerTransform.rotation.eulerAngles;
        playerData.playerScale = _PlayerTransform.localScale;
        SavePlayerTransformToJson(playerData);
    }
    public void RestorePlayerPosition(Transform player) {
        playerData = LoadPlayerTransformFromJson();
        player.position = playerData.playerPosition;
        player.rotation = Quaternion.Euler(playerData.playerRotation);
        player.localScale = playerData.playerScale;
    }
}
