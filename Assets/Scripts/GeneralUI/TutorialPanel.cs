using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 教程面板
/// </summary>
public class TutorialPanel : MonoBehaviourSingleton<TutorialPanel>
{
   [SerializeField] private GameObject tutoriaPanel;
   [SerializeField] private Mask mask;
   private static bool flag=true;
   
   /// <summary>
   /// 显示面板
   /// </summary>
    void ShowPanel()
   {
      tutoriaPanel.gameObject.SetActive(true);
   }
   
   /// <summary>
   /// 关闭面板
   /// </summary>
   public void ClosePanel()
   {
      SoundManager.Instance.PlaySFX("btnClick");
      tutoriaPanel.gameObject.SetActive(false);
      flag = false;
      mask.FadeOut();
   }
   
   /// <summary>
   /// 面板异步返回类
   /// </summary>
   public class WaitForTutorialEnd : CustomYieldInstruction
   {
      public WaitForTutorialEnd()
      {
         TutorialPanel.Instance.ShowPanel();
      }
      public override bool keepWaiting
      {
         get
         {
            return flag;
         }
      }
   }
}
