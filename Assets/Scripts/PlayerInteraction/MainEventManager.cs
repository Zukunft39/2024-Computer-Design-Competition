using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainEventManager : MonoBehaviour
{
    public float timeScale = 1;
    public GameObject BeginCanvas;
    private static MainEventManager instance;
    public static MainEventManager Instance{
        get{
            if (instance == null)
            {
                instance = FindObjectOfType<MainEventManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MainEventManager).Name);
                    instance = singletonObject.AddComponent<MainEventManager>();
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as MainEventManager;
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
    void Start() {
        ShowGameStartPanel();
    }
    void Update() {
        
    }
    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }
    public void HideCursor(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ShowCursor(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void TimeScale(){
        Time.timeScale = timeScale;
    }
    /// <summary>
    /// 判断游戏是否达到游戏判定胜利条件,如果是则返回true
    /// </summary>
    /// <returns></returns>
    public bool GameEnds(){
        int npcCount = 0;
        int[] NPCNum = {0,0,0,0,0,0};
        foreach(var npc in GameObject.FindGameObjectsWithTag("NPC")){
            NPCNum[npcCount] = PlayerPrefs.GetInt("HasSpoken" + npc.name, 0);
            Debug.Log("当前"+npc.name+"是否已经对话:"+NPCNum[npcCount]);
            npcCount++;
        }
        return NPCNum.All(x => x == NPCNum[0]);
    }
    public void ShowGameStartPanel(){
        ShowCursor();
        BeginCanvas.SetActive(true);
    }
    public void ShowGameOverPanel(){
        
    }
}
