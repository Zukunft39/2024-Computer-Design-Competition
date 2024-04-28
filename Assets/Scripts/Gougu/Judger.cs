using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
///  判断器，用于判断当前关卡是否通过
/// </summary>
public class Judger : MonoBehaviourSingleton<Judger>
{
    //当前关卡的哈希表
    //key：格子坐标i,j,三角形方向，三角形类型
    //value：1表示存在，0表示不存在
    private Hashtable curLevelHashTable;
    
    // 初始化关卡
    public void InitializeLevel(int level_id)
    {
        //遍历关卡数据
        foreach (var i in GouguData.Instance.levels[level_id].levelData.maps)
        {
            //根据关卡数据将其添加到哈希表
            curLevelHashTable.Add(GenerateHashKey(i.i,i.j,i.dir,i.type),1);
        }
    }
    
    //利用位运算生成哈希键
    static int GenerateHashKey(int x, int y, Triangle.TriDir dir, int triType)
    {
        return ((x & 15) << 7) + ((y & 15) << 3) + ((int)dir << 1) + triType;
    }
    
    /// <summary>
    ///  判断是否通过
    /// </summary>
    /// <param name="tris"> 三角形列表</param>
    /// <returns></returns>
    public bool JudgeIsApproved(List<Triangle> tris)
    {
        //如果是调试模式，直接返回true
        if (GouguData.Instance.isDebug) return false;
        //如果三角形数量不等于哈希表数量，返回false
        if (tris.Count != curLevelHashTable.Count) return false;
        bool isApproved = true;
        //遍历三角形列表
        foreach (var tri in tris)
        {
            //如果三角形的格子为空，返回false
            if (tri.pivotedGrid is null) return false;
            //如果哈希表中不包含当前三角形，返回false
            isApproved &= curLevelHashTable.ContainsKey(GenerateHashKey(tri.pivotedGrid.index.Item1,
                tri.pivotedGrid.index.Item2, tri.dir,tri.triType));
        }
        //返回是否通过
        return isApproved;
    }
    
    // 关卡通过回调
    void OnIsApproved()
    {
        curLevelHashTable.Clear();
    }
    
    private void Awake()
    {
        //初始化哈希表
        curLevelHashTable = new();
        //订阅关卡通过事件
        GridGameManager.Instance.approvedAct += OnIsApproved;
    }
}
