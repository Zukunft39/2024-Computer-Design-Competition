using System;
using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridMap : MonoBehaviourSingleton<GridMap>
{
    //格子预制体
    public GameObject gridGO;
    //是否在开始时构建
    public bool isContructOnStart;
    //是否在未在编辑器运行时时构建
    public bool isContructOnValidate;
    //格子属性
    public Grid[,] gridAttrs;
    
    //格子图大小
    private int gridMapNum=7;
    //格子列表
    private GameObject[,] grids;
    //格子大小
    private float gridSize;
    
    /// <summary>
    ///  构建格子图
    /// </summary>
    /// <param name="_gridMapNum">格子图大小</param>
    public void ConstructGrid(int _gridMapNum)
    {
        gridMapNum = _gridMapNum;
        Vector3 temp = transform.position;
        float firstX = temp.x;
        grids = new GameObject[gridMapNum+1 , gridMapNum+1];
        gridAttrs = new Grid[gridMapNum+1 , gridMapNum+1];
        gridGO.transform.localScale = GouguData.Instance.gridSize * Vector3.one;
        //遍历生成格子
        for (int i = 0; i < gridMapNum+1; i++)
        {
            for (int j = 0; j < gridMapNum+1; j++)
            {
                if (j != gridMapNum&&i!=gridMapNum)
                {
                    //一般情况，实例化格子预制体
                    GameObject currentGrid = Instantiate(gridGO, temp, quaternion.identity);
                    grids[i, j] = currentGrid;
                    //对格子设置格子属性
                    gridAttrs[i, j] = currentGrid.GetComponent<Grid>();
                    gridAttrs[i, j].index = (i, j);
                    currentGrid.transform.parent = transform;
                    //x坐标增加一个格子大小
                    temp.x += GouguData.Instance.gridSize ;
                }
                else
                {
                    //最后一行/列时，生成空物体
                    GameObject currentGrid = Instantiate(new GameObject(), temp, quaternion.identity);
                    grids[i, j] = currentGrid;
                    //为空物体添加格子属性
                    gridAttrs[i, j] = currentGrid.AddComponent<Grid>();
                    gridAttrs[i, j].index = (i, j);
                    currentGrid.transform.parent = transform;
                    //最后一个格子时，x坐标回到初始位置
                    if(j == gridMapNum)temp.x = firstX;
                    //否则x坐标增加一个格子大小
                        else temp.x += GouguData.Instance.gridSize ;
                }
            }
            //y坐标增加一个格子大小
            temp.y+=GouguData.Instance.gridSize;
        }
    }
    
    /// <summary>
    /// 清除格子图
    /// </summary>
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
    
    /// <summary>
    ///  获取格子锚点位置
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public Vector3 GetGridPivotPos(int i, int j)
    {
        return grids[i, j].transform.position;
    }
    
    /// <summary>
    ///  搜索离给定点最近的格子锚点位置
    /// </summary>
    /// <param name="p">给定点的位置</param>
    /// <returns></returns>
    public (Vector3,int,int) FindNearestGridPivot(Vector3 p)
    {
        return FindNearestGridPivotRecursiveFunc(p,
            new Vector2(0, gridMapNum+1 ),
            new Vector2(0, gridMapNum+1));
    }
    
    // 递归搜索函数
    (Vector3,int,int) FindNearestGridPivotRecursiveFunc(Vector3 p,Vector2 xRange,Vector2 yRange)
    {
        //如果x和y的范围都足够小，递归结束，返回当前格子锚点位置
        if (xRange.y-xRange.x<=0.2f && yRange.y-yRange.x<=0.2f)
            return (grids[(int)yRange.x, (int)xRange.x].transform.position,(int)yRange.x,(int)xRange.x);
        //否则，根据x和y的范围中点，将范围缩小一半
        float xMid = 0.5f * (xRange.x + xRange.y);
        float yMid = 0.5f * (yRange.x + yRange.y);
        //根据给定点的位置，确定下一步搜索的范围
        if (p.x < transform.position.x + gridSize* xMid) xRange.y = xMid;
        else xRange.x = xMid;
        if (p.y < transform.position.y + gridSize* yMid) yRange.y = yMid;
        else yRange.x = yMid;
        //递归搜索
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
        // 根据条件在editor界面构建格子图
       if(isContructOnValidate) ConstructGrid(gridMapNum);
    }
}
