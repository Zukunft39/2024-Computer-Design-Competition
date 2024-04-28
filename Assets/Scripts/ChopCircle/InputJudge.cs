using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputJudge : MonoBehaviour
{
    //割圆按键
    public KeyCode inputKey=KeyCode.Space;
    //割圆滑动条
    public ChopGameSlider chopGameSlider;
    //割圆手柄
    public Transform handle;
    //割圆完成事件
    public delegate void FinishEventDelegate();
    //单例引用
    public static InputJudge instance;
    //割圆完成事件
    public event FinishEventDelegate finishEvent;
    //割圆刀刃动画
    public Animator bladeAnimator;
    
    //目标区域列表
    private List<GameObject> targetAreas;
    
    //判断输入
    void JudgeInput()
    {
        targetAreas = chopGameSlider.GetTargetAreas();
        foreach (var i in targetAreas)
        {
            if(handle.position.x>i.transform.position.x-i.transform.localScale.x/2&&handle.position.x<i.transform.position.x+i.transform.localScale.x/2)
            {
                targetAreas.Remove(i);
                Destroy(i);
                JudgeIsFinish();
                return;
            }
        }
        ChopCircleGamePlay.instance.missTimes++;
    }
    
    //判断是否完成
    void JudgeIsFinish()
    {
        targetAreas = chopGameSlider.GetTargetAreas();
        if(targetAreas.Count==0)
        {
            if(finishEvent is not null)
            {
                finishEvent();
            }
        }
    }
    
    void Awake()
    {
        instance=this;
    }

    private void Update()
    {
        //检测按下按键
        if(Input.GetKeyDown(inputKey)&& ChopCircleGamePlay.instance.GetGameState())
        {
            JudgeInput();
            bladeAnimator.SetTrigger("cut");
            SoundManager.Instance.PlaySFX("knifeCut",0.12f);
        }
    }
   
}
