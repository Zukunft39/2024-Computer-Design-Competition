using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEventManager : MonoBehaviour
{
    public float timeScale = 1;
    void Start() {
        HideCursor();
    }
    void Update() {
        TimeScale();
    }
/// <summary>
/// 锁定鼠标位置并隐藏鼠标.
/// </summary>
    private void HideCursor(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void TimeScale(){
        Time.timeScale = timeScale;
    }
}
