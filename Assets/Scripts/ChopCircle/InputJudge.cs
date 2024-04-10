using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputJudge : MonoBehaviour
{
    public KeyCode inputKey=KeyCode.Space;
    public ChopGameSlider chopGameSlider;
    public Transform handle;
    public delegate void FinishEventDelegate();
    public static InputJudge instance;
    public event FinishEventDelegate finishEvent;
    public Animator bladeAnimator;
    
    private List<GameObject> targetAreas;
    
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
        if(Input.GetKeyDown(inputKey)&& ChopCircleGamePlay.instance.GetGameState())
        {
            JudgeInput();
            bladeAnimator.SetTrigger("cut");
            SoundManager.Instance.PlaySFX("knifeCut",0.12f);
        }
    }
   
}
