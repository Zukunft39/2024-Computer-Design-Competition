using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judger : MonoBehaviourSingleton<Judger>
{
    private Hashtable curLevelHashTable;

    public void InitializeLevel(int level_id)
    {
        foreach (var i in GouguData.Instance.levels[level_id].levelData.maps)
        {
            curLevelHashTable.Add(GenerateHashKey(i.i,i.j,i.dir,i.type),1);
        }
    }

    public static int GenerateHashKey(int x, int y, Triangle.TriDir dir, int triType)
    {
        Debug.Log("x:"+x);
        Debug.Log("y:"+y);
        Debug.Log("dir:"+dir);
        Debug.Log("triType:"+triType);
        return Convert.ToInt32(Convert.ToString(x) + Convert.ToString(y)+Convert.ToString((int)dir)+Convert.ToString(triType));
    }

    public bool JudgeIsApproved(List<Triangle> tris)
    {
        if (GouguData.Instance.isDebug) return false;
        if (tris.Count != curLevelHashTable.Count) return false;
        bool isApproved = true;
        foreach (var tri in tris)
        {
            if (tri.pivotedGrid is null) return false;
            isApproved &= curLevelHashTable.ContainsKey(GenerateHashKey(tri.pivotedGrid.index.Item1,
                tri.pivotedGrid.index.Item2, tri.dir,tri.triType));
        }

        return isApproved;
    }

    void OnIsApproved()
    {
        curLevelHashTable.Clear();
    }
    
    
    private void Awake()
    {
        curLevelHashTable = new();
        GridGameManager.Instance.approvedAct += OnIsApproved;
    }
}
