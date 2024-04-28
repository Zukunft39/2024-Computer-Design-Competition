using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  勾股定理关卡数据储存体
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/CreateGouguLevelSO")]
public class GouguLevelSO : ScriptableObject
{
    //关卡数据
    public string levelName;
    
    //格子到三角形的映射
    [System.Serializable]
    public struct Grid2TriMap
    {
        //格子坐标
        public int i;
        public int j;
        //三角形方向
        public Triangle.TriDir dir;
        //三角形类型
        public int type;
    }
    
    //关卡数据
    [System.Serializable]
    public struct LevelData
    {
        //关卡ID
        public int level_id;
        //格子到三角形的映射集
        public Grid2TriMap[] maps;
        //引导线
        public GameObject guideLine;
        //格子大小
        public int gridMapSize;
    }
    
    //关卡数据
    public LevelData levelData;
}


