using System;
using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridMap : MonoBehaviourSingleton<GridMap>
{
    public GameObject gridGO;
    public bool isContructOnStart;
    public bool isContructOnValidate;
    public Grid[,] gridAttrs;
    
    private int gridMapNum=7;
    private GameObject[,] grids;
    private float gridSize;
    public void ConstructGrid(int _gridMapNum)
    {
        gridMapNum = _gridMapNum;
        Vector3 temp = transform.position;
        float firstX = temp.x;
        grids = new GameObject[gridMapNum+1 , gridMapNum+1];
        gridAttrs = new Grid[gridMapNum+1 , gridMapNum+1];
        gridGO.transform.localScale = GouguData.Instance.gridSize * Vector3.one;
        for (int i = 0; i < gridMapNum+1; i++)
        {
            for (int j = 0; j < gridMapNum+1; j++)
            {
                if (j != gridMapNum&&i!=gridMapNum)
                {
                    GameObject currentGrid = Instantiate(gridGO, temp, quaternion.identity);
                    grids[i, j] = currentGrid;
                    gridAttrs[i, j] = currentGrid.GetComponent<Grid>();
                    gridAttrs[i, j].index = (i, j);
                    currentGrid.transform.parent = transform;
                    temp.x += GouguData.Instance.gridSize ;
                }
                else
                {
                    GameObject currentGrid = Instantiate(new GameObject(), temp, quaternion.identity);
                    grids[i, j] = currentGrid;
                    gridAttrs[i, j] = currentGrid.AddComponent<Grid>();
                    gridAttrs[i, j].index = (i, j);
                    currentGrid.transform.parent = transform;
                    if(j == gridMapNum)temp.x = firstX;
                        else temp.x += GouguData.Instance.gridSize ;
                }
            }
            temp.y+=GouguData.Instance.gridSize;
        }
    }

    public void DeconstructGrid()
    {
        for (int i = 0; i < gridMapNum + 1; i++)
        {
            for (int j = 0; j < gridMapNum + 1; j++)
            {
                Destroy(grids[i,j]);
            }
        }
        gridAttrs = null;
    }

    public Vector3 GetGridPivotPos(int i, int j)
    {
        return grids[i, j].transform.position;
    }

    public (Vector3,int,int) FindNearestGridPivot(Vector3 p)
    {
        return FindNearestGridPivotRecursiveFunc(p,
            new Vector2(0, gridMapNum+1 ),
            new Vector2(0, gridMapNum+1));
    }

    (Vector3,int,int) FindNearestGridPivotRecursiveFunc(Vector3 p,Vector2 xRange,Vector2 yRange)
    {
        if (xRange.y-xRange.x<=0.2f && yRange.y-yRange.x<=0.2f)
            return (grids[(int)yRange.x, (int)xRange.x].transform.position,(int)yRange.x,(int)xRange.x);
        float xMid = 0.5f * (xRange.x + xRange.y);
        float yMid = 0.5f * (yRange.x + yRange.y);
        if (p.x < transform.position.x + gridSize* xMid) xRange.y = xMid;
        else xRange.x = xMid;
        if (p.y < transform.position.y + gridSize* yMid) yRange.y = yMid;
        else yRange.x = yMid;
        return FindNearestGridPivotRecursiveFunc(p, xRange, yRange);
    }

    private void Awake()
    {
        gridSize = GouguData.Instance.gridSize;
    }

    private void Start()
    {
        
        //if(isContructOnStart) ConstructGrid();
    }

    private void OnValidate()
    {
       if(isContructOnValidate) ConstructGrid(gridMapNum);
    }
}
