using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CreateChopCircleLevelSO")]
public class ChopCircleLevelSO : ScriptableObject
{
    public Level[] levels;
}

[Serializable]
public struct TargetAreaData
{
    public ChopGameSlider.DistributeType distributeType;
    public float radiusFrom;
    public float radiusTo;
    public Color color;
}

[Serializable]
public struct Level
{
    public string levelName;
    public TargetAreaData[] targetAreaDatas;
    public float sliderDuration;
}