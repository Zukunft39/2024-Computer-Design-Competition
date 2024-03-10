using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect rect;
    private RectTransform content;
    private int pageCount = 0;
    private float[] pages;

    [Header("滚动参数")]
    [SerializeField] private float moveTime = 4f;
    [SerializeField] private float startPos;//开始移动位置
    [Tooltip("当前位置")]
    [SerializeField] private int currectIndex;
    private float timer = 0;//计时器

    private bool isMove = false;//判断UI是否开始移动
    private bool isDrag = true;//判断是否拖拽

    [Header("按钮参数")]
    [Tooltip("左按钮")]
    [SerializeField] private Button LeftBtn;
    [Tooltip("右按钮")]
    [SerializeField] private Button RightBtn;

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        ListenerMove();
    }
    
    //初始化
    private void Init()
    {
        rect = GetComponent<ScrollRect>();
#if UNITY_EDITOR
        if (rect == null) Debug.LogError("No ScrollRect Component!");
#endif
        content = rect.content;
        pageCount = rect.content.childCount;
        pages = new float[pageCount];
        currectIndex = 0;

        LeftBtn.onClick.AddListener(LeftScroll);
        RightBtn.onClick.AddListener(RightScroll);

        //给pages数组初始化
        for (int x = 0; x < pageCount; x++)
        {
            pages[x] = x * (1.0f / (float)(pageCount - 1));
#if UNITY_EDITOR
            Debug.Log("pages[" + x + "]" + ":" + pages[x]);
#endif
        }
    }
    //监听滑动
    private void ListenerMove()
    {
        if (isMove)
        {
            timer += Time.deltaTime * moveTime;
            rect.horizontalNormalizedPosition = Mathf.Lerp(startPos, pages[currectIndex], timer * 10);
            if (timer >= 1) isMove = false;
        }
    }
    //监听翻动
    private void LeftScroll()
    {
        if (currectIndex < 0) return;
        currectIndex--;
        PageTo(currectIndex);
    }
    private void RightScroll()
    {
        currectIndex++;
        currectIndex %= pages.Length;
        PageTo(currectIndex);
    }
    
    private void PageTo(int index)
    {
        isMove = true;
        currectIndex = index;
        timer = 0;
        startPos = rect.horizontalNormalizedPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int temp = 0;
        for(int i = 0; i < pages.Length; i++)
        {
            if (MathF.Abs(rect.horizontalNormalizedPosition - pages[i]) < MathF.Abs(rect.horizontalNormalizedPosition - pages[temp])) temp = i;
        }
        PageTo(temp);
        isDrag = false;
    }
}
