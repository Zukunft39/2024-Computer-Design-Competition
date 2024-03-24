using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingDynamic : MonoBehaviour
{
    public PaintingPen paintingPen;
    public float MaxSize= 1.0f;
    public float rate = 5;

    public void Update()
    {

        if (paintingPen.isMouseDown)
        {
            if (paintingPen.widthPower<MaxSize)
                paintingPen.widthPower += Time.deltaTime*rate;
        }
       else
        {
            paintingPen.widthPower = 0;
        }
    }
}
