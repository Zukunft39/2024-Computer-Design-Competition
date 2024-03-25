using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CreateGouguLevelSO")]
public class GouguLevelSO : ScriptableObject
{
    public string levelName;
    
    [System.Serializable]
    public struct Grid2TriMap
    {
        public int i;
        public int j;
        public Triangle.TriDir dir;
        public int type;
    }

    [System.Serializable]
    public struct LevelData
    {
        public int level_id;
        public Grid2TriMap[] maps;
        public GameObject guideLine;
        public int gridMapSize;
    }

    public LevelData levelData;
    
    // [System.Serializable]
    // public struct GuideLineData
    // {
    //     public int i1,j1;
    //     public int i2, j2;
    // }
    //
    // public GuideLineData[] guideLineDatas;
    
}


