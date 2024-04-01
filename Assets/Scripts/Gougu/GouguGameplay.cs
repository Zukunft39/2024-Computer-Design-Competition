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
    
    private float[] timeArray;
    
    IEnumerator Gameplay()
    {
        for(int i=0;i<GouguData.Instance.levels.Length;i++)
        {
            float t = Time.time;
            yield return new GridGameManager.WaitForGameEnds(i);
            timeArray[i] = Time.time - t;
        }
        
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        
        string resultTxt = "";
        for (int i=0;i<timeArray.Length;i++)
        {
            resultTxt += $"第{i + 1}关用时：{timeArray[i]:F}秒\n";
        }
        resultPanel.ShowResultPanel(resultTxt);
        //TODO Switch Scene
        
    }

    private void Awake()
    {
        timeArray = new float[GouguData.Instance.levels.Length];
    }

    private void Start()
    {
        StartCoroutine(Gameplay());
    }
}
