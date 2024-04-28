using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //格子当前的三角形列表
    public List<Triangle> triList;
    //格子坐标
    public (int, int) index;

    private void Awake()
    {
        //初始化
        triList = new();
    }
}
