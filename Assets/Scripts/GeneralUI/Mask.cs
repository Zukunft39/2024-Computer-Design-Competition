using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour
{
    public Color initColor;
    
    private Image maskImage;
    private enum State
    {
        IDLE,FADEIN,FADEOUT
    }
    
    private State state = State.IDLE;

    private void Awake()
    {
        maskImage = GetComponent<Image>();
    }

    private void Start()
    {
        maskImage.color = initColor;
    }
    
    public void FadeIn(float duration=1)
    {
        StartCoroutine(FadeInCoroutine(duration));
    }
    
    public void FadeOut(float duration=1)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }
    
    private IEnumerator FadeInCoroutine(float duration)
    {
        state = State.FADEIN;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            maskImage.color = new Color(initColor.r, initColor.g, initColor.b, Mathf.Lerp(0, 1, timer / duration));
            yield return null;
        }
        state = State.IDLE;
    }
    
    private IEnumerator FadeOutCoroutine(float duration)
    {
        state = State.FADEOUT;
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            maskImage.color = new Color(initColor.r, initColor.g, initColor.b, Mathf.Lerp(1, 0, timer / duration));
            yield return null;
        }
        state = State.IDLE;
    }
}
