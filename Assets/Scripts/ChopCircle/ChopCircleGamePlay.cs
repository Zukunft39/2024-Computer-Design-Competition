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
    public int maxGamePhase;
    public CircleRenderer circleRenderer;
    public float transitionDuration;
    public ChopCircleLevelSO levelSo;
    public Text missText;
    public Text timeText;
    public Text levelnameText;
    public float totalTime;
    public int missTimes
    {
        get { return missTimesPri; }
        set
        {
            missTimesPri = value;
            OnMissed();
        }
    }

    public String endingText;

    private int missTimesPri;
    private int gamePhase;
    private bool isGaming;
    private bool isTransitioning;

    private void Awake()
    {
        instance=this;
    }

    private void Start()
    {
        InputJudge.instance.finishEvent+=FinishGame;
        InitializeGame();
    }

    private void Update()
    {
        if(isGaming&&!isTransitioning)
        {
            totalTime += Time.deltaTime;
            timeText.text = "Time: " + totalTime.ToString("F2")+"s";
        }
    }

    public void InitializeGame()
    {
        gamePhase = 0;
        
        StartGame();
    }
    
    void StartGame()
    {
        if(gamePhase>=maxGamePhase)
        {
            //Game Clear
            Debug.Log("Game Clear");
            isGaming = false;
            levelnameText.text= endingText;
            return;
        }
        Debug.Log("Start Game");
        chopGameSlider.ActivateHandle();
        isGaming= true;
        levelnameText.text = levelSo.levels[gamePhase].levelName;
        GenerateTargetAreas();
    }
    void FinishGame()
    {
        chopGameSlider.DeactivateHandle();
        isGaming = false;
        gamePhase++;
        circleRenderer.Chop();
        StartCoroutine(transitionToNextPhase());
    }
    
    IEnumerator transitionToNextPhase()
    {
        isTransitioning= true;
        yield return new WaitForSeconds(transitionDuration);
        Debug.Log("Next Phase");
        isTransitioning= false;
        chopGameSlider.ResetSlider();
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
        missText.text = "Miss: " + missTimesPri;
    }
}
