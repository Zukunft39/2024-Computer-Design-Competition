using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CircleRenderer : MonoBehaviour
{
    public LineRenderer circleRenderer;
    public LineRenderer chopLineRenderer;
    public float radius;
    public int circleSegments;
    [FormerlySerializedAs("chopTimes")] public int edgeCount;
    public float chopDuration;
    public AnimationCurve choppingCurve;
    
    public Text piText;

    enum Status
    {
        Idle,
        Chopping
    }

    private Status status = Status.Idle;
    private float chopTimer;
    private float[] startAngles, endAngles;
    private int chopTimes;
    private float angleOffset;
    private float estimatedPi;

    /// <summary>
    /// 円を表示する
    /// </summary>
    /// <param name="segments">円の間隔</param>
    void ShowCircle(int segments)
    {
        circleRenderer.positionCount = segments + 1;
        circleRenderer.useWorldSpace = false;
        float angle = 20f;
        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            circleRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (360f / segments);
        }
    }

    /// <summary>
    /// チョップラインを表示する
    /// </summary>
    void ShowChopLines()
    {
        switch (status)
        {
            case Status.Idle:
                //ShowIdleChoplines();
                break;
            case Status.Chopping:
                chopTimer += Time.deltaTime;
                ShowChoppingChopLines(chopTimer / chopDuration);
                EstimatePi();
                //Switch Status
                if (chopTimer >= chopDuration)
                {
                    
                    chopTimer = 0;
                    edgeCount *= 2;
                    status = Status.Idle;
                }

                break;
            default:
                break;
        }
    }

    void EstimatePi()
    {
        float distance = 0;
        for (int i = 0; i < chopLineRenderer.positionCount-1; i++)
        {
            distance+= Vector3.Distance(chopLineRenderer.GetPosition(i), chopLineRenderer.GetPosition(i + 1));
        }
        estimatedPi =(float)Math.Round( distance / (2 * radius),3);
        piText.text = "预估π值  " + estimatedPi;
    }
    
    void ShowInitialChopLines()
    {
        chopLineRenderer.useWorldSpace = false;
        float angle = angleOffset;
        chopLineRenderer.positionCount = edgeCount + 1;
        for (int i = 0; i < edgeCount + 1; i++)
        {
           float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
           float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            chopLineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += 360f / edgeCount;
        }
    }

    void ShowChoppingChopLines(float t)
    {
        for (int i = 0; i < edgeCount*2 + 1; i++)
        {
           float angle= Mathf.LerpUnclamped(startAngles[i/2], endAngles[i], choppingCurve.Evaluate(t));
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            chopLineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    
    /// <summary>
    /// 全てのチョップラインを削除する
    /// </summary>
    void ClearChopLines()
    {
        chopLineRenderer.positionCount = 0;
    }
    
    /// <summary>
    /// Chop the Circle
    /// </summary>
    public void Chop()
    {
        chopTimes++;
        ClearChopLines();
        //Set LineRenderer for chopping
        chopLineRenderer.useWorldSpace = false;
        chopLineRenderer.positionCount = edgeCount*2 + 1;
        //Precalculate start and end angle
        startAngles = new float[edgeCount+1];
        endAngles = new float[edgeCount*2+1];
        for(int i=0;i<edgeCount+1;i++)
        {
            startAngles[i] = 360f / edgeCount * i+angleOffset;
        }

        float newOffset = 360f / (edgeCount * 4);
        for(int i=0;i<edgeCount*2+1;i++)
        {
            endAngles[i] = 360f / (edgeCount*2) * i+angleOffset+newOffset;
        }

        angleOffset += newOffset;
        //Switch Status
        status = Status.Chopping;
    }

    private void Start()
    {
        EstimatePi();
        ShowInitialChopLines();
    }

    private void Update()
    {
        ShowCircle(circleSegments);
        ShowChopLines();
    }
    
}