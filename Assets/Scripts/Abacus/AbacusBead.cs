using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbacusBead
{
    [SerializeField] private int value;
    [SerializeField] private RectTransform Transform;
    [Tooltip("是否为算盘上珠")]
    [SerializeField] private bool isUpBead;

    Vector2 originPos;//记录原来位置
    int count = 0;//被点击次数

    public GameObject obj;//实例对象

    public AbacusBead(RectTransform transform, bool isUpBead)
    {
        Transform = transform;
        this.isUpBead = isUpBead;
        if (isUpBead) value = 5;
        else value = 1;
        originPos = transform.anchoredPosition;
    }
    #region 移动
    public void DragBead(Vector2 targetPos)
    {
        if (obj != null)
        {
            Transform.anchoredPosition += targetPos;
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Transform.anchoredPosition;
        }
#if UNITY_EDITOR
        else Debug.LogError("No AbacusBead Prefab!");
#endif
    }
    public void ReturnBead()
    {
        if (obj != null)
        {
            Transform.anchoredPosition = originPos;
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform = Transform;
        }
#if UNITY_EDITOR
        else Debug.LogError("No AbacueBead Prefab!");
#endif
    }
    #endregion
    #region 获取值
    public int GetValue()
    {
        return value;
    }
    public RectTransform GetRect()
    {
        return Transform;
    }
    public bool GetBool()
    {
        return isUpBead;
    }
    public int GetCount()
    {
        return count;
    }
    //设置被点击次数
    public void SetCount(int c)
    {
        count += c;
    }
    #endregion
}
