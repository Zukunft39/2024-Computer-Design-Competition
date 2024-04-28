using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 滑动视窗控件
/// </summary>
public class SlideView : MonoBehaviour
{
   
   private RectTransform[] items;
   [SerializeField] private RectTransform contentPort;
   [SerializeField] private float slideDuratiuon;
   [SerializeField] private AnimationCurve slideCurve;
   private int itemsCount;
   private bool isSliding;
   private int curPage;
   private int prevPage;
   private float curContentX;
   private float destContentX;
   private float slideStartTime;
   private float contentTotalWidth=0;
   
   private void Awake()
   {
      //get items
      items = new RectTransform[contentPort.childCount];
      for (int i = 0; i < items.Length; i++) items[i] = (RectTransform)contentPort.GetChild(i);
      itemsCount = items.Length;
   }

   private void Start()
   {
      //set width of the content port
      contentTotalWidth=0;
      foreach (var i in items)
      {
         contentTotalWidth += i.rect.width;
      }
   }
   
   /// <summary>
   /// 设置页数
   /// </summary>
   /// <param name="m_page">页数</param>
   void SetPage(int m_page)
   {
      if (m_page >= itemsCount || m_page < 0) return;
      prevPage = curPage;
      curPage = m_page;
      float totalWidthNeedsToTranslation=0;
      for (int i = 0; i < m_page; i++)
      {
         totalWidthNeedsToTranslation += items[i].rect.width;
      }
      curContentX = contentPort.anchoredPosition.x;
      destContentX = -totalWidthNeedsToTranslation;
      isSliding = true;
      slideStartTime = Time.time;
   }
   
   /// <summary>
   /// 上一页
   /// </summary>
   public void PageUp()
   {
      //if(isSliding)return;
      SetPage(curPage+1);
   }
   
   /// <summary>
   /// 下一页
   /// </summary>
   public void PageDown()
   {
      //if(isSliding)return;
      SetPage(curPage-1);
   }
   
   private void Update()
   {
      if (isSliding)
      {
         //状态更新
         if ((Time.time - slideStartTime) / slideDuratiuon >= 1)
         {
            isSliding = false;
            slideStartTime = 0;
         }
         
         //插值计算滑动坐标
         float tmpX = Mathf.LerpUnclamped(curContentX, destContentX,
            slideCurve.Evaluate((Time.time - slideStartTime) / slideDuratiuon));
         contentPort.anchoredPosition = new Vector2(tmpX, contentPort.anchoredPosition.y);
      }
   }
}
