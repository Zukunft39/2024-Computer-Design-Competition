using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AbacusEventSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeScale = 1;

    void Start()
    {
        HideCursor();
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
