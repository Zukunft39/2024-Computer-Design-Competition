using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChopGameSlider : MonoBehaviour
{
    public Animator sliderAnimator;
    public GameObject targetArea;
    public float targetAreaAlpha;
    public RectTransform sliderBg;
    public Transform leftPivot;
    public Transform rightPivot;
    public float normalClampRange=1;
    public GameObject handle;
    

    private List<GameObject> targetAreas;

    public void Slide()
    {
        sliderAnimator.SetTrigger("isSliding");
    }

     public enum DistributeType
    {
        Uniform,
        Normal
    }

    public void GenerateOneTargetArea(DistributeType distributeType,float radius,Color color)
    {
        float targetX=0;
        switch (distributeType)
        {
            case DistributeType.Uniform:
                targetX=Mathf.Lerp(leftPivot.position.x, rightPivot.position.x, Random.value);
                break;
            case DistributeType.Normal:
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
        GameObject _targetArea = Instantiate(targetArea, new Vector3(targetX, leftPivot.position.y, 0), Quaternion.identity);
        _targetArea.transform.localScale= new Vector3(radius, 1, 1);  
        _targetArea.GetComponent<Image>().color=new Color(color.r,color.g,color.b,targetAreaAlpha);
        _targetArea.transform.SetParent(sliderBg.transform);
        targetAreas.Add(_targetArea);
    }
    
    public List<GameObject> GetTargetAreas()
    {
        return targetAreas;
    }
    
    public void ActivateHandle()
    {
        handle.SetActive(true);
    }
    
    public void DeactivateHandle()
    {
        handle.SetActive(false);
    }
    
    private void Awake()
    {
        targetAreas = new();
    }

    
}