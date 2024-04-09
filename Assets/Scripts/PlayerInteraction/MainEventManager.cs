using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEventManager : MonoBehaviour
{
    public float timeScale = 1;
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
        HideCursor();
    }
    void Update() {
        TimeScale();
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
    
}
