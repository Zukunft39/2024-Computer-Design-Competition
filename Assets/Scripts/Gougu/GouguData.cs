using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GouguData : MonoBehaviourSingletonPersistent<GouguData>
{
    //数据类，存放勾股定理小游戏的全局数据
    
    //格子大小
    public float gridSize;
    //是否为调试模式
    public bool isDebug;
    //关卡数据
    public GouguLevelSO[] levels;
    //全局音频数据
    public SerializeableDictionary<string,AudioClip> audioDict;
}
