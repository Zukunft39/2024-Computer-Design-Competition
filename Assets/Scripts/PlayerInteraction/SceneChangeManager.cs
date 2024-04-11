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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (asyncLoad.progress < 0.9f)
        {
            // 更新UI显示进度
            //yourProgressBar.value = progress;
            yield return null;
        }
        if(sceneName != "DemoScene") SavePlayerPosition();
        StartCoroutine(Init(sceneName));
    }
    public IEnumerator Init(string sceneName){
        // 通用设置
        MainEventManager.Instance.ShowCursor();
        MainAudioManager.AudioManagerInstance.StopMusic();
        MainAudioManager.AudioManagerInstance.StopSFX();

        if(sceneName == "DemoScene"){
            yield return new WaitForSeconds(0.75f); 
            if(!player) player = GameObject.FindWithTag("Player").transform;
            player.GetComponent<CharacterController>().enabled = false;
            player = RestorePlayerPosition(player);
            player.GetComponent<CharacterController>().enabled = true;
            MainAudioManager.AudioManagerInstance.PlayMusic("MainBackGroundMusic");
            MainEventManager.Instance.HideCursor();  
        }
        else{
            MainAudioManager.AudioManagerInstance.PlayMusic("SceneMusic");
            yield return new WaitForSeconds(0.35f);
        }
    }

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

        playerData.playerPosition = _PlayerTransform.position;
        playerData.playerRotation = _PlayerTransform.rotation.eulerAngles;
        playerData.playerScale = _PlayerTransform.localScale;
        SavePlayerTransformToJson(playerData);
    }
    public Transform RestorePlayerPosition(Transform playerTrans) {
        playerData = LoadPlayerTransformFromJson();

        playerTrans.position = playerData.playerPosition;
        playerTrans.rotation = Quaternion.Euler(playerData.playerRotation);
        playerTrans.localScale = playerData.playerScale;
        return playerTrans;
    }
}

