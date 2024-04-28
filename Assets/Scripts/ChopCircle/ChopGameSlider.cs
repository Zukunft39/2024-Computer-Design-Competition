using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChopGameSlider : MonoBehaviour
{ 
    public AnimationCurve handleCurve;
    public float handleMoveDuration;
    public GameObject targetArea;
    public float targetAreaAlpha;
    public RectTransform sliderBg;
    public Transform leftPivot;
    public Transform rightPivot;
    public float normalClampRange=1;
    public GameObject handle;

    private List<GameObject> targetAreas;
    private float sliderTimer;
    private bool isSliding;
    
    /// <summary>
    ///  滑动
    /// </summary>
     void Slide()
    {
        if(!isSliding)return;
        sliderTimer += Time.deltaTime;
        handle.transform.position =
            Vector3.LerpUnclamped(leftPivot.position, rightPivot.position, handleCurve.Evaluate(
                (sliderTimer % handleMoveDuration)/handleMoveDuration));
    }
    
    /// <summary>
    ///  计算滑块起始位置
    /// </summary>
    /// <param name="previousDuration">前一关滑动周期</param>
    /// <param name="currentDuration">当前关卡滑动周期</param>
     void CalculateSliderStartPos(float previousDuration,float currentDuration)
     {
         sliderTimer = ((sliderTimer % previousDuration) / previousDuration)*currentDuration;
     }
    
    /// <summary>
    ///  重置滑块
    /// </summary>
    public void ResetSlider()
    {
        sliderTimer = 0;
    }
    
    // 目标区域概率分布
     public enum DistributeType
    {
        // 均匀分布
        Uniform,
        // 正态分布
        Normal
    }
    
     /// <summary>
     ///  生成一个目标区域
     /// </summary>
     /// <param name="distributeType">概率分布</param>
     /// <param name="radius">半径</param>
     /// <param name="color">颜色</param>
    public void GenerateOneTargetArea(DistributeType distributeType,float radius,Color color)
    {
        float targetX=0;
        switch (distributeType)
        {
            // 均匀分布
            case DistributeType.Uniform:
                targetX=Mathf.Lerp(leftPivot.position.x, rightPivot.position.x, Random.value);
                break;
            // 正态分布
            case DistributeType.Normal:
                // Box-Muller 变换利用二维正态分布的极坐标变换生成正态分布的随机数
                float u1 = Random.value;
                float u2 = Random.value;
                float x = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2 * Mathf.PI * u2);
                if (x > normalClampRange || x < -normalClampRange)
                {
                    GenerateOneTargetArea(distributeType, radius, color);
                    return;
                }
                targetX =Mathf.Lerp(leftPivot.position.x, rightPivot.position.x, (x + normalClampRange)/(2*normalClampRange));
                break;
        }
        // 生成目标区域
        GameObject _targetArea = Instantiate(targetArea, new Vector3(targetX, leftPivot.position.y, 0), Quaternion.identity);
        _targetArea.transform.localScale= new Vector3(radius, 1, 1);  
        _targetArea.GetComponent<Image>().color=new Color(color.r,color.g,color.b,targetAreaAlpha);
        _targetArea.transform.SetParent(sliderBg.transform);
        // 添加到目标区域列表
        targetAreas.Add(_targetArea);
    }
    
     // 获取目标区域
    public List<GameObject> GetTargetAreas()
    {
        return targetAreas;
    }
    // 激活滑块
    public void ActivateHandle()
    {
        isSliding = true;
    }
    // 激活滑块
    public void ActivateHandle(float previousDuration,float currentDuration)
    {
        CalculateSliderStartPos(previousDuration,currentDuration);
        isSliding = true;
    }
    // 停止滑块
    public void DeactivateHandle()
    {
        isSliding = false;
    }
    
    private void Awake()
    {
        targetAreas = new();
    }

    private void Update()
    {
        Slide();
    }
}