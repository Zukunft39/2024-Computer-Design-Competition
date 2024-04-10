using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class ChopCircleGamePlay : MonoBehaviour
{
    public static ChopCircleGamePlay instance;
    public ChopGameSlider chopGameSlider;
    public CircleRenderer circleRenderer;
    public float transitionDuration;
    public ChopCircleLevelSO levelSo;
    public Text missText;
    public Text timeText;
    public Text levelnameText;
    public ResultPanel resultPanel;

    private String resultTxt;
    private float[] levelTimeList;
    private int[] missTimeList;
    public int missTimes
    {
        get { return missTimesPri; }
        set
        {
            missTimesPri = missTimeList[gamePhase]  = value;
            OnMissed();
        }
    }
    private int missTimesPri;
    
    private int gamePhase;
    private bool isGaming;
    private bool isTransitioning;

    private void Awake()
    {
        instance=this;
        levelTimeList = new float[levelSo.levels.Length];
        missTimeList = new int[levelSo.levels.Length];
        resultTxt = "";
    }

    private void Start()
    {
        InputJudge.instance.finishEvent+=FinishGame;
        chopGameSlider.
        StartCoroutine(GameplayCoroutine());
    }

    private void Update()
    {
        if(isGaming&&!isTransitioning)
        {
            levelTimeList[gamePhase] += Time.deltaTime;
            timeText.text = "当前用时  " + levelTimeList[gamePhase].ToString("F2")+"秒";
        }
    }

    IEnumerator GameplayCoroutine()
    {
        yield return new TutorialPanel.WaitForTutorialEnd();
        StartGame();
    }
    
    void StartGame()
    {
        if(gamePhase>=levelSo.levels.Length)
        {
            GameClear();
            return;
        }
        isGaming= true;
//        levelnameText.text = levelSo.levels[gamePhase].levelName;
        chopGameSlider.handleMoveDuration = levelSo.levels[gamePhase].sliderDuration;
        GenerateTargetAreas();
        if(gamePhase==0) chopGameSlider.ActivateHandle();
        else chopGameSlider.ActivateHandle(levelSo.levels[gamePhase-1].sliderDuration,levelSo.levels[gamePhase].sliderDuration);
    }
    void FinishGame()
    {
        if(gamePhase<=levelSo.levels.Length/3)
        {
            SoundManager.Instance.PlaySFX("appr1");
        }else if (gamePhase <= levelSo.levels.Length / 3 * 2)
        {
            SoundManager.Instance.PlaySFX("appr2");
        }
        else if(gamePhase < levelSo.levels.Length -1)
        {
            SoundManager.Instance.PlaySFX("appr3");
        }
        else
        {
            SoundManager.Instance.PlaySFX("appr4");
        }

        chopGameSlider.DeactivateHandle();
        isGaming = false;
        gamePhase++;
        circleRenderer.Chop();
        StartCoroutine(transitionToNextPhase());
    }

    void GameClear()
    {
        isGaming = false;
        float totalTime=0;
        for (int i = 0; i < gamePhase; i++)
        {
            resultTxt += $"<b>第{i + 1}关</b>\n耗时:{levelTimeList[i]:F}秒\n失误次数:{missTimeList[i]}次\n";
            totalTime += levelTimeList[i];
        }
        resultTxt += $"<b>总时间:{totalTime:F}秒</b>";
        resultPanel.ShowResultPanel(resultTxt);
    }
    
    IEnumerator transitionToNextPhase()
    {
        isTransitioning= true;
        yield return new WaitForSeconds(transitionDuration);
        isTransitioning= false;
        StartGame();
    }

    void GenerateTargetAreas()
    {
        foreach (var VARIABLE in levelSo.levels[gamePhase].targetAreaDatas)
        {
            chopGameSlider.GenerateOneTargetArea(VARIABLE.distributeType, Random.Range(VARIABLE.radiusFrom,VARIABLE.radiusTo), VARIABLE.color);
        }
    }

    void OnMissed()
    {
        missText.text = "失误次数  " + missTimesPri;
    }

    public bool GetGameState()
    {
        return isGaming;
    }
}
