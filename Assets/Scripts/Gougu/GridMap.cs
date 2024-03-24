using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public float spacingSize;
    public GameObject gridGO;
    public bool isContructOnStart;
    public bool isContructOnValidate;

    private List<GameObject> grids;

    void ConstructGrid()
    {
        Vector3 temp = transform.position;
        float firstX = temp.x;
        gridGO.transform.localScale = GouguData.Instance.gridSize * Vector3.one;
        //3x3
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject currentGrid = Instantiate(gridGO, temp, quaternion.identity);
                currentGrid.transform.parent = transform;
                temp.x += GouguData.Instance.gridSize + spacingSize;
                if (j == 2)
                {
                    temp.x = firstX;
                }
            }
            temp.y+=GouguData.Instance.gridSize + spacingSize;
        }
    }
    
    void ShowGrid()
    {
        
    }

    private void Awake()
    {
        grids = new();
    }

    private void Start()
    {
        if(isContructOnStart) ConstructGrid();
    }

    private void OnValidate()
    {
       if(isContructOnValidate) ConstructGrid();
    }
}
