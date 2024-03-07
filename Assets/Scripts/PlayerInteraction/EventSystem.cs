using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HideCursor();
    }
    /// <summary>
    /// 锁定鼠标位置并隐藏鼠标.
    /// </summary>
    private void HideCursor(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
