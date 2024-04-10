using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GouguGameplay : MonoBehaviour
{
    public PlayableDirector director;
    public ResultPanel resultPanel;
    public Text timeTxt;
    
    private float[] timeArray;
    private int curGamePhase;
    private float timer;
    private bool isGaming;
    
    IEnumerator Gameplay()
    {
        yield return new TutorialPanel.WaitForTutorialEnd();
        
        for(curGamePhase=0;curGamePhase<GouguData.Instance.levels.Length;curGamePhase++)
        { 
            isGaming = true;
            timer = Time.time;
            yield return new GridGameManager.WaitForGameEnds(curGamePhase);
            timeArray[curGamePhase] = Time.time - timer;
            isGaming = false;
            timeTxt.text = "";
        }
        
        director.Play();
        yield return new WaitForSeconds((float)director.duration-0.5f);
        director.Pause();
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        string resultTxt = "";
        for (int i=0;i<timeArray.Length;i++)
        {
            resultTxt += $"第{i + 1}关用时：{timeArray[i]:F}秒\n";
        }
        resultPanel.ShowResultPanel(resultTxt);
    }

    private void Awake()
    {
        timeArray = new float[GouguData.Instance.levels.Length];
    }

    private void Start()
    {
        StartCoroutine(Gameplay());
    }

    private void Update()
    {
        if (isGaming)
        {
            float curTime = Time.time;
            timeTxt.text = $"当前用时：{(curTime - timer):F}秒";
        }
    }
}
