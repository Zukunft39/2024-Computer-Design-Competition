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
    
    private List<GameObject> targetAreas;
    
    void JudgeInput()
    {
        targetAreas = chopGameSlider.GetTargetAreas();
        foreach (var i in targetAreas)
        {
            if(handle.position.x>i.transform.position.x-i.transform.localScale.x/2&&handle.position.x<i.transform.position.x+i.transform.localScale.x/2)
            {
                Debug.Log("Hit");
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
            Debug.Log("Finish");
            if(finishEvent!=null)
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
        if(Input.GetKeyDown(inputKey))
        {
            JudgeInput();
        }
    }
    
   
}
