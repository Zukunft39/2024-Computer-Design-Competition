using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BackgroundDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PaintingPen paintingPen;
    public void OnPointerEnter(PointerEventData eventData)
    {
        paintingPen.isMouseEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        paintingPen.isMouseEnter = false;
    }
}
