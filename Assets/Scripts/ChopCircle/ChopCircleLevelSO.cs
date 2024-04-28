using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 割圆术关卡数据储存体
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/CreateChopCircleLevelSO")]
public class ChopCircleLevelSO : ScriptableObject
{
    //关卡数据
    public Level[] levels;
}

/// <summary>
/// 目标区域数据
/// </summary>
[Serializable]
public struct TargetAreaData
{
    //目标区域分布
    public ChopGameSlider.DistributeType distributeType;
    //目标区域半径范围
    public float radiusFrom;
    public float radiusTo;
    //目标区域颜色
    public Color color;
}

/// <summary>
/// 关卡数据
/// </summary>
[Serializable]
public struct Level
{
    //关卡名称
    public string levelName;
    //目标区域数据
    public TargetAreaData[] targetAreaDatas;
    //滑动条滑动周期
    public float sliderDuration;
}