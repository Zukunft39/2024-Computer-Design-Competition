using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public List<Triangle> triList;
    public (int, int) index;

    private void Awake()
    {
        triList = new();
    }
}
